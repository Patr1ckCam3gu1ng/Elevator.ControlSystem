using Elevator.ControlSystem.Commands;
using Elevator.ControlSystem.Models;
using Elevator.ControlSystem.Queries;
using Elevator.ControlSystem.Services;
using Elevator.ControlSystem.Services.Interfaces;
using Elevator.ControlSystem.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Elevator.ControlSystem;

class Program
{
    static async Task Main(string[] args)
    {
        ConfigureLogging();
        
        try
        {
            Log.Information("Starting elevator system");

            var configuration = LoadConfiguration();
            
            var serviceProvider = ConfigureServices(configuration);

            await InitializeElevatorSystem(serviceProvider);
        }
        catch (Exception ex)
        {
            LogUnhandledException(ex);
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    #region Configuration Methods

    /// <summary>
    /// Configures Serilog for logging.
    /// </summary>
    private static void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
    }

    /// <summary>
    /// Loads the application configuration from appSettings.json.
    /// </summary>
    /// <returns>The loaded configuration.</returns>
    private static IConfiguration LoadConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    #endregion

    #region Service Configuration

    /// <summary>
    /// Configures the services and dependency injection.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The configured service provider.</returns>
    private static ServiceProvider ConfigureServices(IConfiguration configuration)
    {
        var elevatorSettings = new ElevatorSettings();
        configuration.GetSection("ElevatorSettings").Bind(elevatorSettings);

        var requestSettings = new RequestSettings();
        configuration.GetSection("RequestSettings").Bind(requestSettings);

        var elevatorSystem = ElevatorSystemFactory.CreateElevatorSystem(elevatorSettings);

        return new ServiceCollection()
            .AddSingleton(elevatorSettings)
            .AddSingleton(requestSettings)
            .AddSingleton(elevatorSystem)
            .AddSingleton<IElevatorService, ElevatorService>()
            .AddSingleton<IRequestService, RequestService>()
            .AddSingleton<IMovementService, MovementService>()
            .AddSingleton<CommandHandler>()
            .AddSingleton<QueryHandler>()
            .BuildServiceProvider();
    }

    #endregion

    #region Elevator System Initialization

    /// <summary>
    /// Initializes the elevator system and starts processing requests.
    /// </summary>
    /// <param name="serviceProvider">The service provider for dependency injection.</param>
    private static async Task InitializeElevatorSystem(ServiceProvider serviceProvider)
    {
        var commandHandler = serviceProvider.GetService<CommandHandler>();
        var queryHandler = serviceProvider.GetService<QueryHandler>();
        var elevatorService = serviceProvider.GetService<IElevatorService>();
        var elevatorSettings = serviceProvider.GetService<ElevatorSettings>();
        var requestSettings = serviceProvider.GetService<RequestSettings>();

        if (commandHandler == null || queryHandler == null || elevatorService == null || elevatorSettings == null || requestSettings == null)
        {
            Log.Error("Failed to initialize services. Please check your configuration");
            throw new InvalidOperationException("Failed to initialize services. Please check your configuration.");
        }

        _ = Task.Run(() => GenerateRandomRequests(commandHandler, elevatorSettings.NumberOfFloors, requestSettings.IntervalInSeconds));
        
        elevatorService.ProcessRequests();
        
        await DisplayElevatorStatus(queryHandler);
    }

    #endregion

    #region Request Generation and Status Display

    /// <summary>
    /// Generates random elevator requests at random intervals.
    /// </summary>
    /// <param name="commandHandler">The command handler to handle the requests.</param>
    /// <param name="numberOfFloors">The total number of floors in the building.</param>
    /// <param name="intervalInSeconds">The interval in seconds between requests.</param>
    private static async Task GenerateRandomRequests(CommandHandler commandHandler, int numberOfFloors, int intervalInSeconds)
    {
        while (true)
        {
            var (floor, direction) = RandomRequestGenerator.GenerateRandomRequest(numberOfFloors);
            
            var addRequestCommand = new AddElevatorRequestCommand(floor, direction);
            
            commandHandler.Handle(addRequestCommand);
            
            await Task.Delay(intervalInSeconds * 1000); // Delay for the specified interval
        }
    }

    /// <summary>
    /// Continuously displays the status of the elevators.
    /// </summary>
    /// <param name="queryHandler">The query handler to get the status.</param>
    private static async Task DisplayElevatorStatus(QueryHandler queryHandler)
    {
        if (queryHandler == null) throw new ArgumentNullException(nameof(queryHandler));

        var previousStatus = string.Empty;

        while (true)
        {
            var getStatusQuery = new GetElevatorStatusQuery();
            var status = queryHandler.Handle(getStatusQuery);
            if (status != previousStatus)
            {
                foreach (var line in status.Split('\n'))
                {
                    Log.Information(line);
                }

                previousStatus = status;
            }

            await Task.Delay(1000);
        }
    }

    #endregion

    #region Exception Handling

    /// <summary>
    /// Logs unhandled exceptions.
    /// </summary>
    /// <param name="ex">The exception to log.</param>
    private static void LogUnhandledException(Exception ex)
    {
        Log.Fatal(ex, "An unhandled exception occurred");
        
        Log.Information("An unexpected error occurred: {ExMessage}", ex.Message);
        
        Environment.Exit(1); // Exit the application
    }

    #endregion
}
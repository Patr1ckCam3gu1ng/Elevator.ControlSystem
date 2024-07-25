using Elevator.ControlSystem.Models;
using Elevator.ControlSystem.Services;
using Microsoft.Extensions.Configuration;

namespace Elevator.ControlSystem.Tests;

[TestClass]
public class ConfigurationTests
{
    private IConfigurationRoot _configuration;
    private ElevatorSettings _elevatorSettings;
    private RequestSettings _requestSettings;

    /// <summary>
    /// Initializes the test setup by loading the configuration from the testsettings.json file.
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("testsettings.json", optional: false, reloadOnChange: true);

        _configuration = configBuilder.Build();
        _elevatorSettings = _configuration.GetSection("ElevatorSettings").Get<ElevatorSettings>() ?? throw new InvalidOperationException();
        _requestSettings = _configuration.GetSection("RequestSettings").Get<RequestSettings>() ?? throw new InvalidOperationException();
    }

    /// <summary>
    /// Tests that the ElevatorSettings are correctly read from the configuration.
    /// </summary>
    [TestMethod]
    public void ElevatorSettings_AreReadCorrectly()
    {
        // Assert: Verify that the settings are correctly loaded
        Assert.AreEqual(3, _elevatorSettings.NumberOfElevators);
        Assert.AreEqual(15, _elevatorSettings.NumberOfFloors);
        Assert.AreEqual(3, _elevatorSettings.MoveTimeSeconds);
        Assert.AreEqual(5, _elevatorSettings.StopTimeSeconds);
    }

    /// <summary>
    /// Tests that the RequestSettings are correctly read from the configuration.
    /// </summary>
    [TestMethod]
    public void RequestSettings_AreReadCorrectly()
    {
        // Assert: Verify that the settings are correctly loaded
        Assert.AreEqual(30, _requestSettings.IntervalInSeconds);
    }

    /// <summary>
    /// Tests that the ElevatorSystem is correctly initialized using the configuration settings.
    /// </summary>
    [TestMethod]
    public void ElevatorSystem_IsInitializedCorrectly()
    {
        // Arrange: Create the elevator system using the factory
        var elevatorSystem = ElevatorSystemFactory.CreateElevatorSystem(_elevatorSettings);

        // Assert: Verify that the elevator system is initialized correctly
        Assert.AreEqual(3, elevatorSystem.Elevators.Count);
        Assert.IsTrue(elevatorSystem.Elevators.All(e => e.CurrentFloor == 1));
    }
}
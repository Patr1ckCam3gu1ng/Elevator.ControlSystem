using Elevator.ControlSystem.Models;
using Elevator.ControlSystem.Services.Interfaces;
using Serilog;

namespace Elevator.ControlSystem.Services;

/// <summary>
/// Service responsible for managing elevator operations.
/// </summary>
public class ElevatorService : IElevatorService
{
    private readonly ElevatorSystemModel _elevatorSystem;
    private readonly ElevatorSettings _settings;
    private readonly IRequestService _requestService;
    private readonly IMovementService _movementService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElevatorService"/> class.
    /// </summary>
    /// <param name="elevatorSystem">The elevator system model.</param>
    /// <param name="settings">The configuration settings for the elevator system.</param>
    /// <param name="requestService">The service for handling requests.</param>
    /// <param name="movementService">The service for handling elevator movements.</param>
    public ElevatorService(
        ElevatorSystemModel elevatorSystem,
        ElevatorSettings settings,
        IRequestService requestService,
        IMovementService movementService)
    {
        _elevatorSystem = elevatorSystem ?? throw new ArgumentNullException(nameof(elevatorSystem));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
        _movementService = movementService ?? throw new ArgumentNullException(nameof(movementService));
    }

    /// <summary>
    /// Adds a request for an elevator to move to a specified floor.
    /// </summary>
    /// <param name="floor">The floor number of the request.</param>
    /// <param name="direction">The direction of the request (up or down).</param>
    public void AddRequest(int floor, string direction)
    {
        _requestService.AddRequest(_elevatorSystem, _settings, floor, direction);
    }

    /// <summary>
    /// Processes elevator requests in a separate thread for each elevator.
    /// </summary>
    public void ProcessRequests()
    {
        for (var i = 0; i < _elevatorSystem.Elevators.Count; i++)
        {
            var elevator = _elevatorSystem.Elevators[i];
            var elevatorIndex = i + 1;
            Task.Run(() =>
            {
                try
                {
                    _movementService.ProcessElevatorRequests(elevator, elevatorIndex);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error processing requests for elevator {ElevatorIndex}", elevatorIndex);
                }
            });
        }
    }
}
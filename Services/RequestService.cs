using Elevator.ControlSystem.Models;
using Elevator.ControlSystem.Services.Interfaces;
using Serilog;

namespace Elevator.ControlSystem.Services;

/// <summary>
/// Provides services for handling elevator requests.
/// </summary>
public class RequestService : IRequestService
{
    /// <summary>
    /// Adds a request to the elevator system.
    /// </summary>
    /// <param name="elevatorSystem">The elevator system model.</param>
    /// <param name="settings">The elevator settings.</param>
    /// <param name="floor">The requested floor.</param>
    /// <param name="direction">The direction (up or down).</param>
    public void AddRequest(ElevatorSystemModel elevatorSystem, ElevatorSettings settings, int floor, string direction)
    {
        try
        {
            // Validate request direction for first and last floors
            if ((floor == 1 && direction is Constants.Down or Constants.Up) || (floor == settings.NumberOfFloors && direction == Constants.Up))
            {
                Log.Information("Ignoring invalid request: {Direction} request on floor {Floor}", direction.ToUpper(), floor);

                return;
            }

            // Check if there are any elevators available
            if (elevatorSystem.Elevators.Count == 0)
            {
                Log.Error("No elevators are available in the system");

                throw new InvalidOperationException("No elevators are available in the system.");
            }

            // Find the nearest available elevator
            var nearestElevator = elevatorSystem.Elevators
                .OrderBy(e => Math.Abs(e.CurrentFloor - floor))
                .ThenBy(e => e.RequestedFloors.Count != 0 ? e.RequestedFloors.Min(f => Math.Abs(f - floor)) : 0)
                .FirstOrDefault();

            // Validate nearest elevator
            if (nearestElevator == null)
            {
                Log.Error("Failed to find the nearest elevator");
                throw new InvalidOperationException("Failed to find the nearest elevator.");
            }

            // Add the request to the nearest elevator
            if (!nearestElevator.RequestedFloors.Contains(floor))
            {
                nearestElevator.RequestedFloors.Add(floor);
                Log.Information("{Direction} request on floor {Floor} received", direction.ToUpper(), floor);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error adding request");
        }
    }
}
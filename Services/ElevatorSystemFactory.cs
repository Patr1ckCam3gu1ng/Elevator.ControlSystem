using Elevator.ControlSystem.Models;
using Serilog;

namespace Elevator.ControlSystem.Services;

/// <summary>
/// Factory to create an elevator system.
/// </summary>
public static class ElevatorSystemFactory
{
    /// <summary>
    /// Creates an elevator system with the specified settings.
    /// </summary>
    /// <param name="settings">The elevator settings.</param>
    /// <returns>An instance of ElevatorSystemModel.</returns>
    public static ElevatorSystemModel CreateElevatorSystem(ElevatorSettings settings)
    {
        var elevatorSystem = new ElevatorSystemModel();
        for (var i = 0; i < settings.NumberOfElevators; i++)
        {
            var elevator = new ElevatorModel
            {
                CurrentFloor = 1 // Assuming elevators start at floor 1, modify as needed
            };
            elevatorSystem.Elevators.Add(elevator);
            Log.Information("Elevator {ElevatorNumber} initialized at floor {CurrentFloor}", i + 1, elevator.CurrentFloor);
        }

        Log.Information("Elevator system initialized with {NumberOfElevators} elevators in total", settings.NumberOfElevators);
        Log.Information("");

        return elevatorSystem;
    }
}
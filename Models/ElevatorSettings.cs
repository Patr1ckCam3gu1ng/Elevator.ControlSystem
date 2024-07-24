namespace Elevator.ControlSystem.Models;

/// <summary>
/// Configuration settings for the elevator system.
/// </summary>
public class ElevatorSettings
{
    /// <summary>
    /// Gets or sets the number of elevators in the system.
    /// </summary>
    public int NumberOfElevators { get; set; }

    /// <summary>
    /// Gets or sets the number of floors in the building.
    /// </summary>
    public int NumberOfFloors { get; set; }

    /// <summary>
    /// Gets or sets the time in seconds for an elevator to move between floors.
    /// </summary>
    public int MoveTimeSeconds { get; set; }

    /// <summary>
    /// Gets or sets the time in seconds for passengers to enter or leave the elevator.
    /// </summary>
    public int StopTimeSeconds { get; set; }
}
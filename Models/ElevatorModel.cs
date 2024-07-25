namespace Elevator.ControlSystem.Models;

/// <summary>
/// Represents an elevator within the building.
/// </summary>
public class ElevatorModel
{
    /// <summary>
    /// Gets or sets the current floor of the elevator.
    /// </summary>
    public int CurrentFloor { get; set; } = 1;

    /// <summary>
    /// Gets the list of requested floors.
    /// </summary>
    public List<int> RequestedFloors { get; set; } = new List<int>();

    /// <summary>
    /// Gets or sets the current state of the elevator.
    /// </summary>
    public string State { get; set; } = Constants.Stopped;
}
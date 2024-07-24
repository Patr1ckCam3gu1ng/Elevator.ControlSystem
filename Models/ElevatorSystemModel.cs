namespace Elevator.ControlSystem.Models;

/// <summary>
/// Represents the elevator system in the building.
/// </summary>
public class ElevatorSystemModel
{
    /// <summary>
    /// Gets the list of elevators in the system.
    /// </summary>
    public List<ElevatorModel> Elevators { get; set; }

    public ElevatorSystemModel()
    {
        Elevators = new List<ElevatorModel>();
    }
}
namespace Elevator.ControlSystem.Commands;

/// <summary>
/// Command to add a request for an elevator.
/// </summary>
public class AddElevatorRequestCommand(int floor, string direction) : ICommand
{
    public int Floor { get; } = floor;
    public string Direction { get; } = direction;
}
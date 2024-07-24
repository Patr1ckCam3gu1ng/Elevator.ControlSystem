using Elevator.ControlSystem.Models;

namespace Elevator.ControlSystem.Services.Interfaces;

public interface IRequestService
{
    void AddRequest(ElevatorSystemModel elevatorSystem, ElevatorSettings settings, int floor, string direction);
}
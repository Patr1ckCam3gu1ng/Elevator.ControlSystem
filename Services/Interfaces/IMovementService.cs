using Elevator.ControlSystem.Models;

namespace Elevator.ControlSystem.Services.Interfaces;

public interface IMovementService
{
    Task ProcessElevatorRequests(ElevatorModel elevator, int elevatorIndex);
}
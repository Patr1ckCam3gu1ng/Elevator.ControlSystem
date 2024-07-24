namespace Elevator.ControlSystem.Services.Interfaces;

public interface IElevatorService
{
    void AddRequest(int floor, string direction);
    
    void ProcessRequests();
}
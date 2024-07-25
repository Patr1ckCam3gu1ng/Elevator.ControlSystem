using Elevator.ControlSystem.Models;
using Elevator.ControlSystem.Services.Interfaces;
using Serilog;

namespace Elevator.ControlSystem.Services;

public class MovementService : IMovementService
{
    private readonly ElevatorSettings _settings;

    public MovementService(ElevatorSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task ProcessElevatorRequests(ElevatorModel elevator, int elevatorIndex)
    {
        var previousFloor = -1;

        while (true)
        {
            try
            {
                if (elevator.RequestedFloors.Count != 0)
                {
                    if (elevator.State is "MovingUp" or "Stopped")
                    {
                        var targetFloors = elevator.RequestedFloors
                            .Where(floor => floor > elevator.CurrentFloor)
                            .OrderBy(floor => floor)
                            .ToList();

                        if (targetFloors.Count != 0)
                        {
                            await ProcessFloors(elevator, targetFloors, "up", elevatorIndex);
                        }
                        else
                        {
                            // Check for any lower floors if no higher floors are pending
                            var lowerFloors = elevator.RequestedFloors
                                .OrderBy(floor => floor)
                                .ToList();
                            await ProcessFloors(elevator, lowerFloors, "down", elevatorIndex);
                        }
                    }
                    else if (elevator.State == "MovingDown")
                    {
                        var targetFloors = elevator.RequestedFloors
                            .Where(floor => floor < elevator.CurrentFloor)
                            .OrderByDescending(floor => floor)
                            .ToList();

                        if (targetFloors.Count != 0)
                        {
                            await ProcessFloors(elevator, targetFloors, "down", elevatorIndex);
                        }
                        else
                        {
                            // Check for any higher floors if no lower floors are pending
                            var higherFloors = elevator.RequestedFloors
                                .OrderByDescending(floor => floor)
                                .ToList();
                            await ProcessFloors(elevator, higherFloors, "up", elevatorIndex);
                        }
                    }
                }
                else
                {
                    await Task.Delay(1000); // Wait before checking again
                }

                // Log the current position of the elevator only if it has changed
                if (elevator.CurrentFloor != previousFloor)
                {
                    Log.Information("Car {ElevatorIndex} is on floor {CurrentFloor}", elevatorIndex, elevator.CurrentFloor);
                    previousFloor = elevator.CurrentFloor;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing elevator requests for elevator {ElevatorIndex}", elevatorIndex);
            }
        }
    }

    private async Task ProcessFloors(ElevatorModel elevator, List<int> targetFloors, string direction, int elevatorIndex)
    {
        foreach (var floor in targetFloors)
        {
            await MoveToFloor(elevator, floor, direction, elevatorIndex);
            
            Stop(elevator, elevatorIndex);
            
            Log.Information("Elevator {ElevatorIndex} stopped at floor {Floor} for passengers to enter/leave", elevatorIndex, elevator.CurrentFloor);
            
            await Task.Delay(_settings.StopTimeSeconds * 1000); // Simulate time for passengers to enter/leave
        }

        // Remove processed floors from the request list
        elevator.RequestedFloors.RemoveAll(targetFloors.Contains);

        // Change the state of the elevator if no more requests in the current direction
        if (!elevator.RequestedFloors.Any(floor => direction == "up" ? floor > elevator.CurrentFloor : floor < elevator.CurrentFloor))
        {
            elevator.State = "Stopped";
        }
    }

    private async Task MoveToFloor(ElevatorModel elevator, int targetFloor, string direction, int elevatorIndex)
    {
        try
        {
            while (elevator.CurrentFloor != targetFloor)
            {
                if (direction == "up")
                {
                    if (elevator.CurrentFloor + 1 > _settings.NumberOfFloors)
                    {
                        Log.Error("Elevator {ElevatorIndex} cannot move above floor {NumberOfFloors}", elevatorIndex, _settings.NumberOfFloors);
                        return;
                    }

                    MoveUp(elevator, elevatorIndex);
                    elevator.CurrentFloor++;
                }
                else
                {
                    if (elevator.CurrentFloor - 1 < 1)
                    {
                        Log.Error("Elevator {ElevatorIndex} cannot move below floor 1", elevatorIndex);
                        return;
                    }

                    MoveDown(elevator, elevatorIndex);
                    elevator.CurrentFloor--;
                }

                Log.Information("Elevator {ElevatorIndex} is on floor {Floor}", elevatorIndex, elevator.CurrentFloor);
                await Task.Delay(_settings.MoveTimeSeconds * 1000); // Simulate time to move a floor
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error moving elevator {ElevatorIndex} to floor {TargetFloor}", elevatorIndex, targetFloor);
        }
    }

    private void MoveUp(ElevatorModel elevator, int elevatorIndex)
    {
        try
        {
            if (elevator.State != "MovingUp")
            {
                Log.Information("Elevator {ElevatorIndex} starting to move up", elevatorIndex);
                elevator.State = "MovingUp";
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error setting elevator {ElevatorIndex} state to MovingUp", elevatorIndex);
        }
    }

    private void MoveDown(ElevatorModel elevator, int elevatorIndex)
    {
        try
        {
            if (elevator.State != "MovingDown")
            {
                Log.Information("Elevator {ElevatorIndex} starting to move down", elevatorIndex);
                elevator.State = "MovingDown";
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error setting elevator {ElevatorIndex} state to MovingDown", elevatorIndex);
        }
    }

    private void Stop(ElevatorModel elevator, int elevatorIndex)
    {
        try
        {
            if (elevator.State != "Stopped")
            {
                Log.Information("Elevator {ElevatorIndex} stopping", elevatorIndex);
                elevator.State = "Stopped";
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error setting elevator {ElevatorIndex} state to Stopped", elevatorIndex);
        }
    }
}
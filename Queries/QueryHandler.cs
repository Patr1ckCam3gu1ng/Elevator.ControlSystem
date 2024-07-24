using System.Text;
using Elevator.ControlSystem.Models;
using Serilog;

namespace Elevator.ControlSystem.Queries;

/// <summary>
/// Handles queries for the elevator system.
/// </summary>
public class QueryHandler
{
    private readonly ElevatorSystemModel _elevatorSystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryHandler"/> class.
    /// </summary>
    /// <param name="elevatorSystem">The elevator system model.</param>
    public QueryHandler(ElevatorSystemModel elevatorSystem)
    {
        _elevatorSystem = elevatorSystem ?? throw new ArgumentNullException(nameof(elevatorSystem));
    }

    /// <summary>
    /// Handles the given query and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="query">The query to handle.</param>
    /// <returns>The result of the query.</returns>
    public TResult Handle<TResult>(IQuery<TResult> query)
    {
        try
        {
            return (TResult)(object)GetElevatorStatus();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error handling query");
        }

        throw new InvalidOperationException();
    }

    /// <summary>
    /// Gets the status of the elevators.
    /// </summary>
    /// <returns>A string representing the status of the elevators.</returns>
    private string GetElevatorStatus()
    {
        try
        {
            var statusBuilder = new StringBuilder();
            var elevators = _elevatorSystem.Elevators;

            statusBuilder.AppendLine("-----------------");

            for (var i = 0; i < elevators.Count; i++)
            {
                var elevator = elevators[i];
                statusBuilder.AppendLine($"Car {i + 1} is on floor {elevator.CurrentFloor}");
            }

            statusBuilder.AppendLine("-----------------");
            
            return statusBuilder.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting elevator status");
            return "Error retrieving status.";
        }
    }
}
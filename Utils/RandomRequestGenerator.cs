namespace Elevator.ControlSystem.Utils;

/// <summary>
/// Utility class to generate random elevator requests.
/// </summary>
public static class RandomRequestGenerator
{
    private static readonly Random Random = new Random();

    /// <summary>
    /// Generates a random request for an elevator.
    /// </summary>
    /// <returns>A tuple containing the floor number and the direction.</returns>
    public static (int floor, string direction) GenerateRandomRequest(int numberOfFloors)
    {
        var random = new Random();

        var floor = random.Next(1, numberOfFloors + 1);

        var direction = random.Next(0, 2) == 0 ? Constants.Up : Constants.Down;

        return (floor, direction);
    }
}
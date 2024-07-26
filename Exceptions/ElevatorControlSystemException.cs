namespace Elevator.ControlSystem.Exceptions
{
    /// <summary>
    /// Represents errors that occur during elevator operations.
    /// </summary>
    public class ElevatorControlSystemException : ApplicationException
    {
        public ElevatorControlSystemException() { }

        public ElevatorControlSystemException(string message) : base(message) { }
    }
}
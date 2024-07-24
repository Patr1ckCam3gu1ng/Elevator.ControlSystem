using Elevator.ControlSystem.Services.Interfaces;

namespace Elevator.ControlSystem.Commands
{
    /// <summary>
    /// Handles commands for the elevator system.
    /// </summary>
    public class CommandHandler(IElevatorService elevatorService)
    {
        /// <summary>
        /// Handles the given command.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        public void Handle(ICommand command)
        {
            try
            {
                switch (command)
                {
                    case AddElevatorRequestCommand addRequestCommand:
                        elevatorService.AddRequest(addRequestCommand.Floor, addRequestCommand.Direction);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling command: {ex.Message}");
            }
        }
    }
}
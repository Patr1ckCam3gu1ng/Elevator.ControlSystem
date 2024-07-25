using Elevator.ControlSystem.Models;
using Elevator.ControlSystem.Services;
using Serilog;

namespace Elevator.ControlSystem.Tests
{
    [TestClass]
    public class RequestServiceTests
    {
        private RequestService? _requestService; // Marking as nullable
        private ElevatorSystemModel? _elevatorSystem; // Marking as nullable
        private ElevatorSettings? _elevatorSettings; // Marking as nullable

        /// <summary>
        /// Initializes the test setup by creating instances of RequestService, ElevatorSystemModel, and ElevatorSettings.
        /// Configures Serilog for logging during tests.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _requestService = new RequestService();
            _elevatorSystem = new ElevatorSystemModel
            {
                Elevators =
                [
                    new ElevatorModel { CurrentFloor = 1 },
                    new ElevatorModel { CurrentFloor = 5 }
                ]
            };
            _elevatorSettings = new ElevatorSettings
            {
                NumberOfFloors = 10
            };

            // Setup Serilog for tests
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        }

        /// <summary>
        /// Tests that a valid request adds a request to the nearest available elevator.
        /// </summary>
        [TestMethod]
        public void AddRequest_ValidRequest_AddsRequestToNearestElevator()
        {
            // Arrange: Create a valid request (up from the third floor)
            const int floor = 3;
            const string direction = Constants.Up;

            // Act: Add the valid request
            _requestService?.AddRequest(_elevatorSystem!, _elevatorSettings!, floor, direction);

            // Assert: Verify that the request was added to the nearest elevator
            Assert.IsTrue(_elevatorSystem?.Elevators.First(e => e.CurrentFloor == 1).RequestedFloors.Contains(floor) ?? false);
        }

        /// <summary>
        /// Tests that requests to move up from the top floor and down from the bottom floor are ignored.
        /// </summary>
        [TestMethod]
        public void AddRequest_HandlesRequestForTopAndBottomFloorsCorrectly()
        {
            // Arrange: Define the top and bottom floors
            var topFloor = _elevatorSettings!.NumberOfFloors;
            const int bottomFloor = 1;

            // Act: Attempt to add invalid requests
            _requestService?.AddRequest(_elevatorSystem!, _elevatorSettings!, topFloor, Constants.Up);
            _requestService?.AddRequest(_elevatorSystem!, _elevatorSettings!, bottomFloor, Constants.Down);

            // Assert: Verify that the invalid requests were not added
            Assert.IsFalse(_elevatorSystem?.Elevators.Any(e => e.RequestedFloors.Contains(topFloor)) ?? true);
            Assert.IsFalse(_elevatorSystem?.Elevators.Any(e => e.RequestedFloors.Contains(bottomFloor)) ?? true);
        }

        /// <summary>
        /// Tests that multiple valid requests are added to the nearest available elevators.
        /// </summary>
        [TestMethod]
        public void AddRequest_MultipleValidRequests_AddsToNearestElevators()
        {
            // Arrange: Create multiple valid requests
            var requests = new[]
            {
                new { Floor = 2, Direction = Constants.Up },
                new { Floor = 4, Direction = Constants.Down },
                new { Floor = 7, Direction = Constants.Up }
            };

            // Act: Add the valid requests
            foreach (var request in requests)
            {
                _requestService?.AddRequest(_elevatorSystem!, _elevatorSettings!, request.Floor, request.Direction);
            }

            // Assert: Verify that each request was added to the nearest elevator
            Assert.IsTrue(_elevatorSystem?.Elevators.First(e => e.CurrentFloor == 1).RequestedFloors.Contains(2) ?? false);
            Assert.IsTrue(_elevatorSystem?.Elevators.First(e => e.CurrentFloor == 5).RequestedFloors.Contains(4) ?? false);
            Assert.IsTrue(_elevatorSystem?.Elevators.First(e => e.CurrentFloor == 5).RequestedFloors.Contains(7) ?? false);
        }
    }
}
using Elevator.ControlSystem.Models;
using Elevator.ControlSystem.Services;
using Serilog;

namespace Elevator.ControlSystem.Tests
{
    [TestClass]
    public class RequestServiceTests
    {
        private RequestService _requestService;
        private ElevatorSystemModel _elevatorSystem;
        private ElevatorSettings _elevatorSettings;

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
        /// Tests that an invalid request (e.g., down from the first floor) does not add a request to any elevator.
        /// </summary>
        [TestMethod]
        public void AddRequest_InvalidRequest_DoesNotAddRequest()
        {
            // Arrange: Create an invalid request (down from the first floor)
            const int floor = 1;
            const string direction = "down";

            // Act: Attempt to add the invalid request
            _requestService.AddRequest(_elevatorSystem, _elevatorSettings, floor, direction);

            // Assert: Verify that the request was not added to any elevator
            Assert.IsFalse(_elevatorSystem.Elevators.Any(e => e.RequestedFloors.Contains(floor)));
        }

        /// <summary>
        /// Tests that a valid request adds a request to the nearest available elevator.
        /// </summary>
        [TestMethod]
        public void AddRequest_ValidRequest_AddsRequestToNearestElevator()
        {
            // Arrange: Create a valid request (up from the third floor)
            const int floor = 3;
            const string direction = "up";

            // Act: Add the valid request
            _requestService.AddRequest(_elevatorSystem, _elevatorSettings, floor, direction);

            // Assert: Verify that the request was added to the nearest elevator
            Assert.IsTrue(_elevatorSystem.Elevators.First(e => e.CurrentFloor == 1).RequestedFloors.Contains(floor));
        }

        /// <summary>
        /// Tests that requests to move up from the top floor and down from the bottom floor are ignored.
        /// </summary>
        [TestMethod]
        public void AddRequest_HandlesRequestForTopAndBottomFloorsCorrectly()
        {
            // Arrange: Define the top and bottom floors
            var topFloor = _elevatorSettings.NumberOfFloors;
            const int bottomFloor = 1;

            // Act: Attempt to add invalid requests
            _requestService.AddRequest(_elevatorSystem, _elevatorSettings, topFloor, "up");
            _requestService.AddRequest(_elevatorSystem, _elevatorSettings, bottomFloor, "down");

            // Assert: Verify that the invalid requests were not added
            Assert.IsFalse(_elevatorSystem.Elevators.Any(e => e.RequestedFloors.Contains(topFloor)));
            Assert.IsFalse(_elevatorSystem.Elevators.Any(e => e.RequestedFloors.Contains(bottomFloor)));
        }
    }
}
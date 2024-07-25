# Elevator Control System

## Overview

This project implements an elementary elevator control system for a building with the following pre-defined parameters:
- The building has 10 floors.
- There are four elevators.
- It takes 10 seconds for an elevator car to move from one floor to the next.
- When a car stops on a floor, it takes 10 seconds for passengers to enter/leave and then the car is ready to move again.

---
## Features
- CQRS Design Pattern: Separates the responsibility of commands and queries, ensuring clear segregation of concerns.
- Serilog Logging: Provides detailed logging information for monitoring and debugging.
- Generates random calls for the elevator on floors throughout the building.
- Moves elevators to pick up passengers and disembark them.
- Ensures an "up" elevator keeps going up until it has no more passengers and the same for a "down" elevator.
- Logs the relative position of the elevator cars and user actions.
---

## Prerequisites

- .NET 5.0 SDK or later
- Visual Studio 2019 or later (recommended)
---

## Configuration

The configuration settings are located in `appsettings.json`.

```json
{
  "ElevatorSettings": {
    "NumberOfElevators": 4,
    "NumberOfFloors": 10,
    "MoveTimeSeconds": 10,
    "StopTimeSeconds": 10
  },
  "RequestSettings": {
    "IntervalInSeconds": 60
  }
}
```
---

## Design Patterns Used

### CQRS (Command Query Responsibility Segregation)

The CQRS pattern is used to separate the responsibility of commands and queries. Commands are used to change the state of the application, while queries are used to retrieve data. This separation allows for more scalable and maintainable code.

- **Commands**:
    - Handle state-changing operations.
    - Example: `AddElevatorRequestCommand`.
- **Queries**:
    - Handle data retrieval operations.
    - Example: `GetElevatorStatusQuery`.

### Singleton Pattern

The Singleton pattern ensures that a class has only one instance and provides a global point of access to it. This is used for configuration settings and services.

- **Service Registration**: Services are registered as singletons in the `ConfigureServices` method in `Program.cs`.

---

## Unit Tests

The project includes unit tests to ensure the functionality of key components. The tests are implemented using **MSTest**.

### Test Cases

RequestServiceTests

- **AddRequest_ValidRequest_AddsRequestToNearestElevator**:
  - Tests that a valid request adds a request to the nearest available elevator.

- **AddRequest_HandlesRequestForTopAndBottomFloorsCorrectly**:
  - Tests that requests to move up from the top floor and down from the bottom floor are ignored.

- **AddRequest_MultipleValidRequests_AddsToNearestElevators**:
  - Tests that multiple valid requests are added to the nearest available elevators.

ConfigurationTests

- **ElevatorSettings_AreReadCorrectly**:
  - Tests that the `ElevatorSettings` are correctly read from the configuration.

- **RequestSettings_AreReadCorrectly**:
  - Tests that the `RequestSettings` are correctly read from the configuration.

- **ElevatorSystem_IsInitializedCorrectly**:
  - Tests that the elevator system is correctly initialized using the configuration settings.



---
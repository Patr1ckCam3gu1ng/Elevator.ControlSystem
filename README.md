# Elevator Control System

## Overview

This project implements an elementary elevator control system for a building with the following parameters:
- The building has 10 floors.
- There are four elevators.
- It takes 10 seconds for an elevator car to move from one floor to the next.
- When a car stops on a floor, it takes 10 seconds for passengers to enter/leave and then the car is ready to move again.

## Features

- Generates random calls for the elevator on floors throughout the building.
- Moves elevators to pick up passengers and disembark them.
- Ensures an "up" elevator keeps going up until it has no more passengers and the same for a "down" elevator.
- Logs the relative position of the elevator cars and user actions.

## Configuration

The configuration settings are located in `appsettings.json`.

```json
{
  "ElevatorSettings": {
    "NumberOfElevators": 4,
    "NumberOfFloors": 10,
    "MoveTimeSeconds": 10,
    "StopTimeSeconds": 10
  }
}

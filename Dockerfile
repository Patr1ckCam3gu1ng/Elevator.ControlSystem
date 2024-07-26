# Use the .NET 8.0 SDK to build and run tests
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project files and restore dependencies for both the main and test projects
COPY Elevator.ControlSystem.csproj .
COPY Elevator.ControlSystem.Tests/Elevator.ControlSystem.Tests.csproj Elevator.ControlSystem.Tests/
RUN dotnet restore Elevator.ControlSystem.csproj

# Copy the rest of the source code and build the app
COPY . .
RUN dotnet build Elevator.ControlSystem.csproj -c Release

# Build and run tests
RUN dotnet build Elevator.ControlSystem.Tests/Elevator.ControlSystem.Tests.csproj -c Release
RUN dotnet test Elevator.ControlSystem.Tests/Elevator.ControlSystem.Tests.csproj --logger:trx

# Publish the app
RUN dotnet publish Elevator.ControlSystem.csproj -c Release -o /app/publish

# Use the runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Elevator.ControlSystem.dll"]

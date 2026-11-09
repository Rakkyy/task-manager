# Task Manager

Simple WPF desktop app to add tasks, mark them as completed and filter the list
(All / Completed / Not Completed). Built with .NET 10 and the MVVM pattern.

## Run

Requires Windows and the .NET 10 SDK.

    dotnet run --project src/TaskManager.App

## Tests

    dotnet test

## Projects

- `TaskManager.Core` models, view models and the command class (no WPF dependency)
- `TaskManager.App` WPF window (XAML only)
- `TaskManager.Tests` xUnit tests for the view model

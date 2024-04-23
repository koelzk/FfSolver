# FfSolver

This is a solver for the solitaire-based card game Fortune’s Foundation, which is part of the [Zachtronics Solitaire Collection](https://www.zachtronics.com/solitaire-collection/) made by [Zachtronics](https://www.zachtronics.com/).

## How To Build

.NET 8 is required. Go to the top-level directory and build with `dotnet build`.

## How To Run

Run the solver application from the top-level directory with `dotnet run --project src/SolverApp/SolverApp.csproj`.

The app will extract the board state from a screenshot of the game, calculate a solution for the board and display the card moves:
To solve a custom game, replace the image file `src/SolverApp/image.png` with a screenshot of a new game captured in 1920x1080 resolution.

## Solver

The solver uses [A*](https://en.wikipedia.org/wiki/A*_search_algorithm) as pathfinding strategy and will produce solutions with 60 t0 70 moves. There's a unit test for playing around with the solver located in [src/Solver.Tests/SolverTests.cs](src/Solver.Tests/SolverTests.cs).


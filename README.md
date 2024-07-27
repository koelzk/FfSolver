# FfSolver

![UI](wiki/ui.png)

This is a solver for the solitaire-based card game Fortune’s Foundation, which is part of the [Zachtronics Solitaire Collection](https://www.zachtronics.com/solitaire-collection/) made by [Zachtronics](https://www.zachtronics.com/).

It can solve start and mid-game board states and also features detecting board states from screenshots. The solver can be either run as UI application and or as a console application.

## How To Build

.NET 8 is required. Go to the top-level directory and build with `dotnet build`.

## Run as UI Application

Start the application with `dotnet run --project src/SolverAvn/SolverAvn.csproj`. 

1. Afterwards, you can load a screenshot by clicking *Import Screenshot*. The image must be a screenshot of the game's board with the game resolution set to 1920x1080 ([example file](src/SolverApp/image.png)).

1. If board detection was successful, you should see the detected cards highlighted in the displayed screenshot image.

1. Click on *Solve* to start the solver. The solver will try to find a solution within 100,000 iterations. After finding an initial solution, it will look for solutions with fewer moves. Click on *Stop* to abort solving and use the latest solution found.

## Run as Console Application

<pre>
<font color="#3465A4">5♠  </font> <font color="#4E9A06">J♣  </font> <font color="#C4A000">3♦  </font> <font color="#C4A000">9♦  </font> <font color="#808080">8   </font>  --  <font color="#CC0000">10♥ </font> <font color="#3465A4">Q♠  </font> <font color="#4E9A06">4♣  </font> <font color="#4E9A06">5♣  </font> <font color="#808080">0   </font>
<font color="#4E9A06">7♣  </font> <font color="#CC0000">2♥  </font> <font color="#CC0000">K♥  </font> <font color="#3465A4">7♠  </font> <font color="#3465A4">10♠ </font>  --  <font color="#3465A4">J♠  </font> <font color="#C4A000">10♦ </font> <font color="#808080">20  </font> <font color="#808080">21  </font> <font color="#808080">1   </font>
<font color="#CC0000">4♥  </font> <font color="#808080">5   </font> <font color="#C4A000">6♦  </font> <font color="#4E9A06">Q♣  </font> <font color="#4E9A06">10♣ </font>  --  <font color="#3465A4">K♠  </font> <font color="#CC0000">8♥  </font> <font color="#3465A4">2♠  </font> <font color="#3465A4">8♠  </font> <font color="#4E9A06">8♣  </font>
<font color="#CC0000">7♥  </font> <font color="#C4A000">K♦  </font> <font color="#4E9A06">6♣  </font> <font color="#808080">11  </font> <font color="#CC0000">J♥  </font>  --  <font color="#4E9A06">K♣  </font> <font color="#808080">16  </font> <font color="#CC0000">5♥  </font> <font color="#3465A4">3♠  </font> <font color="#808080">10  </font>
<font color="#808080">9   </font> <font color="#C4A000">7♦  </font> <font color="#C4A000">5♦  </font> <font color="#4E9A06">3♣  </font> <font color="#C4A000">J♦  </font>  --  <font color="#808080">7   </font> <font color="#808080">13  </font> <font color="#C4A000">4♦  </font> <font color="#C4A000">2♦  </font> <font color="#808080">2   </font>
<font color="#CC0000">6♥  </font> <font color="#808080">14  </font> <font color="#808080">3   </font> <font color="#4E9A06">2♣  </font> <font color="#C4A000">Q♦  </font>  --  <font color="#CC0000">9♥  </font> <font color="#4E9A06">9♣  </font> <font color="#3465A4">4♠  </font> <font color="#3465A4">6♠  </font> <font color="#808080">19  </font>
<font color="#808080">15  </font> <font color="#3465A4">9♠  </font> <font color="#808080">17  </font> <font color="#CC0000">3♥  </font> <font color="#808080">18  </font>  --  <font color="#808080">4   </font> <font color="#808080">12  </font> <font color="#808080">6   </font> <font color="#CC0000">Q♥  </font> <font color="#C4A000">8♦  </font>
</pre>

Run the solver with the board state read from an image using the command
`dotnet run --project src/SolverApp/SolverApp.csproj -- -i src/SolverApp/image.png`.
The image must be a screenshot of the game's board with the game resolution set to 1920x1080 ([example file](src/SolverApp/image.png)).

You can also run the solver with the board state read from a text file using the command
`dotnet run --project src/SolverApp/SolverApp.csproj -- -i src/SolverApp/example.txt -t` ([example file](src/SolverApp/example.txt)).

If a solution is found, all moves will be printed out with a pretty-print of the board state after each move. Moves automatically performed by the game are omitted.

The solver uses [A*](https://en.wikipedia.org/wiki/A*_search_algorithm) as pathfinding strategy and will typically produce solutions with 60 to 70 moves. There's a unit test for playing around with the solver located in [src/Solver.Tests/SolverTests.cs](src/Solver.Tests/SolverTests.cs).
There are additional options to customize the search for solutions. See the full list of arguments:

| Short | Long       | Description                                   | Default |
|-------|------------|-----------------------------------------------|---------|
| -i    | --input    | Required. Path to input file                  | -       |
| -t    | --text     | Read text instead of image from input file    | false   |
| -m    | --max-iter | Maximum number of iterations                  | 100000  |
| -s    | --steps    | Maximum number of moves a solution may have   | 80      |
| -f    | --full     | Evaluate all iterations to find best solution | false   |

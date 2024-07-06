using System.Diagnostics;

namespace FfSolver;


public class Solver
{
    private readonly Dictionary<Board, BoardNode> visitedNodes = [];
    private readonly PriorityQueue<BoardNode, int> queue = new();

    Board start;

    public Solver(Board start)
    {
        this.start = start;
    }

    public SolveResult Solve(int maxIterations = 500_000, int maxSteps = 100, bool returnOnSolve = true)
    {
        var board = new Board(start);
        board.ApplyAutoMoves();

        var startNode = new BoardNode(board, null, null, 0, 0);
        queue.Enqueue(startNode, 0);

        var currentMaxSteps = maxSteps;
        var solutionNode = default(BoardNode?);
        int iteration;

        for (iteration = 0; iteration < maxIterations; iteration++)
        {
            if (queue.Count == 0)
            {
                return new SolveResult(SolveResultStatus.NoSolution, iteration);
            }

            var currentNode = queue.Dequeue();
            var current = currentNode.Board;

            if (current.IsGameWon)
            {
                solutionNode = solutionNode == null || solutionNode.Step > currentNode.Step
                    ? currentNode
                    : solutionNode;
                currentMaxSteps = solutionNode.Step - 1;

                if (returnOnSolve)
                {
                    break;
                }
            }   

            var moves = current.EnumerateMoves().ToList();

            foreach (var move in moves)
            {
                AddNode(currentNode, move, currentMaxSteps);
            }
        }

        return solutionNode is null
            ? new SolveResult(SolveResultStatus.ReachedMaxIterations, iteration)
            : new SolveResult(SolveResultStatus.Solved, iteration, AssembleMoves(solutionNode.Board));
    }

    private void AddNode(BoardNode currentNode, Move move, int maxSteps)
    {
        Board next = new Board(currentNode.Board);
        next.ApplyMove(move);
        next.Normalize();
        var step = currentNode.Step + 1;

        var oldNode = visitedNodes.TryGetValue(next, out var on) ? on : null;

        if ((oldNode == null && step <= maxSteps) || (oldNode != null && step < oldNode.Step))
        {
            var nextNode = new BoardNode(next, currentNode.Board, move, step, next.GetScore(step));

            if (oldNode == null)
            {
                visitedNodes.Add(next, nextNode);
                queue.Enqueue(nextNode, -nextNode.Score);
            }
            else
            {
                visitedNodes[next] = nextNode;
            }
        }
    }

    private List<Move> AssembleMoves(Board end)
    {
        var nodeStack = new Stack<BoardNode>();

        var b = end;
        while (visitedNodes.TryGetValue(b, out var node) && node.Previous != null)
        {
            nodeStack.Push(node);
            b = node.Previous;
        }

        var norm = new BoardNormalization();
        var moves = new List<Move>();

        while (nodeStack.TryPop(out var node))
        {
            Debug.Assert(node?.Move != null);
            moves.Add(norm.Translate(node.Move.Value));

            Debug.Assert(node?.Previous != null);
            Board currentNoNorm = new Board(node.Previous);
            currentNoNorm.ApplyMove(node.Move.Value);
            currentNoNorm.ApplyAutoMoves();

            norm.Advance(currentNoNorm);
        }

        return moves;
    }    
}

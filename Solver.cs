

using System.Diagnostics;

namespace FfSolver;


public class Solver
{
    private readonly Dictionary<Board, BoardNode> visitedNodes = new Dictionary<Board, BoardNode>();
    private readonly PriorityQueue<BoardNode, int> queue = new PriorityQueue<BoardNode, int>();

    Board start;

    public Solver(Board start)
    {
        this.start = start;
    }

    public SolveResult Solve(int maxIterations = 500_000, int maxSteps = 100)
    {
        var board = new Board(start);
        board.ApplyAutoMoves();
        var startNode = new BoardNode(board, null, null, 0, 0);
        queue.Enqueue(startNode, 0);

        for (var iteration = 0; iteration < maxIterations; iteration++)
        {
            if (queue.Count == 0)
            {
                return new SolveResult(SolveResultStatus.NoSolution);
            }

            var currentNode = queue.Dequeue();
            var current = currentNode.Board;

#if DEBUG_OUTPUT
            if (iteration % 1000 == 0)
            {
                Console.WriteLine($"{iteration}\n========================");
                Console.WriteLine(current);
            }
#endif

            if (current.IsGameWon)
            {
                return new SolveResult(SolveResultStatus.Solved, AssembleMoves(current));
            }   

            var moves = current.EnumerateMoves().ToList();

            foreach (var move in moves)
            {
                AddNode(currentNode, move, maxSteps);
            }
        }

        return new SolveResult(SolveResultStatus.ReachedMaxIterations);
    }

    private void AddNode(BoardNode currentNode, Move move, int maxSteps)
    {
        Board next = new Board(currentNode.Board);
        next.ApplyMove(move);
        next.Normalize();

        if (!visitedNodes.ContainsKey(next) && currentNode.Step < maxSteps)
        {
            var nextNode = new BoardNode(next, currentNode.Board, move, currentNode.Step + 1, next.GetScore(currentNode.Step + 1));
            visitedNodes.Add(next, nextNode);
            queue.Enqueue(nextNode, -nextNode.Score);
        }
        else
        {
            //TODO: Check for shortcuts
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

    public class BoardNode
    {
        public BoardNode(Board board, Board? previous, Move? move, int step, int score)
        {
            Board = board;
            Previous = previous;
            Move = move;
            Step = step;
            Score = score;
        }

        public Board Board { get; }
        public Board? Previous { get; }
        public Move? Move { get; }

        public int Step { get; }

        public int Score { get; }
    }
}

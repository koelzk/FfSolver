

namespace FfSolver;


public class Solver
{
    private readonly Dictionary<Board, Node> visitedNodes = new Dictionary<Board, Node>();
    private readonly PriorityQueue<Node, int> queue = new PriorityQueue<Node, int>();

    Board start;

    public Solver(Board start)
    {
        this.start = start;
    }

    public SolveResult Solve(int maxIterations = 500_000, int maxSteps = 50)
    {
        var board = new Board(start);
        board.Normalize();
        var startNode = new Node(board, new Move(0, 1), 0, 0);
        queue.Enqueue(startNode, 0);

        for (var iteration = 0; iteration < maxIterations; iteration++)
        {
            if (queue.Count == 0)
            {
                return new SolveResult(SolveResultStatus.NoSolution);
            }

            var currentNode = queue.Dequeue();
            var current = currentNode.Board;

            if (current.IsGameWon)
            {
                return new SolveResult(SolveResultStatus.Solved, GetMoves(current));
            }   

            var moves = current.EnumerateMoves().ToList();

            foreach (var move in moves)
            {
                AddNode(currentNode, move, maxSteps);
            }
        }

        return new SolveResult(SolveResultStatus.ReachedMaxIterations);
    }

    private List<Move>? GetMoves(Board current)
    {
        var moves = new List<Move>();
        var board = current;
        while (true)
        {
            if (visitedNodes.TryGetValue(board, out var previousNode))
            {
                moves.Add(previousNode.Move);
                board = previousNode.Board;
            }
            else
            {
                break;
            }
        }

        // Remove board normalization from moves:
        return moves;
    }

    private void AddNode(Node currentNode, Move move, int maxSteps)
    {
        Board next = new Board(currentNode.Board);
        next.ApplyMove(move);
        next.Normalize();

        if (!visitedNodes.ContainsKey(next) && currentNode.Step < maxSteps)
        {
            var nextNode = new Node(next, move, currentNode.Step + 1, next.GetScore(currentNode.Step + 1));
            visitedNodes.Add(next, currentNode);
            queue.Enqueue(nextNode, -nextNode.Score);
        }
        else
        {
            //TODO: Check for shortcuts
        }
    }

    public class Node
    {
        public Node(Board board, Move move, int step, int score)
        {
            Board = board;
            Move = move;
            Step = step;
            Score = score;
        }

        public Board Board { get; }

        public Move Move { get; }

        public int Step { get; }

        public int Score { get; }
    }
}

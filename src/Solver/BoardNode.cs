namespace FfSolver;


public partial class Solver
{
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

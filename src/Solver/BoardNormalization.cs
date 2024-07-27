namespace FfSolver;

public class BoardNormalization
{
    private int[] cascadeIndices;

    public BoardNormalization()
    {
        cascadeIndices = Enumerable.Range(0, Board.CascadeCount).ToArray();
    }

    public void Advance(Board board)
    {
        var cascadeMap = board.Cascades
            .Zip(cascadeIndices, (cc, ci) => (cascade: cc, cascadeIndex: ci, stackId: CascadeRank(cc)))
            .OrderBy(t => t.stackId)
            .ToArray();

        cascadeIndices = cascadeMap.Select(t => t.cascadeIndex).ToArray();
    }

    private int Translate(int index) => index >= 0 ? cascadeIndices[index] : index;

    public Move Translate(Move m) => new Move(Translate(m.From), Translate(m.To), m.Count);

    public static int CascadeRank(IReadOnlyList<Card> cascade) =>
        cascade.Count == 0 ? int.MaxValue : cascade[0].Value;
}

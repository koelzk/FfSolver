using Avalonia;
using Avalonia.Media;
using FfSolver;
using ReactiveUI;
using SolverAvn.Services;

namespace SolverAvn.ViewModels;

/// <summary>
/// View model for displaying a single card
/// </summary>
/// <seealso cref="Views.CardView"/>.
public class CardViewModel : ViewModelBase
{
    private Card card;

    public CardViewModel()
    {
        Card = new Card(Card.QueenRank, Suit.Red);
    }

    public CardViewModel(DetectedCard detectedCard)
    {
        Card = detectedCard.Card;
        Region = detectedCard.Region;
    }

    /// <summary>
    /// Gets the card label
    /// </summary>
    public string Label { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the card suit
    /// </summary>
    public Suit Suit { get; private set; }

    /// <summary>
    /// Gets the background brush
    /// </summary>
    public IBrush Background { get; private set; } = new SolidColorBrush(Color.FromRgb(255, 255, 255));

    /// <summary>
    /// Gets the card
    /// </summary>
    public Card Card
    {
        get => card; set
        {
            if (!card.Equals(value))
            {
                this.RaiseAndSetIfChanged(ref card, value);

                Label = GetLabel();
                Suit = card.Suit;
                Background = new SolidColorBrush(GetBackgroundColor());
                Foreground = new SolidColorBrush(GetForegroundColor());

                this.RaisePropertyChanged(nameof(Label));
                this.RaisePropertyChanged(nameof(Suit));
                this.RaisePropertyChanged(nameof(Background));
                this.RaisePropertyChanged(nameof(Foreground));
            };
        }
    }

    /// <summary>
    /// Gets the region of the detected card in the screenshot.
    /// </summary>
    public Rect Region { get; }

    /// <summary>
    /// Gets the foreground brush
    /// </summary>
    public IBrush Foreground { get; private set; } = new SolidColorBrush(Color.FromRgb(0, 0, 0));

    private string GetLabel()
    {
        if (card.Suit == Suit.MajorArc)
        {
            return card.Rank.ToString();
        }

        return card.Rank switch
        {
            Card.AceRank => "A",
            Card.JackRank => "J",
            Card.QueenRank => "Q",
            Card.KingRank => "K",
            _ => card.Rank.ToString(),
        };
    }

    private Color GetBackgroundColor()
    {
        return card.Suit switch
        {
            Suit.MajorArc => Color.FromUInt32(0xff282523),
            _ => Color.FromUInt32(0xfff8e3c1)
        };
    }

    private Color GetForegroundColor()
    {
        return card.Suit switch
        {
            Suit.Red => Color.FromUInt32(0xff963728),
            Suit.Green => Color.FromUInt32(0xff497327),
            Suit.Blue => Color.FromUInt32(0xff326973),
            Suit.Yellow => Color.FromUInt32(0xff956f3f),
            _ => Color.FromUInt32(0xffeea96b),
        };
    }
}

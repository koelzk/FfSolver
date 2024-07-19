using System.Text;
using FfSolver;
using Microsoft.VisualBasic;
using SkiaSharp;

/// <summary>
/// Reads the board state from a screenshot of Fortune's Foundation captured at 1920x1080 resolution.
/// </summary>
/// <remarks>
/// The board state of a new game is captured by checking the 70 cards laid out in 10
/// cascades (middle cascade remains empty) with 7 cards each.
/// For each card, this is done by extracting a 118x29 pixel tile from the screenshot
/// and then comparing it with template tiles using summed scquare difference.
/// </remarks>
public class BoardExtractor
{
    /// <summary>
    /// Left X-position of each cascade in pixels
    /// </summary>
    private static readonly int[] XCoords = [178, 323, 467, 612, 757, 902, 1046, 1191, 1336, 1480, 1625];
    /// <summary>
    /// Top Y-position of each cascade in pixels
    /// </summary>
    private static readonly int[] YCoords = [347, 378, 409, 440, 471, 502, 533, 564, 595, 626, 657, 688, 719, 750, 781, 812, 843, 874, 905, 936, 967, 998, 1029];

    private const int cellX = 1378;

    private const int cellY = 122;

    private readonly IReadOnlyDictionary<Card, SKBitmap> templateMap;
    /// <summary>
    /// Tile width in pixels
    /// </summary>
    private const int TileWidth = 118;
    /// <summary>
    /// Tile height in pixels
    /// </summary>
    private const int TileHeight = 29;

    private static IEnumerable<Tile> EnumerateTiles()
    {
        var size = new SKSizeI(TileWidth, TileHeight);

        yield return new Tile(
            Move.Cell,
            0,
            SKRectI.Create(new SKPointI(cellX, cellY), new SKSizeI(TileHeight, TileWidth)),
            true);

        var cascadeTiles = YCoords.SelectMany(
            (y, j) => XCoords.Select((x, i) => 
                new Tile(i, j, SKRectI.Create(new SKPointI(x, y), size), false)
            )
        );

        foreach (var tile in cascadeTiles)
        {
            yield return tile;
        }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BoardExtractor"/>.
    /// </summary>
    /// <param name="templateDirPath">Directory path containing template images</param>
    public BoardExtractor(string templateDirPath)
    {
        templateMap = LoadTemplates(templateDirPath);
    }

    private IReadOnlyDictionary<Card, SKBitmap> LoadTemplates(string templateDirPath)
    {
        var templateFiles = Directory.EnumerateFiles(templateDirPath);

        var map = templateFiles.ToDictionary(
            filename => BoardHelper.ParseCard(Path.GetFileNameWithoutExtension(filename))
                ?? throw new ArgumentOutOfRangeException(nameof(filename), filename, "Unexpected file name"),
            filename => SKBitmap.Decode(filename));

        return map;
    }

    private static long GetSsd(SKBitmap a, SKBitmap b)
    {
        if (a is null)
        {
            throw new ArgumentNullException(nameof(a));
        }

        if (b is null)
        {
            throw new ArgumentNullException(nameof(b));
        }

        if (a.Width != b.Width || a.Height != b.Height || a.BytesPerPixel != b.BytesPerPixel)
        {
            throw new ArgumentException("Images must have same width, height and bytes per pixel", nameof(b));
        }

        long ssd = 0;

        var spanA = a.GetPixelSpan();
        var spanB = b.GetPixelSpan();

        for (var i = 0; i < spanA.Length;)
        {
            for (var j = 0; j < 3; j++)
            {
                var diff = spanA[i] - spanB[i];
                ssd += diff * diff;
                i++;
            }

            i++; // Skip Alpha
        }

        return ssd;
    }

    public Board DetectBoard(string imageFilePath, ICollection<ExtractedCard>? extractedCards = null)
    {
        var image = SKBitmap.Decode(imageFilePath);

        if (image.Width != 1920 || image.Height != 1080)
        {
            throw new ArgumentException("Image resolution is not 1920x1080.", nameof(imageFilePath));
        }

        var sb = new StringBuilder();
        string cellString = "";

        var candidates = new HashSet<Card>(templateMap.Keys);

        foreach (var tile in EnumerateTiles())
        {
            if (candidates.Count == 0)
            {
                break;
            }

            var tileImage = tile.GetImage(image);

            var ssds = candidates
                .Select(card => (card: card, ssd: GetSsd(tileImage, templateMap[card])))
                .TakeUntil(t => t.ssd > 0)
                .OrderBy(t => t.ssd)
                .ToList();
            var card = ssds
                .Where(t => t.ssd <= 200_000)
                .Select(t => (Card?)t.card)
                .FirstOrDefault();

            var cardString = card?.ToString() ?? "-";

            if (card != null)
            {
                extractedCards?.Add(new ExtractedCard(
                    new ExtractedCardRegion(tile.Region.Left, tile.Region.Top, tile.Region.Width, tile.Region.Height),
                    card.Value));
                candidates.Remove(card.Value);
            }

            if (tile.CascadeIndex == Move.Cell)
            {
                cellString = cardString;
            }
            else
            {
                sb.Append(cardString);
                var column = tile.CascadeIndex;
                sb.Append(column == 10 ? "\n" : " ");
            }
        }

        return BoardHelper.Parse(sb.ToString(), cellString);
    }

    public static void ExtractImageTile(string imageFilePath, Func<int, string> tileNameSelector)
    {
        var image = SKImage.FromEncodedData(imageFilePath);

        if (image.Width != 1920 || image.Height != 1080)
        {
            throw new ArgumentException("Image resolution is not 1920x1080.", nameof(imageFilePath));
        }

        foreach (var (tile, index) in EnumerateTiles().Select(Tuple.Create<Tile, int>))
        {
            var tileImage = image.Subset(tile.Region);
            var name = tileNameSelector?.Invoke(index) ?? index.ToString();
            var imagePath = $"{name}.png";

            WriteImageFile(tileImage, imagePath);
        }
    }

    private static void WriteImageFile(SKImage tileImage, string imagePath)
    {
        var encodedData = tileImage.Encode(SKEncodedImageFormat.Png, 100);
        using var bitmapImageStream = File.Open(imagePath, FileMode.Create, FileAccess.Write, FileShare.None);
        encodedData.SaveTo(bitmapImageStream);
        bitmapImageStream.Flush(true);
    }

    private record Tile(int CascadeIndex, int Row, SKRectI Region, bool Rotated)
    {
        internal SKBitmap GetImage(SKBitmap image)
        {
            var tileImage = new SKBitmap();
            image.GetPixelSpan();

            if (!image.ExtractSubset(tileImage, Region))
            {
                throw new InvalidDataException("Could not extract tile image");
            }

            if (Rotated)
            {
                var rotated = new SKBitmap(tileImage.Height, tileImage.Width);

                using (var surface = new SKCanvas(rotated))
                {
                    surface.Translate(rotated.Width, 0);
                    surface.RotateDegrees(90);
                    surface.DrawBitmap(tileImage, 0, 0);
                }

                return rotated;
            }

            return tileImage.Copy();
        }
    }
}

public record ExtractedCardRegion(int Left, int Top, int Width, int Height);

public record ExtractedCard(ExtractedCardRegion Region, Card Card);

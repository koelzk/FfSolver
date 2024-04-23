using System.Security.Cryptography;
using System.Text;
using FfSolver;
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
    private static readonly int[] XCoords = [178, 323, 467, 612, 757, 1046, 1191, 1336, 1480, 1625];
    /// <summary>
    /// Top Y-position of each cascade in pixels
    /// </summary>
    private static readonly int[] YCoords = [347, 378, 409, 440, 471, 502, 533];
    private readonly IReadOnlyDictionary<Card, SKBitmap> templateMap;
    /// <summary>
    /// Tile width in pixels
    /// </summary>
    private const int TileWidth = 118;
    /// <summary>
    /// Tile height in pixels
    /// </summary>
    private const int TileHeight = 29;

    private static IEnumerable<SKRectI> EnumerateTiles()
    {
        var size = new SKSizeI(TileWidth, TileHeight);
        return YCoords.SelectMany(
            y => XCoords.Select(x => SKRectI.Create(new SKPointI(x, y), size)));
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

    public Board DetectBoard(string imageFilePath)
    {
        var image = SKBitmap.Decode(imageFilePath);

        if (image.Width != 1920 || image.Height != 1080)
        {
            throw new ArgumentException("Image resolution is not 1920x1080.", nameof(imageFilePath));
        }

        var sb = new StringBuilder();

        var candidates = new HashSet<Card>(templateMap.Keys);

        foreach (var (tile, index) in EnumerateTiles().Select(Tuple.Create<SKRectI, int>))
        {
            var tileImage = new SKBitmap();

            image.GetPixelSpan();

            if (!image.ExtractSubset(tileImage, tile))
            {
                throw new InvalidDataException("Could not extract tile image");
            }

            tileImage = tileImage.Copy();

            (Card card, long ssd) best = (candidates.First(), long.MaxValue);
            foreach (var card in candidates)
            {
                var ssd = GetSsd(tileImage, templateMap[card]);
                Console.WriteLine($"{card} => {ssd}");                

                if (ssd < best.ssd)
                {
                    best = (card, ssd);

                    if (ssd == 0)
                    {
                        break;
                    }
                }
            }

            candidates.Remove(best.card);

            sb.Append(best.card);

            var column = index % 10;
            sb.Append(column == 9 ? "\n" : column == 4 ? " - " : " ");
        }

        return BoardHelper.Parse(sb.ToString());        
    }    

    public static void ExtractImageTile(string imageFilePath, Func<int, string> tileNameSelector)
    {
        var image = SKImage.FromEncodedData(imageFilePath);

        if (image.Width != 1920 || image.Height != 1080)
        {
            throw new ArgumentException("Image resolution is not 1920x1080.", nameof(imageFilePath));
        }

        foreach (var (tile, index) in EnumerateTiles().Select(Tuple.Create<SKRectI, int>))
        {
            var tileImage = image.Subset(tile);
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
}
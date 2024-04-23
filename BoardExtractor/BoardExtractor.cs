using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using FfSolver;
using SkiaSharp;

public class BoardExtractor
{
    private static readonly int[] XCoords = { 178, 323, 467, 612, 757, 1046, 1191, 1336, 1480, 1625 };
    private static readonly int[] YCoords = { 347, 378, 409, 440, 471, 502, 533 };
    private readonly IReadOnlyDictionary<string, string> templateMap;
    private const int TileWidth = 117;
    private const int TileHeight = 29;

    private static IEnumerable<SKRectI> EnumerateTiles()
    {
        var size = new SKSizeI(TileWidth, TileHeight);
        return YCoords.SelectMany(
            y => XCoords.Select(x => SKRectI.Create(new SKPointI(x, y), size)));
    }

    public BoardExtractor()
    {
        templateMap = LoadTemplates();
    }

    private static string GetImageHash(SKImage image)
    {
            using var bitmap = SKBitmap.FromImage(image);
            var md5 = MD5.Create();
            return Convert.ToHexString(md5.ComputeHash(bitmap.Bytes));
    }

    private IReadOnlyDictionary<string, string> LoadTemplates()
    {
        var templateFiles = Directory.EnumerateFiles("templates");

        return templateFiles
            .Select(path => (
                name: Path.GetFileNameWithoutExtension(path),
                hash: GetImageHash(SKImage.FromEncodedData(path))))
            .ToDictionary(t => t.hash, t => t.name);
    }

    public Board DetectBoard(string imageFilePath)
    {
        var image = SKImage.FromEncodedData(imageFilePath);

        if (image.Width != 1920 || image.Height != 1080)
        {
            throw new ArgumentException("Image resolution is not 1920x1080.", nameof(imageFilePath));
        }

        var sb = new StringBuilder();

        foreach (var (tile, index) in EnumerateTiles().Select(Tuple.Create<SKRectI, int>))
        {
            var tileImage = image.Subset(tile);
            var hash = GetImageHash(tileImage);

            var cardString = templateMap.TryGetValue(hash, out var s) ? s : string.Empty;

            sb.Append(cardString);

            var column = index % 10;
            sb.Append(column == 9 ? "\n" : column == 4 ? " - " : " ");
        }

        return BoardHelper.Parse(sb.ToString());        
    }    

    public static void ExtractImageTile(string imageFilePath)
    {
        var image = SKImage.FromEncodedData(imageFilePath);

        if (image.Width != 1920 || image.Height != 1080)
        {
            throw new ArgumentException("Image resolution is not 1920x1080.", nameof(imageFilePath));
        }

        foreach (var (tile, index) in EnumerateTiles().Select(Tuple.Create<SKRectI, int>))
        {
            var tileImage = image.Subset(tile);
            
            var encodedData = tileImage.Encode(SKEncodedImageFormat.Png, 100);
            var imagePath = $"{index}.png";
            using var bitmapImageStream = File.Open(imagePath, FileMode.Create, FileAccess.Write, FileShare.None);
            encodedData.SaveTo(bitmapImageStream);
            bitmapImageStream.Flush(true);
        }
    }
}
using System.Security.Cryptography;
using System.Text;
using FfSolver;
using SkiaSharp;

/// <summary>
/// Reads the board state from a screenshot of Fortune's Foundation captured at 1920x1080 resolution.
/// </summary>
public class BoardExtractor
{
    private static readonly int[] XCoords = [178, 323, 467, 612, 757, 1046, 1191, 1336, 1480, 1625];
    private static readonly int[] YCoords = [347, 378, 409, 440, 471, 502, 533];
    private readonly IReadOnlyDictionary<string, string> templateMap;
    private const int TileWidth = 117;
    private const int TileHeight = 29;

    private readonly static IReadOnlyDictionary<string, string> defaultTemplateMap = new Dictionary<string, string>
    {
        { "1DB1F4552315CA0C06631DC5C659CF4F", "4G" },
        { "D6C0A9B99A8CAEF63032FBCD0714C022", "9R" },
        { "64195CDCA847FB87FC79B3728A380AFC", "3" },
        { "F64FA37EDD9581C4898B91629EED6B03", "7G" },
        { "2E110F0E4C24813C530263CC48F76140", "2" },
        { "D621C584E3E488E40D3D3F1CBC599479", "KY" },
        { "337B2D1D1FC4A2951150F3319BDF94CB", "8G" },
        { "7AFD7E6DD72F2F6FC17C6A0B76B4DA3E", "KB" },
        { "AB2ED73748071FACB2A4379B5A86C175", "KG" },
        { "8D1B6789ADAD79092DE02B67B82EE0E2", "6B" },
        { "647C1D7608226B0D7901DA1D7D411805", "JG" },
        { "450279239496A88F24448E5D2F7B721E", "2B" },
        { "E4998AC981B79654AF8F0567DC3D3C31", "20" },
        { "986F6CB35E0A880683EF43D694E19CAE", "13" },
        { "4C6B50ED0B78B9B5DDBE8E46DAEBEC34", "QG" },
        { "8F238C755E9AE5D4F364420096C6F0C9", "0" },
        { "66CBB652F2B3A34809B36FEF142B596C", "3R" },
        { "C3BEA25B3155CB994AC71BF63A533D6D", "7R" },
        { "BDD99396D5B8B5C3B8A5036BC846CA7A", "12" },
        { "D10E639DD78B4A78EE5481F0116E7A91", "17" },
        { "20D801893685A499AFB87BCC1497641F", "10G" },
        { "00A548B37E4BBBAB2A42D7A3691865C7", "9Y" },
        { "1A08BD35287A475F1B96A13B1D7B47E7", "8" },
        { "DC2818FA57B4AE908AB3E7382ACF0924", "9" },
        { "44EE74432D1FCC6E1461E57DCED8D3CC", "7" },
        { "477812392710252571478E8749E34289", "6G" },
        { "6929A82A0CCE6E1E7BB22273F74EBBC2", "KR" },
        { "ADD7079344AFCEBFC3FC8CDFF48D14E8", "5" },
        { "15122D9B628CB09A2938274020B0A3F2", "4B" },
        { "966471388C1E93EA9961EF9CD13E6BB7", "8Y" },
        { "8C32615E7A5AE7814415A84012D66CF2", "5Y" },
        { "142A7C5A4FD8141C7A1DFA4D45EDCAE8", "9G" },
        { "AFA8AA48FF5F38C4B5E470B189650554", "QB" },
        { "45B6D7E898E2BD38426685F3A1971552", "16" },
        { "86FEE991CA5266FED00AA7197EB33AF6", "21" },
        { "199BE6EEFB855411BAE8B9A4CA4B2B26", "10B" },
        { "7AE71F00E8F24F96FC9CD1BDEE67E3E1", "5R" },
        { "00F1C76ED2C422E59110C598D14BE7A8", "5G" },
        { "BC0FBC413D0C64BF13AFD4CF6811ABEB", "6Y" },
        { "6483AAB0CEB46682738B8FBE42F30427", "3Y" },
        { "F175E971A69A20B570BC91C6D6E66776", "2G" },
        { "DA0599F54EB38F35D280D037DAC60671", "9B" },
        { "F0E24F89194235F9B324EA2466DCACDE", "4Y" },
        { "A24C1B0BF114116E8B79FF3900C5216F", "JB" },
        { "DCC0CD4D0CD94A8321F0BE24850B90F1", "3B" },
        { "3B46B9BDF53658C960E249B1A4C9BA26", "11" },
        { "FCD4B42B0F4FF38CF9515691C6ACAA77", "QY" },
        { "5F78358A39BCF57F0E147062686EC8D8", "JR" },
        { "EDA9CD5265F9D283ECB3E1177EBE513D", "14" },
        { "8ABFC2C1915DDBEAFCE20C2E70E08F65", "19" },
        { "FA5F54BF24C61C556AF4C9FEF2903246", "10R" },
        { "BE851CD6133566EA16A88C81E88A41FB", "2Y" },
        { "96F567BCF93E6AB022E1BCFD6902B8C2", "JY" },
        { "1A41C825EBD2B0DCC2A041BBC5693428", "7Y" },
        { "E7CCC57CB170EDA17F24A4536A8307DA", "4R" },
        { "95974B4490F72554066FF42AD9545119", "10" },
        { "C51EF545563DBC4EA415DF0B1A60A7FA", "2R" },
        { "41CDF64A625FF43872F8E949C1073BBB", "4" },
        { "37E7ACC5867EFB2B74FE22AE1B0FB21D", "QR" },
        { "A0749E24EFA4C5F776C76973FCAA083F", "15" },
        { "2FCC48D58E3B7D57E7D78043B64DA17C", "6" },
        { "92E373C928B753971B8588A478AA373A", "5B" },
        { "FF1EC32C97157E60039266A9D32728E6", "10Y" },
        { "BFC539C411C05FA2108F166F64EA95EF", "3G" },
        { "41E27C1BF184BB1C69EB5F8425E0FB7F", "8B" },
        { "49C0995550F95B89D6695AE146B5B6D2", "1" },
        { "685DA83680878726911AE90C1E73B2AA", "7B" },
        { "54B42D3414310DD5844A26ECC43C0E6A", "18" },
        { "DF8D9DEDBFED472DD8ACFF9239BB8A52", "6R" },
        { "730B72F6BA365A3B181A48A15A00A443", "8R" },
    };

    private static IEnumerable<SKRectI> EnumerateTiles()
    {
        var size = new SKSizeI(TileWidth, TileHeight);
        return YCoords.SelectMany(
            y => XCoords.Select(x => SKRectI.Create(new SKPointI(x, y), size)));
    }

    public BoardExtractor()
    {
        //templateMap = LoadTemplates(); // Uncomment this to re-create hashes from template images
        templateMap = defaultTemplateMap;
    }

    private static string GetImageHash(SKImage image)
    {
            using var bitmap = SKBitmap.FromImage(image);
            var md5 = MD5.Create();
            return Convert.ToHexString(md5.ComputeHash(bitmap.Bytes));
    }

    private IReadOnlyDictionary<string, string> LoadTemplates()
    {
        var templateFiles = Directory.EnumerateFiles("src/BoardExtractor/templates");

        var map = templateFiles
            .Select(path => (
                name: Path.GetFileNameWithoutExtension(path),
                hash: GetImageHash(SKImage.FromEncodedData(path))))
            .ToDictionary(t => t.hash, t => t.name);

        foreach (var kvp in map)
        {
            Console.WriteLine($"{kvp.Key}\t{kvp.Value}");
        }

        return map;
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
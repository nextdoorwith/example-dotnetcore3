using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace JsonSerializerExample
{
    public class Example3
    {
        public static async Task Run()
        {
            var json = await LoadJson("input.json");
            await WriteJson("output.json", json);
        }

        public static async Task<BasicExampleElement> LoadJson(string filename)
        {
            // デシリアライズ時のオプション
            var serializerOptions = new JsonSerializerOptions()
            {
                // キャメルケースとプロパティ名の相違に対応
                // ・例: JsonのmyNameフィールドをMyNameプロパティに対応付け
                // ・大小文字を区別する場合は[JsonPropertyName]を使用のこと
                PropertyNameCaseInsensitive = true,
                // コメントをスキップ
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            // DateTimeの既定の日付書式を変更
            // (既定のISO形式ではなく独自の日付書式に変更)
            serializerOptions.Converters.Add(new DateTimeLocalConverter());

            // JSONの読み取り
            // ※日本語を含むUTF8(BOMなし)のファイルを前提
            using var stream = new FileStream(filename, FileMode.Open);
            return await JsonSerializer.DeserializeAsync<BasicExampleElement>(stream, serializerOptions);
        }

        public static async Task WriteJson(string filename, BasicExampleElement element)
        {
            // シリアライズ時のオプション
            var serializerOptions = new JsonSerializerOptions()
            {
                // 日本語文字のユニコードエスケープを防止
                // (例えば "あ" が "\u3042" のようにエスケープされないようにする。)
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                // 人が参照しやすいようインデントを付与(不要であれば、このオプションは除外)
                WriteIndented = true,
                // キャメルケースでJsonフィールドを使用(既定はパスカルケース)
                // (例: MyNameプロパティは、myNameというJsonフィールドとして出力)
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            // DateTime既定の日付書式を変更
            serializerOptions.Converters.Add(new DateTimeLocalConverter());

            // JSONの出力
            using var stream = new FileStream(filename, FileMode.OpenOrCreate | FileMode.CreateNew);
            await JsonSerializer.SerializeAsync(stream, element, serializerOptions);
        }

    }
}

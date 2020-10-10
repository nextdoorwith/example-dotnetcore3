using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace JsonSerializerExample
{
    public class Example3
    {
        public static void Run()
        {
            var json = LoadJson("input.json");
            WriteJson("output.json", json);
        }

        public static BasicExampleElement LoadJson(string filename)
        {
            // 日本語を含むUTF8(BOMなし)のファイルを前提
            var bytes = File.ReadAllBytes(filename);

            // 日本語を扱うためにUtf8JsonReaderを使用
            var readerOptions = new JsonReaderOptions()
            {
                CommentHandling = JsonCommentHandling.Skip, // コメントをスキップ
            };
            var reader = new Utf8JsonReader(bytes, readerOptions);

            // デシリアライズ時のオプション
            var serializerOptions = new JsonSerializerOptions()
            {
                // キャメルケースとプロパティ名の相違に対応
                // ・例: JsonのmyNameフィールドをMyNameプロパティに対応付け
                // ・大小文字を区別する場合は[JsonPropertyName]を使用のこと
                PropertyNameCaseInsensitive = true,
            };

            // DateTimeの既定の日付書式を変更
            // (既定のISO形式ではなく独自の日付書式に変更)
            serializerOptions.Converters.Add(new DateTimeLocalConverter());

            // JSONの読み取り
            return JsonSerializer.Deserialize<BasicExampleElement>(ref reader, serializerOptions);
        }

        public static void WriteJson(string filename, BasicExampleElement element)
        {
            using var stream = new FileStream(filename, FileMode.OpenOrCreate | FileMode.CreateNew);

            // 日本語を扱うためにUtf8JsonReaderを使用
            var writerOptions = new JsonWriterOptions()
            {
                // 日本語文字のユニコードエスケープを防止
                // (例えば "あ" が "\u3042" のようにエスケープされないようにする。)
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                // 人が参照しやすいようインデントを付与(不要であれば、このオプションは除外)
                Indented = true
            };
            var writer = new Utf8JsonWriter(stream, writerOptions);

            // シリアライズ時のオプション
            var serializerOptions = new JsonSerializerOptions()
            {
                // キャメルケースでJsonフィールドを使用(既定はパスカルケース)
                // (例: MyNameプロパティは、myNameというJsonフィールドとして出力)
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // DateTime既定の日付書式を変更
            serializerOptions.Converters.Add(new DateTimeLocalConverter());

            // JSONの出力
            JsonSerializer.Serialize(writer, element, serializerOptions);
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace JsonSerializerExample
{
    class Example1
    {
        public static void Run()
        {
            // シリアライズ(クラス・プロパティ -> JSON)
            var dic = new Dictionary<string, object>()
            {
                ["intValue"] = 100,
                ["stringValue"] = "ABCD",
                ["boolValue"] = true,
                ["intArrayValue"] = new int[]{ 1, 2, 3, 4 }
            };
            var serialized = JsonSerializer.Serialize(dic);
            Console.WriteLine($"serialized: {serialized}");

            // デシリアライズ(JSON -> クラス・プロパティ)
            var deserialized = JsonSerializer.Deserialize<Dictionary<string, object>>(serialized);
            foreach(var kv in deserialized)
                Console.WriteLine($"deserialize: {kv.Key}: {kv.Value}");

        }

    }
}

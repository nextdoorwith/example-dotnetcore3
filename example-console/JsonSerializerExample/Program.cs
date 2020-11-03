using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace JsonSerializerExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Example1 ======================");
            Example1.Run();

            Console.WriteLine("Example2 ======================");
            Example2.Run();

            Console.WriteLine("Example3 ======================");
            await Example3.Run();
        }

    }
}

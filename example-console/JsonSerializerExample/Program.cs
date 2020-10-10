using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace JsonSerializerExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Example1 ======================");
            Example1.Run();

            Console.WriteLine("Example2 ======================");
            Example2.Run();

            Console.WriteLine("Example3 ======================");
            Example3.Run();
        }

    }
}

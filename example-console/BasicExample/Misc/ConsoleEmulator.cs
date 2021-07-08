using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace BasicExample.Misc
{
    /// <summary>
    /// コンソール出力エミュレータ
    /// </summary>
    public class ConsoleEmulator
    {
        private readonly ITestOutputHelper _output;
        public ConsoleEmulator(ITestOutputHelper output)
            => _output = output;

        public void WriteLine(string value)
            => _output.WriteLine(value);

        public void WriteLine(object value)
            => _output.WriteLine(value?.ToString());

        public void WriteLine(string format, params object[] args)
            => _output.WriteLine(format, args);
    }
}

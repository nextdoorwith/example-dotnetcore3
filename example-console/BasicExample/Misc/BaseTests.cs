using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace BasicExample.Misc
{
    public class BaseTests
    {
        protected ConsoleEmulator Console { get; }

        public BaseTests(ITestOutputHelper output)
        {
            Console = new ConsoleEmulator(output);
        }
    }
}

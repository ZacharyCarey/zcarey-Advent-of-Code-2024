using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AdventOfCode.Parsing
{
    public static partial class Parser
    {
        public static IEnumerable<string> GetLines(this string input)
        {
            return input.Split(
                new string[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );
        }
    }

}

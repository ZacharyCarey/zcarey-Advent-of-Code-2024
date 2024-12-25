using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Day_13
{
    internal class Program : ProgramStructure<ClawMachine[]>
    {

        Program() : base(input => input.GetBlocks().Select(ClawMachine.Parse).ToArray())
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(ClawMachine[] input)
        {
            return input.Select(TokenCost).Sum();
        }

        private long TokenCost(ClawMachine machine)
        {
            // A little linear algebra to calculate the number of button pushes
            long num = (machine.Prize.Y * machine.ButtonA.X - machine.Prize.X * machine.ButtonA.Y);
            long den = (machine.ButtonB.Y * machine.ButtonA.X - machine.ButtonB.X * machine.ButtonA.Y);
            if (num % den != 0)
            {
                // Non-integer solutions are not valid
                return 0;
            }
            long b = num / den;

            num = (machine.Prize.X - b * machine.ButtonB.X);
            den = machine.ButtonA.X;
            if (num % den != 0)
            {
                // Non-integer solutions are not valid
                return 0;
            }
            long a = num / den;

            return 3 * a + b;
        }

        protected override object SolvePart2(ClawMachine[] input)
        {
            Func<ClawMachine, ClawMachine> AdjustPrizeLocation = (input) => new ClawMachine(input.ButtonA, input.ButtonB, input.Prize + new LargeSize(10000000000000, 10000000000000));
            return input.Select(AdjustPrizeLocation).Select(TokenCost).Sum();
        }

    }

    internal struct ClawMachine
    {
        private static readonly Regex ButtonRegex = new(@"Button [AB]: X([+-]\d+), Y([+-]\d+)", RegexOptions.Compiled);
        private static readonly Regex PrizeRegex = new(@"Prize: X=(\d+), Y=(\d+)", RegexOptions.Compiled);

        public LargePoint ButtonA;
        public LargePoint ButtonB;
        public LargePoint Prize;

        public ClawMachine(LargePoint btnA, LargePoint btnB, LargePoint prize)
        {
            this.ButtonA = btnA;
            this.ButtonB = btnB;
            this.Prize = prize;
        }

        public static ClawMachine Parse(List<string> input)
        {
            ClawMachine result = new();

            var btnAMatch = ButtonRegex.Match(input[0]);
            result.ButtonA.X = long.Parse(btnAMatch.Groups[1].Value);
            result.ButtonA.Y = long.Parse(btnAMatch.Groups[2].Value);

            var btnBMatch = ButtonRegex.Match(input[1]);
            result.ButtonB.X = long.Parse(btnBMatch.Groups[1].Value);
            result.ButtonB.Y = long.Parse(btnBMatch.Groups[2].Value);

            var prizeMatch = PrizeRegex.Match(input[2]);
            result.Prize.X = long.Parse(prizeMatch.Groups[1].Value);
            result.Prize.Y = long.Parse(prizeMatch.Groups[2].Value);

            return result;
        }
    }
}

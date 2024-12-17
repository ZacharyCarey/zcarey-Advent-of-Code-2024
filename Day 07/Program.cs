using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day_07
{
    internal class Program : ProgramStructure<List<Equation>>
    {

        Program() : base(x => x.GetLines().Create<string, Equation>().ToList())
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(List<Equation> input)
        {
            return input.Where(eq => IsSolvable(eq, '+', '*')).Select(eq => eq.Result).Sum();
        }

        protected override object SolvePart2(List<Equation> input)
        {
            return input.Where(eq => IsSolvable(eq, '+', '*', '|')).Select(eq => eq.Result).Sum();
        }

        private static bool IsSolvable(Equation eq, params char[] ops)
        {
            return GetAllOperators(eq.Numbers.Length - 1, ops).Select(eq.IsValid).Any(x => x == true);
        }

        private static IEnumerable<char[]> GetAllOperators(int n, params char[] operators)
        {
            char[] ops = new char[n];
            Array.Fill(ops, operators[0]);
            while(true)
            {
                yield return ops;
                int i;
                for (i = 0; i < n; i++)
                {
                    bool stop = false;
                    for (int j = 0; j < operators.Length - 1; j++)
                    {
                        if (ops[i] == operators[j])
                        {
                            ops[i] = operators[j + 1];
                            stop = true;
                            break;
                        }
                    }
                    if (stop) break;
                    ops[i] = operators[0];
                }
                if (i == n) yield break;
            }
        }
    }

    internal struct Equation : IObjectParser<string, Equation> 
    {
        public long Result;
        public long[] Numbers;

        public Equation(string input)
        {
            int index = input.IndexOf(':');
            Result = long.Parse(input[..index]);
            Numbers = input[(index + 2)..].Split().Select(long.Parse).ToArray();
        }

        public static Equation Parse(string input)
        {
            return new Equation(input);
        }

        public bool IsValid(char[] ops)
        {
            long result = Numbers[0];
            for (int i = 1; i < Numbers.Length; i++)
            {
                char c = ops[i - 1];
                if (c == '*')
                {
                    result *= Numbers[i];
                }
                else if (c == '+')
                {
                    result += Numbers[i];
                }
                else if (c == '|')
                {
                    result = Combine(result, Numbers[i]);
                }
                else
                {
                    throw new ArgumentException();
                }
                if (result > this.Result) return false;
            }

            return result == this.Result;
        }

        private long Combine(long left, long right)
        {
            return long.Parse(left.ToString() + right.ToString());
        }
    }
}

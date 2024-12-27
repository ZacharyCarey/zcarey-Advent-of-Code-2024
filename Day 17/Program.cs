using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Day_17
{
    internal class Program : ProgramStructure<Computer>
    {

        Program() : base(Computer.Parse)
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(Computer input)
        {
            return input.Run();
        }

        protected override object SolvePart2(Computer input)
        {
            long A = 0;
            for (int i = input.Program.Length - 1, j = 0; i >= 0; i--, j++)
            {
                A <<= 3;
                while (!input.Compute(A).SequenceEqual(input.Program.Skip(i).Select(x => (long)x)))
                {
                    A++;
                }
            }

            return A;
        }
    }

    internal class Computer
    {
        public long[] Registers;
        public long RegisterA { get => Registers[0]; set => Registers[0] = value; }
        //public long RegisterB { get => Registers[1]; set => Registers[1] = value; }
        //public long RegisterC { get => Registers[2]; set => Registers[2] = value; }

        public int[] Program;

        // Used during computing
        private long[] registers;

        private Computer(long[] registers, int[] program)
        {
            this.Registers = registers;
            this.Program = program;
            this.registers = new long[Registers.Length];
        }

        public Computer Copy()
        {
            long[] reg = new long[Registers.Length];
            this.Registers.CopyTo(reg, 0);

            int[] prog = new int[this.Program.Length];
            this.Program.CopyTo(prog, 0);

            return new Computer(reg, prog);
        }

        public static Computer Parse(string input)
        {
            List<string>[] blocks = input.GetBlocks().ToArray();
            IEnumerable<long> registers = blocks[0].Select(x => x["Register A: ".Length..]).Select(long.Parse);
            IEnumerable<int> program = blocks[1][0]["Program: ".Length..].Split(',').Select(int.Parse);
            return new Computer(registers.ToArray(), program.ToArray());
        }

        public string Run()
        {
            return string.Join(',', Compute());
        }

        public IEnumerable<long> Compute(long registerA = -1)
        {
            int ProgramCounter = 0;
            this.Registers.CopyTo(registers, 0);
            if (registerA >= 0) this.registers[0] = registerA;

            while (ProgramCounter < Program.Length)
            {
                int op = Program[ProgramCounter];
                switch(op)
                {
                    case 0: // adv
                        registers[0] = PerformDivision(ProgramCounter, registers);
                        break;
                    case 1: // bxl
                        registers[1] = registers[1] ^ Program[ProgramCounter + 1];
                        break;
                    case 2: // bst
                        registers[1] = GetComboOperand(ProgramCounter, registers) % 8;
                        break;
                    case 3: // jnz
                        if (registers[0] != 0)
                        {
                            ProgramCounter = Program[ProgramCounter + 1];
                            continue; // Dont advance the program counter
                        }
                        break;
                    case 4: // bxc
                        registers[1] = registers[1] ^ registers[2];
                        break;
                    case 5: // out
                        yield return GetComboOperand(ProgramCounter, registers) % 8;
                        break;
                    case 6: // bdv
                        registers[1] = PerformDivision(ProgramCounter, registers);
                        break;
                    case 7: // cdv
                        registers[2] = PerformDivision(ProgramCounter, registers);
                        break;
                    default:
                        throw new Exception("Unknown opcode");

                }
                ProgramCounter += 2;
            }
        }

        private long GetComboOperand(int pc, long[] registers)
        {
            int op = Program[pc + 1];
            switch(op)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    return op;
                case 4:
                case 5:
                case 6:
                    return registers[op - 4];
                case 7:
                default:
                    throw new Exception("Invalid combo operand");
            }
        }

        private long PerformDivision(int pc, long[] registers)
        {
            long num = registers[0];
            long den = (1L << (int)GetComboOperand(pc, registers));
            return num / den;
        }
    }
}

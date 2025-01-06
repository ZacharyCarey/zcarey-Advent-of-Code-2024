using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Day_24
{
    internal class Program : ProgramStructure<(Dictionary<string, bool> Wires, List<Gate> Gates)>
    {

        Program() : base(Gate.Parse)
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1((Dictionary<string, bool> Wires, List<Gate> Gates) input)
        {
            Simulate(input);

            long result = 0;
            for (int i = 0; i < 63; i++)
            {
                if (input.Wires.TryGetValue($"z{i:00}", out bool value) && value)
                {
                    result |= (1L << i);
                }
            }

            return result;
        }

        private static void Simulate((Dictionary<string, bool> Wires, List<Gate> Gates) input)
        {
            Queue<Gate> gates = new Queue<Gate>(input.Gates);
            while (gates.Count > 0)
            {
                Gate gate = gates.Dequeue();
                if (!gate.TrySimulate(input.Wires))
                {
                    gates.Enqueue(gate);
                }
            }
        }

        protected override object SolvePart2((Dictionary<string, bool> Wires, List<Gate> Gates) input)
        {
            // https://www.build-electronic-circuits.com/full-adder/
            var invalidGates = new HashSet<Gate>();
            int highestBit = input.Gates.Where(g => g.OutputWire.StartsWith('z')).Select(g => int.Parse(g.OutputWire[1..])).Max();
            string highestOutput = $"z{highestBit:00}";
            foreach (var gate in input.Gates)
            {
                // Thankfully input gates are all correct, as only the OUTPUTS of gates are swapped.
                bool isInputGate = (gate.LeftWire.StartsWith('x') && gate.RightWire.StartsWith('y')) || (gate.LeftWire.StartsWith('y') && gate.RightWire.StartsWith('x'));
                bool isOutputGate = gate.OutputWire.StartsWith('z');

                // Input XOR gate should be followed by an AND gate and an XOR gate
                // Except the first bit, which has no carry-in
                if (isInputGate && gate.Operation == "XOR" && !gate.LeftWire.EndsWith("00"))
                {
                    foreach(Gate connectedGate in input.Gates.Where(g => g.InputWires.Contains(gate.OutputWire)))
                    {
                        if (connectedGate.Operation == "OR")
                        {
                            invalidGates.Add(gate);
                        } 
                    }
                }

                // Input AND gate is only followed by an OR gate
                // Except the first bit, which has no carry-in
                if (isInputGate && gate.Operation == "AND" && !gate.LeftWire.EndsWith("00"))
                {
                    
                    foreach (Gate connectedGate in input.Gates.Where(g => g.InputWires.Contains(gate.OutputWire)))
                    {
                        if (connectedGate.Operation == "AND")
                        {
                            invalidGates.Add(gate);
                        }
                    }
                }

                // Intermediate gates are only AND or OR gates, no XOR
                if (!isInputGate && !isOutputGate)
                {
                    if (gate.Operation == "XOR")
                    {
                        invalidGates.Add(gate);
                    }
                }
                
                // All output gates should be an XOR gate, except the last bit which is the carry-out from the previous bit
                if (isOutputGate)
                {
                    if (gate.OutputWire == highestOutput)
                    {
                        // Highest bit gate should be an OR gate from the previous bit carry-out
                        if (gate.Operation != "OR")
                        {
                            invalidGates.Add(gate);
                        }
                    } else
                    {
                        // Output gate should be XOR
                        if (gate.Operation != "XOR")
                        {
                            invalidGates.Add(gate);
                        }
                    }
                }
            }

            return string.Join(',', invalidGates.Select(g => g.OutputWire).Order());
        }
    }

    internal struct Gate
    {
        private Func<bool, bool, bool> OperationFunc;
        public string Operation;
        public string LeftWire;
        public string RightWire;
        public string OutputWire;

        public Gate(string op, string left, string right, string output)
        {
            this.LeftWire = left;
            this.RightWire = right;
            this.OutputWire = output;
            switch (op)
            {
                case "AND": this.OperationFunc = AND; break;
                case "OR": this.OperationFunc = OR; break;
                case "XOR": this.OperationFunc = XOR; break;
                default: throw new Exception();
            }
            this.Operation = op;
        }

        public bool TrySimulate(Dictionary<string, bool> states)
        {
            if (states.TryGetValue(LeftWire, out bool leftState) && states.TryGetValue(RightWire, out bool rightState))
            {
                states[OutputWire] = OperationFunc(leftState, rightState);
                return true;
            } else
            {
                return false;
            }
        }

        public IEnumerable<string> InputWires {
            get {
                yield return LeftWire;
                yield return RightWire;
            }
        }

        private static bool AND(bool left, bool right)
        {
            return left && right;
        }

        private static bool OR(bool left, bool right)
        {
            return left || right;
        }

        private static bool XOR(bool left, bool right)
        {
            return left ^ right;
        }

        public static (Dictionary<string, bool> Wires, List<Gate> Gates) Parse(string input)
        {
            var blocks = input.GetBlocks().ToArray();
            var wires = new Dictionary<string, bool>();
            foreach (string line in blocks[0])
            {
                int colon = line.IndexOf(':');
                wires[line[..colon]] = (line[colon + 2] == '1');
            }

            List<Gate> gates = new();
            foreach (string line in blocks[1])
            {
                int arrow = line.IndexOf("->");
                string output = line[(arrow + 3)..];
                string[] inputs = line[..(arrow - 1)].Split();
                gates.Add(new Gate(inputs[1], inputs[0], inputs[2], output));
            }

            return (wires, gates);
        }
    }
}

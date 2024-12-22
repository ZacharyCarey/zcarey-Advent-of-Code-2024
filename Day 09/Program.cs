using AdventOfCode;
using AdventOfCode.Parsing;
using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day_09
{
    internal class Program : ProgramStructure<int[]>
    {

        Program() : base(x => ParseInput(x.AsEnumerable().Select(c => int.Parse(c.ToString())).ToArray()))
        { }

        static void Main(string[] args)
        {
            new Program()
                .Run(args);
                //.Run(args, "Example.txt");
        }

        protected override object SolvePart1(int[] input)
        {
            int freeBlockIndex = 0;

            for(int i = input.Length - 1; i >= freeBlockIndex; i--)
            {
                if (input[i] < 0) continue;

                // Found the right-most file block, find the next open slot
                for (; freeBlockIndex < i; freeBlockIndex++)
                {
                    if (input[freeBlockIndex] != -1) continue;

                    // Found an open block! Move the file
                    input[freeBlockIndex] = input[i];
                    input[i] = -1;
                    break;
                }
                //PrintFS(input);
            }

            return CalculateChecksum(input);
        }

        private static ulong CalculateChecksum(int[] input)
        {
            ulong checksum = 0;
            for (uint i = 0; i < input.Length; i++)
            {
                if (input[i] >= 0)
                {
                    checksum += i * (uint)input[i];
                }
            }
            return checksum;
        }

        private static void PrintFS(int[] input, bool simple = false)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (simple)
                {
                    if (input[i] < 0)
                    {
                        Console.Write('.');
                    } else
                    {
                        Console.Write(input[i]);
                    }
                }
                else
                {
                    Console.Write($"[{input[i]}]");
                }
            }
            Console.WriteLine();
        }

        protected override object SolvePart2(int[] input)
        {
            List<FileIndex> fileIndex = new();
            LinkedList<FileIndex> freeSpaceIndex = new();

            // First index the filesystem
            int lastID = input[0];
            int lastOffset = 0;
            int lastSize = 1;
            for(int i = 1; i < input.Length; i++)
            {
                int ID = input[i];
                if (ID != lastID)
                {
                    if (lastID < 0)
                    {
                        // Index as free space
                        freeSpaceIndex.AddLast(new FileIndex(-1, lastOffset, lastSize));
                    } else
                    {
                        // Index as a file
                        fileIndex.Add(new FileIndex(lastID, lastOffset, lastSize));
                    }

                    // Start new index
                    lastID = ID;
                    lastOffset = i;
                    lastSize = 0;
                }

                // Increase the size of the current index
                lastSize++;
            }

            if (lastSize > 0)
            {
                if (lastID < 0)
                {
                    // Index as free space
                    freeSpaceIndex.AddLast(new FileIndex(-1, lastOffset, lastSize));
                }
                else
                {
                    // Index as a file
                    fileIndex.Add(new FileIndex(lastID, lastOffset, lastSize));
                }
            }

            // Run the defragmentation algorithm
            fileIndex.Sort((a, b) => -a.ID.CompareTo(b.ID));
            foreach(var file in fileIndex)
            {
                // First try and find free space large enough
                LinkedListNode<FileIndex>? freeSpace = null;
                LinkedListNode<FileIndex>? nextNode = freeSpaceIndex.First;
                while(nextNode != null)
                {
                    if (nextNode.Value.Size >= file.Size)
                    {
                        freeSpace = nextNode;
                        break;
                    }
                    nextNode = nextNode.Next;
                }

                // If we found a large enough free space, move the file
                if (freeSpace != null && freeSpace.Value.Offset < file.Offset)
                {
                    // First move the file in the filesystem
                    Array.Fill(input, file.ID, freeSpace.Value.Offset, file.Size);
                    Array.Fill(input, -1, file.Offset, file.Size);

                    // Index the free space that moving the file will create
                    FileIndex newFreeSpace = new FileIndex(-1, file.Offset, file.Size);
                    LinkedListNode<FileIndex>? itr = freeSpaceIndex.First;
                    while (itr != null)
                    {
                        if (itr.Value.Offset >= file.Offset + file.Size)
                        {
                            freeSpaceIndex.AddBefore(itr, newFreeSpace);
                            break;
                        }
                        itr = itr.Next;
                    }
                    if (itr == null)
                    {
                        freeSpaceIndex.AddLast(newFreeSpace);
                    }

                    // Reduce the old free space where the file is being moved to
                    FileIndex temp = freeSpace.Value;
                    temp.Offset += file.Size;
                    temp.Size -= file.Size;
                    freeSpace.Value = temp;

                    if (freeSpace.Value.Size == 0)
                    {
                        freeSpaceIndex.Remove(freeSpace);
                    }

                    //PrintFS(input, true);
                }
            }

            return CalculateChecksum(input);
        }

        internal static int[] ParseInput(int[] input)
        {
            int[] result = new int[input.Sum()];
            int index = 0;
            int ID = 0;
            for (int i = 0; i < input.Length; i += 2) {
                for (int j = 0; j < input[i]; j++)
                {
                    result[index++] = ID;
                }
                ID++;

                if ((i + 1) >= input.Length) continue;
                for(int j = 0; j < input[i + 1]; j++)
                {
                    result[index++] = -1;
                }
            }

            //PrintFS(result, true);

            return result;
        }
    }

    internal struct FileIndex
    {
        public int ID;
        public int Offset;
        public int Size;

        public FileIndex()
        {
            ID = -1;
            Offset = 0;
            Size = 0;
        }

        public FileIndex(int id, int offset, int size)
        {
            this.ID = id;
            this.Offset = offset;
            this.Size = size;
        }
    }
}

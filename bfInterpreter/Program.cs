using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace bfInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the path of the program to run.");
            string path = Console.ReadLine();
            StreamReader streamReader = new StreamReader(path);
            string program = streamReader.ReadToEnd();
            Console.WriteLine(program);
            Console.ReadLine();

            Interpreter interpreter = new Interpreter(program);
            interpreter.Run();
            Console.ReadLine();
        }
    }

    class Interpreter
    {
        int programPointer = 0;

        int[] cells = new int[30000];
        int cellPointer;

        string program;
        Dictionary<int, int> bracketMatching = new Dictionary<int, int>();

        public Interpreter(string Program)
        {
            this.program = Program;
            this.bracketMatching = MatchBrackets(this.program);
        }

        /// <summary>
        /// Excecute the command and increase program pointer by 1
        /// </summary>
        /// <param name="command">The command at the position of the program pointer</param>
        void ExcecuteCommand(char command)
        {
            // Move the cell pointer one position to the right
            if (command == '>')
            {
                cellPointer += 1;
            }
            // Move the cell pointer one position to the left
            else if (command == '<')
            {
                cellPointer -= 1;
            }
            // Print the ASCII representation of the number under the cell pointer
            else if (command == '.')
            {
                Console.WriteLine((char)cells[cellPointer]);
            }
            // Take one byte of input and put it under the cell pointer
            else if (command == ',')
            {
                Console.Write("Please input one character : ");
                string input = Console.ReadLine();
                char character = input[0];
                cells[cellPointer] = (int)character;
            }
            // Increment the number under the cell pointer by one
            else if (command == '+')
            {
                cells[cellPointer] += 1;
            }
            // Decrement the number under the cell pointer by one
            else if (command == '-')
            {
                cells[cellPointer] -= 1;
            }
            // If the data under the cell pointer is currently 0, jumps to the instruction after the next ']'
            else if (command == '[')
            {
                if (cells[cellPointer] == 0)
                {
                    programPointer = bracketMatching.FirstOrDefault(x => x.Value == programPointer).Key;
                }
            }
            // If the data under the cell pointer is not currently 0, jumps back to the matching '['
            else if (command == ']')
            {
                if (cells[cellPointer] > 0)
                {
                    programPointer = bracketMatching[programPointer];
                }
            }
            programPointer += 1;
        }

        /// <summary>
        /// Creates a dictionary of matching brackets
        /// </summary>
        /// <param name="program">The brainfuck program</param>
        /// <returns>A dictionary of matching brackets (']','[')</returns>
        public Dictionary<int, int> MatchBrackets(string program)
        {
            // Count the max amount of brackets the program can have, so we
            // can make a stack of the appropriate size
            int bracketCount = 0;
            foreach (char command in program)
            {
                if (command == ']')
                {
                    bracketCount += 1;
                }
            }

            Dictionary<int, int> bracketMatching = new Dictionary<int, int>();
            Stack bracketStack = new Stack(bracketCount);

            try
            {
                for (int i = 0; i < program.Length; i++)
                {
                    // Push the positions of any '[' onto the stack
                    if (program[i] == '[')
                    {
                        bracketStack.Push(i);
                    }
                    // Pop the matching '[' position off the stack, and add it to the dictionary
                    else if (program[i] == ']')
                    {
                        int leftBracketPos = (int)bracketStack.Pop();
                        bracketMatching.Add(i, leftBracketPos);
                    }
                }

                return bracketMatching;
            }
            // Too many '[' in the brainf*ck program
            catch (StackOverflowException ex)
            {
                Console.WriteLine("Invalid bracketing. Program has unmatch '['" + ex.Message);
                return null;
            }
            // Too many ']' in the brainf*ck program
            catch (StackEmptyException ex)
            {
                Console.WriteLine("Invalid bracketing. Program has unmatched ']'" + ex.Message);
                return null;
            }
        }

        public void Run()
        {
            while (programPointer < program.Length)
            {
                ExcecuteCommand(program[programPointer]);
            }
        }
    }

    class Stack
    {
        object[] stack;
        int stackSize = 20;    // stack defaults to 20 slots
        int pointer = 0;       // points to the next empty slot in the stack

        public Stack()
        {
            stack = new object[this.stackSize];
        }

        public Stack(int StackSize)
        {
            this.stackSize = StackSize;
            stack = new object[stackSize];
        }

        // Pops the next item off the stack
        public object Pop()
        {
            // Throw exception if the stack is empty
            if (pointer == 0)
            {
                throw new StackEmptyException("Cannot pop, stack is empty");
            }

            pointer -= 1;
            object item = stack[pointer];
            stack[pointer] = null;
            return item;
        }

        // Pushes an item on to the stack
        public void Push(object item)
        {
            // Throw error if the stack is full
            if (stack[stackSize - 1] != null)
            {
                throw new StackOverflowException("Stack full");
            }

            stack[pointer] = item;
            pointer += 1;
        }

        // Returns whether the stack can pop an item
        public bool CanPop()
        {
            if (stack.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    class StackEmptyException : Exception
    {
        public StackEmptyException(string message)
            : base(message)
        {
        }
    }
}

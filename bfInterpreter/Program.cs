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

        int[] cells = new int[50000];
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
            if (command == '>')
            {
                cellPointer += 1;
            }
            else if (command == '<')
            {
                cellPointer -= 1;
            }
            else if (command == '.')
            {
                Console.Write((char)cells[cellPointer]);
            }
            else if (command == ',')
            {
                Console.Write("Please input one character : ");
                string input = Console.ReadLine();
                char character = input[0];
                cells[cellPointer] = (int)character;
            }
            else if (command == '+')
            {
                cells[cellPointer] += 1;
            }
            else if (command == '-')
            {
                cells[cellPointer] -= 1;
            }
            else if (command == '[')
            {
                if(cells[cellPointer] == 0)
                {
                    programPointer = bracketMatching.FirstOrDefault(x => x.Value == programPointer).Key;
                }
            }
            else if (command == ']')
            {
                if(cells[cellPointer] > 0)
                {
                    programPointer = bracketMatching[programPointer];
                }
            }
            programPointer += 1;
        }
        
        public Dictionary<int, int> MatchBrackets(string program)
        {
            Dictionary<int, int> bracketMatching = new Dictionary<int, int>();
            Stack bracketStack = new Stack(300);
            for(int i = 0; i < program.Length; i++)
            {
                if(program[i] == '[')
                {
                    bracketStack.Push(i);
                }
                else if(program[i] == ']')
                {
                    try
                    {
                        int leftBracketPos = (int)bracketStack.Pop();
                        bracketMatching.Add(i, leftBracketPos);
                    }
                    catch(StackEmptyException ex)
                    {
                        Console.WriteLine(ex.Message + "  Invalid Bracketing");
                    }
                    
                }
            }

            return bracketMatching;
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
            stack[pointer] = item;
            pointer += 1;
        }

        // Returns wether the stack can pop an item
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

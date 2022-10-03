using System;
using System.Threading;

namespace _8BitComputer
{
    internal class Computer
    {
        //Constructors 
        public Computer()
        {
            ALU.setRegisters(RegisterA, RegisterB, FlagsRegister);
            RAM.setRegister(MemoryAddressRegister);
            CPU.setRegister(InstructionRegister);
            ProgramCounter.setRegister(CounterRegister);

            runPrompt();
        }

        //Fields
        private static Bus BUS = new Bus();
        public static ushort controlSignals = new ushort();
        ALU ALU = new ALU();
        RAM RAM = new RAM();
        CPU CPU = new CPU();
        ProgramCounter ProgramCounter = new ProgramCounter();
        Register RegisterA = new Register();
        Register RegisterB = new Register();
        Register FlagsRegister = new Register();
        Register OutputRegister = new Register();
        Register MemoryAddressRegister = new Register();
        Register InstructionRegister = new Register();
        Register CounterRegister = new Register();
        int clockDelay = 10; //milliseconds between each clock cycle
        bool debug = false;

        //Control signals used in machine language
        public static ushort HLT = 0b1000000000000000; // HALT computation 
        public static ushort MI = 0b0100000000000000; // Memory Address Register In
        public static ushort RI = 0b0010000000000000; // RAM In
        public static ushort RO = 0b0001000000000000; // RAM Out
        public static ushort IO = 0b0000100000000000; // Instruction Register Out
        public static ushort II = 0b0000010000000000; // Instruction Register In
        public static ushort AI = 0b0000001000000000; // Register A In
        public static ushort AO = 0b0000000100000000; // Register A Out
        public static ushort SO = 0b0000000010000000; // ALU Sum Out
        public static ushort SU = 0b0000000001000000; // ALU Subtract
        public static ushort BI = 0b0000000000100000; // Register B In
        public static ushort OI = 0b0000000000010000; // Output Register In
        public static ushort CE = 0b0000000000001000; // Count Enable (increment)
        public static ushort CO = 0b0000000000000100; // Program Counter Out
        public static ushort J = 0b0000000000000010; // Jump (Program Counter In)
        public static ushort FI = 0b0000000000000001; // Flags In

        //Lookup for assembly codes. Used for debug printing only
        public static string[] assemblyCodes = new string[16]
        {
            "NOP",
            "LDA",
            "ADD",
            "SUB",
            "STA",
            "LDI",
            "JMP",
            "JC",
            "JZ",
            " ",
            " ",
            " ",
            " ",
            " ",
            "OUT",
            "HLT"
        };

        public static byte Flags_Carry = 0b01;
        public static byte Flags_Zero = 0b10;

        //Methods
        void clock(bool loop = true)
        {
            tick();
            //Loop until escape key is pressed
            while (loop && !(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                //Wait for delay
                Thread.Sleep(clockDelay);

                tick();

                if (((Computer.controlSignals & Computer.HLT) > 0)) return;
            }
        }

        void runPrompt()
        {
            string mainResponse;
            do
            {
                Console.WriteLine("[1] Run [2] RAM [3] Tick [4] Settings [5] Reset [Q]uit");
                Console.Write("Enter Selection: ");
                mainResponse = Console.ReadLine();
                if (mainResponse.Length == 0) mainResponse = "3";
                switch (mainResponse.ToLower()[0])
                {
                    case 'q':
                        //Exit
                        break;
                    case '1':
                        //Trigger Clock to Loop
                        clock();
                        break;
                    case '2':
                        //Trigger memory read/write prompts
                        RAM.interactiveMemoryWrite();
                        break;
                    case '3':
                        //Trigger single clock pulse
                        clock(false);
                        break;
                    case '4':
                        //Prompt debug
                        Console.Write("Debug (T/F):");
                        string debugTemp = Console.ReadLine();
                        debug = (debugTemp.ToLower().StartsWith('t') ? true : false);

                        //Prompt clock speed
                        Console.Write("Clock Speed (0-1000ms):");
                        string clockSpeedTempString = Console.ReadLine();
                        try
                        {
                            int clockSpeedTempInt = Convert.ToInt32(clockSpeedTempString);
                            if (clockSpeedTempInt < 0)
                            {
                                clockDelay = 0;
                            }
                            else if (clockSpeedTempInt > 1000)
                            {
                                clockDelay = 1000;
                            }
                            else
                            {
                                clockDelay = clockSpeedTempInt;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error! Unable to convert input to int. Enter integer between 0 and 1000");
                        }
                        break;
                    case '5':
                        //Reset all registers/values, preserving RAM
                        RegisterA.value = 0;
                        RegisterB.value = 0;
                        FlagsRegister.value = 0;
                        OutputRegister.value = 0;
                        MemoryAddressRegister.value = 0;
                        InstructionRegister.value = 0;
                        CounterRegister.value = 0;
                        controlSignals = 0;
                        BUS.write(0);
                        break;
                    default:
                        Console.WriteLine("Error! Unknown command. Try again");
                        break;
                }
            } while (!mainResponse.ToString().ToLower().StartsWith('q'));
        }

        void printDebug()
        {
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("\tBus:                  {0} [{1}]", formatBytes(readBus(), 8), Convert.ToInt32(readBus()));
            Console.WriteLine("\tRegisterA:            {0} [{1}]", formatBytes(RegisterA.value, 8), Convert.ToInt32(RegisterA.value));
            Console.WriteLine("\tRegisterB:            {0} [{1}]", formatBytes(RegisterB.value, 8), Convert.ToInt32(RegisterB.value));
            Console.WriteLine("\tSum:                  {0} [{1}]", formatBytes(ALU.sum(), 8), Convert.ToInt32(ALU.sum()));
            Console.WriteLine("\tProgram Counter:      {0} [{1}]", formatBytes(CounterRegister.value, 4), Convert.ToInt32(CounterRegister.value));
            Console.WriteLine("\tInstruction Step:     {0} [{1}]", formatBytes(CPU.instructionCount, 3), Convert.ToInt32(CPU.instructionCount));
            Console.WriteLine("\tInstruction Register: {0} [{1}]", formatBytes(InstructionRegister.value, 8), CPU.printInstruction(InstructionRegister.value));
            Console.WriteLine("\tControl Signals:      {0} [{1}]", formatBytes(Computer.controlSignals, 16), formatControlSignals());
        }

        public static string formatBytes(int value, int displayLength)
        {
            string result = "";
            for (int i = (displayLength - 1); i >= 0; i--)
            {
                int bitNum = (int)Math.Pow(2, i);
                result += ((bitNum & value) > 0) ? "1" : "0";
            }
            return result;
        }

        public static string formatControlSignals()
        {
            string result = "";
            if ((Computer.controlSignals & Computer.HLT) > 0) result += "HLT|";
            if ((Computer.controlSignals & Computer.MI) > 0) result += "MI|";
            if ((Computer.controlSignals & Computer.RI) > 0) result += "RI|";
            if ((Computer.controlSignals & Computer.RO) > 0) result += "RO|";
            if ((Computer.controlSignals & Computer.IO) > 0) result += "IO|";
            if ((Computer.controlSignals & Computer.II) > 0) result += "II|";
            if ((Computer.controlSignals & Computer.AI) > 0) result += "AI|";
            if ((Computer.controlSignals & Computer.AO) > 0) result += "AO|";
            if ((Computer.controlSignals & Computer.SO) > 0) result += "SO|";
            if ((Computer.controlSignals & Computer.SU) > 0) result += "SU|";
            if ((Computer.controlSignals & Computer.BI) > 0) result += "BI|";
            if ((Computer.controlSignals & Computer.OI) > 0) result += "OI|";
            if ((Computer.controlSignals & Computer.CE) > 0) result += "CE|";
            if ((Computer.controlSignals & Computer.CO) > 0) result += "CO|";
            if ((Computer.controlSignals & Computer.J) > 0) result += "J|";
            if ((Computer.controlSignals & Computer.FI) > 0) result += "FI|";

            //Trim trailing "|"
            if (result.Length > 0) result.Remove(result.Length - 1, 1);

            return result;
        }

        public static void writeBus(byte value)
        {
            BUS.write(value);
        }

        public static byte readBus()
        {
            return BUS.read();
        }

        public void DisplayOutput()
        {
            Console.WriteLine("Output: {0}", OutputRegister.value);
        }

        public void tick()
        {
            //handle all output instructions
            if ((Computer.controlSignals & Computer.RO) > 0) RAM.enableOut();
            if ((Computer.controlSignals & Computer.IO) > 0) InstructionRegister.enableOut(bitSelectorEnum._4LSB);
            if ((Computer.controlSignals & Computer.AO) > 0) RegisterA.enableOut();
            if ((Computer.controlSignals & Computer.SO) > 0) ALU.enableOut();
            if ((Computer.controlSignals & Computer.CO) > 0) CounterRegister.enableOut(bitSelectorEnum._4LSB);

            //handle all input instructions
            if ((Computer.controlSignals & Computer.MI) > 0) MemoryAddressRegister.enableIn(bitSelectorEnum._4LSB);
            if ((Computer.controlSignals & Computer.RI) > 0) RAM.enableIn();
            if ((Computer.controlSignals & Computer.II) > 0) InstructionRegister.enableIn();
            if ((Computer.controlSignals & Computer.AI) > 0) RegisterA.enableIn();
            if ((Computer.controlSignals & Computer.BI) > 0) RegisterB.enableIn();
            if ((Computer.controlSignals & Computer.OI) > 0) { OutputRegister.enableIn(); DisplayOutput(); };

            //All other instructions
            if ((Computer.controlSignals & Computer.HLT) > 0) ; //No need to do anything, the clock won't proceed if this signal is set
            if ((Computer.controlSignals & Computer.SU) > 0) ; //No need to do anything, the ALU reads this at calculation time
            if ((Computer.controlSignals & Computer.CE) > 0) ; //No need to do anything, the Program Counter reads this at tick time
            if ((Computer.controlSignals & Computer.J) > 0) ProgramCounter.jump();
            if ((Computer.controlSignals * Computer.FI) > 0) ; //No need to do anything, the ALU reads the value calculation time

            ProgramCounter.onClockTick(); //Must be called before signal reset for clock enable

            if (debug) printDebug();

            //Reset control signals
            Computer.controlSignals = 0;
            Computer.controlSignals = 0;

            CPU.onClockTick();

            if (debug) printDebug();
        }
    }
}

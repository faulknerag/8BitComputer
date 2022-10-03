using System;

namespace _8BitComputer
{
    internal class CPU
    {
        //Constructors
        public CPU()
        {
            buildMachineCode();
        }

        //Fields
        Register InstructionRegister;
        //byte instruction = 0;
        public byte instructionCount = 0;
        int[,,] machineCodeTranslator = new int[16, 8, 4];

        //Methods
        public void setRegister(Register _register)
        {
            InstructionRegister = _register;
        }

        public void onClockTick()
        {
            //Extract instruction as 4 Most Significant Bits of instruction register
            byte instruction = (byte)((InstructionRegister.value & 0b11110000) >> 4);

            //Translate instruction register (assembly code) into control signals (machine code)
            Computer.controlSignals = (ushort)machineCodeTranslator[instruction, instructionCount, ALU.FlagsRegister.value];

            instructionCount = (byte)(++instructionCount % 8); //increment the count, cycling 0 - 7
        }

        private void buildMachineCode()
        {
            //Just for conciseness
            ushort HLT = Computer.HLT;
            ushort MI = Computer.MI;
            ushort RI = Computer.RI;
            ushort RO = Computer.RO;
            ushort IO = Computer.IO;
            ushort II = Computer.II;
            ushort AI = Computer.AI;
            ushort AO = Computer.AO;
            ushort SO = Computer.SO;
            ushort SU = Computer.SU;
            ushort BI = Computer.BI;
            ushort OI = Computer.OI;
            ushort CE = Computer.CE;
            ushort CO = Computer.CO;
            ushort J = Computer.J;
            ushort FI = Computer.FI;

            //Lookup to convert assembly to machine code 
            int[,] machineCodeTranslatorTemplate = new int[16, 8] {
                {(MI|CO),(RO|II|CE),0,0,0,0,0,0}, //[0] NOP - Get Instruction[Program Counter] into Instruction Register and increment Program Counter
                {(MI|CO),(RO|II|CE),(MI|IO),(RO|AI),0,0,0,0}, //[1] LDA - Load RAM[ARG] into Register A
                {(MI|CO),(RO|II|CE),(MI|IO),(RO|BI),(AI|SO|FI),0,0,0}, //[2] ADD - Add RAM[ARG] to Register A and store in Register A
                {(MI|CO),(RO|II|CE),(MI|IO),(RO|BI),(AI|SO|SU|FI),0,0,0}, //[3] SUB - Sub RAM[ARG] from Register A and store in Register A
                {(MI|CO),(RO|II|CE),(MI|IO),(AO|RI),0,0,0,0}, //[4] STA - Store Register A in RAM[ARG]
                {(MI|CO),(RO|II|CE),(IO|AI),0,0,0,0,0}, //[5] LDI - Write ARG to Register A
                {(MI|CO),(RO|II|CE),(IO|J),0,0,0,0,0}, //[6] JMP - Jump to Instruction Counter ARG
                {(MI|CO),(RO|II|CE),0,0,0,0,0,0}, //[7] JC - Jump to Instruction Counter ARG if the Carry Flag is set
                {(MI|CO),(RO|II|CE),0,0,0,0,0,0}, //[8] JZ - Jump to Instructin Counter ARG if the last sum was 0
                {(MI|CO),(RO|II|CE),0,0,0,0,0,0},
                {(MI|CO),(RO|II|CE),0,0,0,0,0,0},
                {(MI|CO),(RO|II|CE),0,0,0,0,0,0},
                {(MI|CO),(RO|II|CE),0,0,0,0,0,0},
                {(MI|CO),(RO|II|CE),0,0,0,0,0,0},
                {(MI|CO),(RO|II|CE),(AO|OI),0,0,0,0,0}, //[14] OUT - Write Register A to Output Register and display
                {(MI|CO),(RO|II|CE),(HLT),0,0,0,0,0}, //[15] HLT - Stop all processing
            };

            //Duplicate template for each flags option, modifying the conditional jump instances
            for (byte instruction = 0; instruction < 16; instruction++)
            {
                for (byte step = 0; step < 8; step++)
                {
                    for (byte flags = 0; flags < 4; flags++)
                    {
                        //Most values are an exact copy of the template and don't care about the flags
                        machineCodeTranslator[instruction, step, flags] = machineCodeTranslatorTemplate[instruction, step];

                        //Exception: Use JMP [6] instructions on JC [7] when carry flag is set in flags register
                        if (instruction == 7 && ((flags & Computer.Flags_Carry) > 0))
                        {
                            machineCodeTranslator[instruction, step, flags] = machineCodeTranslatorTemplate[6, step];
                        }
                        //Exception: JZ when zero flag is set in flags register
                        if (instruction == 8 && ((flags & Computer.Flags_Zero) > 0))
                        {
                            machineCodeTranslator[instruction, step, flags] = machineCodeTranslatorTemplate[6, step];
                        }
                    }
                }

            }
        }

        public static string printInstruction(byte value)
        {
            string result = "";

            byte instruction = (byte)((value & 0b11110000) >> 4);

            result += Computer.assemblyCodes[instruction];

            result += " " + Convert.ToInt32((value & 0b00001111));

            return result;
        }
    }
}

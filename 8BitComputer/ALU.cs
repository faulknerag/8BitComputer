namespace _8BitComputer
{
    internal class ALU
    {
        //Constructors
        public ALU() { }

        //Fields
        public byte value = 0b0;
        public Register RegisterA;
        public Register RegisterB;
        public static Register FlagsRegister;

        //Methods
        public void enableOut()
        {
            bool subtract = (Computer.controlSignals & Computer.SU) > 0;
            bool carryBit = false;

            if (!subtract)
            {
                carryBit = (RegisterA.value + RegisterB.value) > 255;
                value = (byte)(RegisterA.value + RegisterB.value);
            }
            else
            {
                carryBit = (RegisterA.value - RegisterB.value) < 0;
                value = (byte)(RegisterA.value - RegisterB.value);
            }

            //If Flags In (FI) signal is set, record information about the zero and carry status of the calculation
            if ((Computer.controlSignals & Computer.FI) > 0)
            {
                if (value == 0 && carryBit)
                {
                    FlagsRegister.value = (byte)(Computer.Flags_Zero | Computer.Flags_Carry);
                }
                else if (value == 0 && !carryBit)
                {
                    FlagsRegister.value = (byte)(Computer.Flags_Zero);
                }
                else if (value != 0 && carryBit)
                {
                    FlagsRegister.value = (byte)(Computer.Flags_Carry);
                }
                else
                {
                    FlagsRegister.value = 0;
                }

                //FlagsRegister.value = (byte)((Computer.Flags_Zero * (value == 0 ? 1:0)) | (Computer.Flags_Carry * (carryBit ? 1 : 0))); // Gross
            }

            //Output to Bus
            Computer.writeBus(value);

            //Console.WriteLine("ALU Calculation result: {0}", (byte)value);
        }

        public byte sum()
        {
            bool subtract = (Computer.controlSignals & Computer.SU) > 0;
            byte value;

            if (!subtract)
            {
                value = (byte)(RegisterA.value + RegisterB.value);
            }
            else
            {
                value = (byte)(RegisterA.value - RegisterB.value);
            }

            return value;
        }

        public void setRegisters(Register _registerA, Register _registerB, Register _flagsRegister)
        {
            RegisterA = _registerA;
            RegisterB = _registerB;
            FlagsRegister = _flagsRegister;
        }
    }
}

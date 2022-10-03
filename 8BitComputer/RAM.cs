using System;

namespace _8BitComputer
{
    internal class RAM
    {
        //Constructors
        public RAM() {
            // Initialize RAM to example program - Double up to 128, then restart at 1
            storedBytes[0] = 0b01010001; //LDI 1
            storedBytes[1] = 0b11100000; //OUT
            storedBytes[2] = 0b01001111; //STA 15
            storedBytes[3] = 0b00101111; //ADD 15
            storedBytes[4] = 0b01110000; //JC 0
            storedBytes[5] = 0b11100000; //OUT
            storedBytes[6] = 0b01100010; //JMP 2
        }

        //Fields
        Register MemoryAddressRegister;
        byte[] storedBytes = new byte[16];

        //Methods
        public byte readByte(byte address)
        {
            byte result = storedBytes[address];
            return result;
        }

        public void writeByte(byte address, byte value)
        {
            storedBytes[address] = value;
        }

        public void enableOut()
        {
            byte address = MemoryAddressRegister.value;
            Computer.writeBus(storedBytes[address & 0b00001111]); //Only index 4 Least Significant Digits
        }

        //Write bus value into the current Memory Address Register
        public void enableIn()
        {
            writeByte(MemoryAddressRegister.value, Computer.readBus());
        }

        public void setRegister(Register _register)
        {
            MemoryAddressRegister = _register;
        }

        public void interactiveMemoryWrite()
        {
            Console.WriteLine("Interactive RAM Update");
            Console.WriteLine("(D)isplay; (C)lear; (S)ave;\nADDR:VALUE (e.g. 15:01010101) to write VALUE at ADDR");
            string response;
            do
            {
                Console.Write("Enter Selection: ");
                response = Console.ReadLine();
                if (response.Length == 0) response = "error";
                switch (response.ToLower()[0])
                {
                    case 's':
                        return;
                    case 'd': //Display all memory values
                        for (int i = 0; i < storedBytes.Length; i++)
                        {
                            string step = (i < 10) ? i.ToString() + " " : i.ToString();
                            Console.WriteLine("\t{0}:{1} [{2}]", step, Computer.formatBytes(storedBytes[i], 8), CPU.printInstruction(storedBytes[i])); //Convert.ToString(storedBytes[i], 2)
                        }
                        break;
                    case 'c': //Clear all memory values
                        for (int i = 0; i < storedBytes.Length; i++)
                        {
                            storedBytes[i] = 0;
                        }
                        break;
                    case 'e':
                        Console.WriteLine("Error! Invalid Input, Try Again");
                        break;
                    default:
                        string[] newAddrValue = response.Split(':');
                        try
                        {
                            byte address = Convert.ToByte(newAddrValue[0], 10); //Assumes address is decimal
                            byte value = Convert.ToByte(newAddrValue[1], 2); //Assumes values is binary
                            writeByte(address, value);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine("Error! Invalid Input, Try Again");
                        }
                        break;
                }
            } while (!response.ToString().ToLower().StartsWith('q'));
        }
    }
}

namespace _8BitComputer
{
    internal class Bus
    {
        //Constructors
        public Bus() { }

        //Fields
        private byte value = 0b0;

        //Methods
        public void write(byte _value)
        {
            value = _value;
        }

        public byte read()
        {
            return value;
        }
    }
}

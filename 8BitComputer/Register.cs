namespace _8BitComputer
{
    internal class Register
    {
        //Constructors
        public Register() { }

        //Fields
        private byte _value;

        public byte value { get => _value; set => this._value = value; }

        //Methods
        //Write register value to bus
        public void enableOut(bitSelectorEnum bitSelector = bitSelectorEnum.all)
        {
            switch (bitSelector)
            {
                case bitSelectorEnum.all:
                default:
                    Computer.writeBus(value);
                    break;
                case bitSelectorEnum._4LSB:
                    Computer.writeBus((byte)(value & 0b00001111));
                    break;
                case bitSelectorEnum._4MSB:
                    Computer.writeBus((byte)(value & 0b11110000));
                    break;
            }
        }

        //Read current bus value into register
        public void enableIn(bitSelectorEnum bitSelector = bitSelectorEnum.all)
        {
            switch (bitSelector)
            {
                case bitSelectorEnum.all:
                default:
                    value = Computer.readBus();
                    break;
                case bitSelectorEnum._4LSB:
                    value = (byte)(Computer.readBus() & 0b00001111);
                    break;
                case bitSelectorEnum._4MSB:
                    value = (byte)(Computer.readBus() & 0b11110000);
                    break;
            }
        }
    }
}

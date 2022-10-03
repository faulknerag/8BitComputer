namespace _8BitComputer
{
    internal class ProgramCounter
    {
        //Constructors 
        public ProgramCounter() { }

        //Fields
        Register counterRegister;

        //Methods
        public void setRegister(Register _register)
        {
            counterRegister = _register;
        }

        public void jump()
        {
            counterRegister.enableIn(bitSelectorEnum._4LSB);
        }

        public void onClockTick()
        {
            if ((Computer.controlSignals & Computer.CE) > 0)
            {
                counterRegister.value = (byte)(counterRegister.value + 1);
            }
        }
    }
}

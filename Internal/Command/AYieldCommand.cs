namespace BLK10.Iterator.Command
{
    public abstract class AYieldCommand
    {
        protected bool _processed;

        internal abstract bool OnProcess();
    }
}

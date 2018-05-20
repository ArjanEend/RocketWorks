namespace RocketWorks.Triggers
{
    public abstract class TriggerBase
    {
        private bool blocked = false;
        public bool Blocked
        {
            get { return blocked; }
            set { blocked = value; }
        }

        public abstract void Execute();
    }
}

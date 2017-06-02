namespace WispCloud.Logic
{
    public class BaseManager
    {
        public WispContext Context { get; private set; }

        public BaseManager(WispContext context)
        {
            this.Context = context;
        }

    }

}
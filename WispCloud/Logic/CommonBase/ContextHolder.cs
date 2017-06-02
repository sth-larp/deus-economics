using WispCloud.Logic;

namespace DeusCloud.Logic.CommonBase
{
    public class ContextHolder : IContextHolder
    {
        public UserContext UserContext { get; private set; }

        public ContextHolder(UserContext context)
        {
            this.UserContext = context;
        }

    }

}
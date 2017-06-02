using System;

namespace WispCloudClient.SignalRTypes
{
    public class BaseEventArgs
    {
        public Guid EventID { get; }
        public EventActionType Action { get; set; }

        public BaseEventArgs()
        {
            this.Action = EventActionType.None;
        }

    }

}

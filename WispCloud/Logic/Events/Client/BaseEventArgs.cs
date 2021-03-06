﻿using System;

namespace DeusCloud.Logic.Events.Client
{
    public abstract class BaseEventArgs
    {
        public Guid EventID { get; }
        public EventActionType Action { get; set; }

        public BaseEventArgs()
        {
            this.EventID = Guid.NewGuid();
            this.Action = EventActionType.None;
        }

    }

}
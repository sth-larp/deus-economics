﻿using System;

namespace DeusClient.ApiCommands.CommonBase
{
    public abstract class InputCommand<Input> : BaseCommand, IInputCommand<Input>
    {
        public override Type InputType { get { return typeof(Input); } }
        public override Type OutputType { get { return null; } }

    }

    public interface IInputCommand<Input>
    {
    }

}

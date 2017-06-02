using System;

namespace DeusClient.ApiCommands.CommonBase
{
    public abstract class InputOutputCommand<Input, Output> : BaseCommand, IInputCommand<Input>, IOutputCommand<Output>
    {
        public override Type InputType { get { return typeof(Input); } }
        public override Type OutputType { get { return typeof(Output); } }

    }

}

using System;

namespace DeusCloud.Exceptions
{
    public class DeusException : Exception
    {
        public bool Expected { get; set; }

        public DeusException(bool expected = true)
            : base(string.Empty)
        {
            this.Expected = expected;
        }
        public DeusException(string message, bool expected = true)
            : base(message)
        {
            this.Expected = expected;
        }

        public DeusException(string message, Exception innerException, bool expected = true)
            : base(message, innerException)
        {
            this.Expected = expected;
        }

    }

}
using System;

namespace DeusCloud.Exceptions
{
    public class DeusDuplicateException : DeusException
    {
        public DeusDuplicateException(bool expected = true)
            : base(expected)
        {
        }
        public DeusDuplicateException(string message, bool expected = true)
            : base(message, expected)
        {
        }
        public DeusDuplicateException(string message, Exception innerException, bool expected = true)
            : base(message, innerException, expected)
        {
        }

    }

}
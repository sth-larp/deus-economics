using System;
using System.Net;

namespace DeusCloud.Exceptions
{
    public class DeusHttpException : DeusException
    {
        public HttpStatusCode Status { get; set; }

        public DeusHttpException(HttpStatusCode status, bool expected = true)
            : base(expected)
        {
            this.Status = status;
        }
        public DeusHttpException(HttpStatusCode status, string message, bool expected = true)
            : base(message, expected)
        {
            this.Status = status;
        }

        public DeusHttpException(HttpStatusCode status, string message, Exception innerException, bool expected = true)
            : base(message, innerException, expected)
        {
            this.Status = status;
        }

    }

}
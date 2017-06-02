using System.Net;
using WispCloud;

namespace DeusCloud.Exceptions
{
    public static class Try
    {
        public static void Argument(object obj, string paramName, bool expected = true)
        {
            if (obj == null)
                throw new DeusException($"{paramName} cant be null;", expected);
        }
        public static void ArgumentStr(string str, string paramName, bool expected = true)
        {
            if (string.IsNullOrEmpty(str))
                throw new DeusException($"{paramName} cant be empty or null;", expected);
        }

        public static void Null(object obj, string message, bool expected = true)
        {
            if (obj != null)
                throw new DeusException(message, expected);
        }
        public static void Null(object obj, HttpStatusCode status, bool expected = true)
        {
            if (obj != null)
                throw new DeusHttpException(status, expected);
        }
        public static void NotNull(object obj, string message, bool expected = true)
        {
            if (obj == null)
                throw new DeusException(message, expected);
        }
        public static void NotNull(object obj, HttpStatusCode status, bool expected = true)
        {
            if (obj == null)
                throw new DeusHttpException(status, expected);
        }
        public static void NotEmpty(string str, string message, bool expected = true)
        {
            if (string.IsNullOrEmpty(str))
                throw new DeusException(message, expected);
        }
        public static void NotEmpty(string str, HttpStatusCode status, bool expected = true)
        {
            if (string.IsNullOrEmpty(str))
                throw new DeusHttpException(status, expected);
        }

        public static void InRange(int value, int from, int to, string message, bool expected = true)
        {
            if (value < from || value > to)
                throw new DeusException(message, expected);
        }
        public static void InRange(int value, int from, int to, HttpStatusCode status, bool expected = true)
        {
            if (value < from || value > to)
                throw new DeusHttpException(status, expected);
        }
        public static void InRange(float value, float from, float to, string message, bool expected = true)
        {
            if (value < from || value > to)
                throw new DeusException(message, expected);
        }
        public static void InRange(float value, float from, float to, HttpStatusCode status, bool expected = true)
        {
            if (value < from || value > to)
                throw new DeusHttpException(status, expected);
        }

        public static void Condition(bool conditionResult, string message, bool expected = true)
        {
            if (!conditionResult)
                throw new DeusException(message, expected);
        }
        public static void Condition(bool conditionResult, HttpStatusCode status, bool expected = true)
        {
            if (!conditionResult)
                throw new DeusHttpException(status, expected);
        }

    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeusCloud.Exceptions
{
    [Serializable]
    public class ExceptionPath
    {
        List<object> _path;

        public ExceptionPath()
        {
            this._path = new List<object>();
        }

        public void PrependField(string fieldName)
        {
            _path.Add(fieldName);
        }

        public void PrependIndex(int index)
        {
            _path.Add(index);
        }

        public override string ToString()
        {
            var pathBuilder = new StringBuilder();
            foreach (var pathPart in _path.AsEnumerable().Reverse())
            {
                if (pathPart is string)
                {
                    if (pathBuilder.Length > 0)
                        pathBuilder.Append('.');
                    pathBuilder.Append(pathPart);
                }
                else if (pathPart is int)
                {
                    pathBuilder.Append('[').Append(pathPart).Append(']');
                }
            }
            return pathBuilder.ToString();
        }

    }

    public static class ExceptionPathExtensions
    {
        static string ExceptionPathKey { get; }

        static ExceptionPathExtensions()
        {
            ExceptionPathKey = "wisp.Path";
        }

        public static bool ContainsPath(this Exception exception)
        {
            var path = (exception.Data[ExceptionPathKey] as ExceptionPath);
            return (path != null);
        }

        public static ExceptionPath GetOrCreatePath(this Exception exception)
        {
            var path = (exception.Data[ExceptionPathKey] as ExceptionPath);
            if (path == null)
            {
                path = new ExceptionPath();
                exception.Data[ExceptionPathKey] = path;
            }
            return path;
        }

        public static void PrependField(this Exception exception, string fieldName)
        {
            var path = exception.GetOrCreatePath();
            path.PrependField(fieldName);
        }

        public static void PrependIndex(this Exception exception, int index)
        {
            var path = exception.GetOrCreatePath();
            path.PrependIndex(index);
        }

    }

}
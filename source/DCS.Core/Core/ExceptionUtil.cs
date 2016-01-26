using System;
using System.CodeDom.Compiler;
using System.IO;

// ReSharper disable once CheckNamespace
namespace DCS.Core
{
    public static class ExceptionUtil
    {
        public static string Summary(this Exception ex)
        {
            var stringWriter = new StringWriter();
            var writer = new IndentedTextWriter(stringWriter);
            Exception current = ex;
            while (current != null)
            {
                writer.WriteLine("({0}) {1}", current.GetType().Name, current.Message);
                writer.Indent++;
                current = current.InnerException;
            }
            return stringWriter.ToString();
        }
    }
}
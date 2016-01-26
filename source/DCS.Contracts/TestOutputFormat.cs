using System;

namespace DCS.Contracts
{
    public enum TestOutputFormat
    {
        Invalid = 0,
        Text,
        Html,
        Json
    }

    public static class TestOutputFormatExt
    {
        public static string ToMimeType(this TestOutputFormat format)
        {
            switch (format)
            {
                case TestOutputFormat.Text:
                    return "text/plain";
                case TestOutputFormat.Html:
                    return "text/html";
                case TestOutputFormat.Json:
                    return "application/json";
                default:
                    throw new ArgumentOutOfRangeException("format");
            }
        }
    }
}
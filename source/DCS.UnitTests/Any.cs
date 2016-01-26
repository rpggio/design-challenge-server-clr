using System;
using System.IO;

namespace DCS.UnitTests
{
    public static class Any
    {
        private static Random _random = new Random();

        public static string Name()
        {
            return Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
        }

        public static string Name(string prefix)
        {
            return string.Format("{0}_{1}", prefix, Name());
        }

        public static string Email()
        {
            return Email(Name());
        }

        public static string Email(string username)
        {
            return string.Format("{0}@localhost", username);
        }
    }
}
using System;
using System.IO;
using System.Reflection;

namespace Share
{
    public static class Folder
    {
        public static string GetCurrentDir()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }
    }
}

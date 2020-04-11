using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PDF2TIF.Utils
{
    class FileUtils
    {
        internal static void DeleteFileIfExists(string file)
        {
            if (File.Exists(file))
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
        }
    }
}

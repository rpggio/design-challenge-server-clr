using System;
using System.IO;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace DCS.Core
{
    public class FileUtil
    {
        /// <summary>
        ///     Recursively copy directory contents into another directory, merging into existing directories.
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="overwrite"></param>
        public static void CopyDirectoryContents(string sourceDirName, string destDirName, bool overwrite,
            Action<string, string> copyListener = null)
        {
            if (copyListener == null)
            {
                copyListener = delegate { };
            }

            if (string.IsNullOrWhiteSpace(sourceDirName))
                throw new ArgumentException("Argument is empty", "sourceDirName");
            if (string.IsNullOrWhiteSpace(destDirName)) throw new ArgumentException("Argument is empty", "destDirName");

            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            Directory.CreateDirectory(destDirName);
            var files = dir.GetFiles();
            var dirs = dir.GetDirectories();

            foreach (var file in files)
            {
                copyListener(file.ToString(), Path.Combine(destDirName, file.Name));
                file.CopyTo(Path.Combine(destDirName, file.Name), overwrite);
            }

            foreach (DirectoryInfo sub in dirs)
            {
                CopyDirectoryContents(sub.FullName, Path.Combine(destDirName, sub.Name), overwrite);
            }
        }

        /// <summary>
        ///     Depth-first recursive delete, with handling for descendant
        ///     directories open in Windows Explorer.
        /// </summary>
        public static void DeleteDirectory(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Thread.Sleep(100);
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Thread.Sleep(100);
                Directory.Delete(path, true);
            }
        }

        public static string NormalizePath(string path)
        {
            if (path == null)
            {
                return null;
            }
            return Path.GetFullPath(new Uri(path).LocalPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        public static bool FilesContentsAreEqual(FileInfo fileInfo1, FileInfo fileInfo2)
        {
            bool result;

            if (!fileInfo1.Exists || !fileInfo2.Exists)
            {
                return false;
            }

            if (fileInfo1.Length != fileInfo2.Length)
            {
                result = false;
            }
            else
            {
                using (var file1 = fileInfo1.OpenRead())
                {
                    using (var file2 = fileInfo2.OpenRead())
                    {
                        result = StreamsContentsAreEqual(file1, file2);
                    }
                }
            }

            return result;
        }

        public static bool StreamsContentsAreEqual(Stream stream1, Stream stream2)
        {
            const int bufferSize = 2048*2;
            var buffer1 = new byte[bufferSize];
            var buffer2 = new byte[bufferSize];

            while (true)
            {
                int count1 = stream1.Read(buffer1, 0, bufferSize);
                int count2 = stream2.Read(buffer2, 0, bufferSize);

                if (count1 != count2)
                {
                    return false;
                }

                if (count1 == 0)
                {
                    return true;
                }

                int iterations = (int) Math.Ceiling((double) count1/sizeof (Int64));
                for (int i = 0; i < iterations; i++)
                {
                    if (BitConverter.ToInt64(buffer1, i*sizeof (Int64)) !=
                        BitConverter.ToInt64(buffer2, i*sizeof (Int64)))
                    {
                        return false;
                    }
                }
            }
        }
    }
}
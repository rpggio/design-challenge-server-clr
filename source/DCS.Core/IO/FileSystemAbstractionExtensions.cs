using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DCS.Core.IO
{
    public static class FileSystemAbstractionExtensions
    {
        public static IDirectory Directory(this IDirectory parent, string name)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (name == null) throw new ArgumentNullException("name");
            return new PathDirectory(Path.Combine(parent.Path, name));
        }

        public static IFile File(this IDirectory parent, string path)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (path == null) throw new ArgumentNullException("path");
            return new PathFile(Path.Combine(parent.Path, path));
        }

        public static void CopyContentsInto(this IDirectory source, IDirectory target, bool overwrite = true)
        {
            FileUtil.CopyDirectoryContents(source.Path, target.Path, overwrite);
        }

        public static void CopyContentsInto(this IDirectory source, string target, bool overwrite = true)
        {
            FileUtil.CopyDirectoryContents(source.Path, target, overwrite);
        }

        public static void CopyFilesInfo(this IDirectory source, IDirectory target, bool overwrite = true)
        {
            foreach (var file in source.Files())
            {
                file.CopyInto(target, overwrite);
            }
        }

        public static bool Exists(this IFile file)
        {
            return System.IO.File.Exists(file.Path);
        }

        public static bool Exists(this IDirectory directory)
        {
            return System.IO.Directory.Exists(directory.Path);
        }

        public static bool IsEmpty(this IDirectory directory)
        {
            return !System.IO.Directory.EnumerateFileSystemEntries(directory.Path).Any();
        }

        public static DirectoryInfo Create(this IDirectory directory)
        {
            return System.IO.Directory.CreateDirectory(directory.Path);
        }

        public static IEnumerable<IDirectory> Directories(this IDirectory directory)
        {
            return System.IO.Directory.EnumerateDirectories(directory.Path)
                .Select(d => new PathDirectory(d));
        }

        public static IEnumerable<IDirectory> Directories(this IDirectory directory, string pattern)
        {
            return System.IO.Directory.EnumerateDirectories(directory.Path, pattern)
                .Select(d => new PathDirectory(d));
        }

        public static IEnumerable<IFile> Files(this IDirectory directory)
        {
            return System.IO.Directory.EnumerateFiles(directory.Path)
                .Select(d => new PathFile(d));
        }

        /// <summary>
        /// Recursively walk all of the subdirectories and files, breadth-first.
        /// </summary>
        public static IEnumerable<IFileSystemItem> Recurse(this IDirectory directory)
        {
            foreach (var file in Files(directory))
            {
                yield return file;
            }

            foreach (var subDir in Directories(directory))
            {
                yield return subDir;
                foreach (var item in Recurse(subDir))
                {
                    yield return item;
                }
            }
        }

        public static IFile CopyTo(this IFile file, IFile target, bool overwrite = true)
        {
            System.IO.File.Copy(file.Path, target.Path, overwrite);
            return target;
        }

        public static IFile CopyInto(this IFile file, IDirectory target, bool overwrite = true)
        {
            return CopyTo(file, target.File(file.Name()));
        }

        public static void Delete(this IDirectory directory)
        {
            FileUtil.DeleteDirectory(directory.Path);
        }

        public static void DeleteFiles(this IDirectory directory)
        {
            foreach (var file in directory.Files())
            {
                file.Delete();
            }
        }

        public static void DeleteContents(this IDirectory directory)
        {
            if (!directory.Exists())
            {
                return;
            }

            foreach (var dir in directory.Directories())
            {
                dir.Delete();
            }

            foreach (var file in directory.Files())
            {
                file.Delete();
            }
        }

        public static void Delete(this IFile file)
        {
            System.IO.File.Delete(file.Path);
        }

        /// <summary>
        /// The name of the file itself.
        /// </summary>
        public static string Name(this IFileSystemItem item)
        {
            return Path.GetFileName(item.Path);
        }

        /// <summary>
        /// Return parent directory, or null if argument is root directory or null.
        /// </summary>
        public static IDirectory Parent(this IFileSystemItem item)
        {
            if (item == null)
            {
                return null;
            }
            var directoryName = Path.GetDirectoryName(item.Path);
            return directoryName == null
                ? null
                : new PathDirectory(directoryName);
        }

        public static string AbsolutePath(this IFileSystemItem item)
        {
            return Path.GetFullPath(item.Path);
        }

        public static string PathRelativeFrom(this IFileSystemItem item, IDirectory baseDirectory)
        {
            if (item == null || baseDirectory == null)
            {
                return null;
            }

            var relativePathParts = new Stack<string>();
            var current = item;
            while (!current.PathEquals(baseDirectory))
            {
                var parent = current.Parent();
                if (parent == null)
                {
                    throw new Exception("Could not make relative path");
                }

                relativePathParts.Push(current.Name());
                current = parent;
            }
         
            return Path.Combine(relativePathParts.ToArray());
        }

        public static bool PathEquals(this IFileSystemItem item, IFileSystemItem compareTo)
        {
            return string.Equals(item.AbsolutePath(), compareTo.AbsolutePath(), StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContentsAreEqualTo(this IFile file, IFile compareTo)
        {
            return FileUtil.FilesContentsAreEqual(file.Info(), compareTo.Info());
        }

        public static StreamReader OpenReadText(this IFile file)
        {
            return System.IO.File.OpenText(file.Path);
        }

        public static FileStream OpenRead(this IFile file)
        {
            return System.IO.File.OpenRead(file.Path);
        }

        public static string ReadAllText(this IFile file)
        {
            return System.IO.File.ReadAllText(file.Path);
        }

        public static void WriteAllText(this IFile file, string text)
        {
            System.IO.File.WriteAllText(file.Path, text);
        }

        public static FileInfo Info(this IFile file)
        {
            return new FileInfo(file.Path);
        }

        public static DirectoryInfo Info(this IDirectory directory)
        {
            return new DirectoryInfo(directory.Path);
        }
    }
}
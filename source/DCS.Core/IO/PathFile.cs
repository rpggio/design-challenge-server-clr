namespace DCS.Core.IO
{
    public class PathFile : IFile
    {
        private readonly string _path;

        public PathFile(params string[] parts)
            : this(System.IO.Path.Combine(parts))
        {
        }

        public PathFile(string path)
        {
            _path = FileUtil.NormalizePath(path);
        }

        public string Path
        {
            get { return _path; }
        }

        public override string ToString()
        {
            return _path;
        }
    }
}
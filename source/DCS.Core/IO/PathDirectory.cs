namespace DCS.Core.IO
{
    public class PathDirectory : IDirectory
    {
        private readonly string _path;

        public PathDirectory(string path)
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
using DCS.Core;
using DCS.Core.IO;
using DCS.ServerRuntime.Framework;

namespace DCS.ServerRuntime.Services
{
    [RegisterComponent(RegisterWith.SingleInstance)]
    public class DcsShell
    {
        private readonly Shell _shell;

        public DcsShell(Shell shell)
        {
            _shell = shell;
        }

        public bool ZipDirectoryContents(IDirectory source, IFile target)
        {
            return _shell.RunBackground("7z", string.Format(@"a -tzip {0} {1}\*", target.Path, source.Path)) == 0;
        }

    }
}
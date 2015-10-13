using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MonoTorrent.Common;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;
using Alphaleonis.Win32.Filesystem;
using System.Reflection;

namespace MonoTorrent.Client
{
    internal class TorrentFileStream : FileStream
    {
        TorrentFile file;

        public TorrentFile File
        {
            get { return file; }
        }

        public string Path
        {
            get { return file.FullPath; }
        }

        static MethodInfo _createFileCore;

        static SafeFileHandle CreateHandle(string path, FileMode mode, FileAccess access, FileShare share)
        {
            if (_createFileCore == null)
            {
                _createFileCore = typeof(Alphaleonis.Win32.Filesystem.File).GetMethod("CreateFileCore", BindingFlags.Static | BindingFlags.NonPublic);
                if (_createFileCore == null)
                    _createFileCore = typeof(Alphaleonis.Win32.Filesystem.File).GetMethod("CreateFileInternal", BindingFlags.Static | BindingFlags.NonPublic);
            }

            FileSystemRights rights = access == FileAccess.Read ? FileSystemRights.Read : (access == FileAccess.Write ? FileSystemRights.Write : FileSystemRights.Read | FileSystemRights.Write);

            return (SafeFileHandle)_createFileCore.Invoke(null, new object[] { null, path, ExtendedFileAttributes.Normal, null, mode, rights, share, true, PathFormat.RelativePath
});
        }

        public TorrentFileStream(TorrentFile file, FileMode mode, FileAccess access, FileShare share)
           : base(CreateHandle(file.FullPath, mode, access, share), access, 1)
        {
            this.file = file;
        }
    }
}

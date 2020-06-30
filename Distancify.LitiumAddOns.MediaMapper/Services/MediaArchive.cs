using System;
using System.Collections.Generic;
using Litium.Media;

namespace Distancify.LitiumAddOns.MediaMapper.Services
{
    public abstract class MediaArchive
    {   
        public abstract string DefaultFolderTemplate { get; }
        public abstract File GetFile(Guid fileId);
        public abstract void EnsureFolderExists(string path);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">The path to the folder (folder names separated by forward slash), with or without leading or trailing slash</param>
        /// <returns>An instance of the folder</returns>
        /// <exception cref="FolderNotFoundException"></exception>
        public abstract Folder GetFolder(string path);
        public abstract IEnumerable<File> GetFiles(Folder folder, bool includeSubFolders);
        public abstract void MoveFile(Guid fileId, Folder targetFolder);
        public abstract void SaveChanges(File file);

        public class FolderNotFoundException : Exception { }
    }
}

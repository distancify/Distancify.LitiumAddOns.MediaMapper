using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Data;
using Litium.FieldFramework;
using Litium.Media;
using Litium.Media.Queryable;
using Litium.Runtime.DependencyInjection;

namespace Distancify.LitiumAddOns.MediaMapper.Services
{
    [Service(
        ServiceType = typeof(MediaArchive),
        Lifetime = DependencyLifetime.Scoped)]
    public class MediaArchiveImpl : MediaArchive
    {
        private const string DefaultFolderTemplate = "DefaultFolderTemplate";

        private readonly FolderService _folderService;
        private readonly FieldTemplateService _fieldTemplateService;
        private readonly FileService _fileService;
        private readonly DataService _dataService;
        private readonly Folder _rootFolder;
        
        public MediaArchiveImpl(FolderService folderService,
            FieldTemplateService fieldTemplateService,
            FileService fileService,
            DataService dataService)
        {
            _folderService = folderService;
            _fieldTemplateService = fieldTemplateService;
            _fileService = fileService;
            _dataService = dataService;
            _rootFolder = new RootFolder(fieldTemplateService);
        }

        public override void EnsureFolderExists(string path)
        {
            Folder current = _rootFolder;

            foreach (var folder in SplitPath(path))
            {
                var child = GetChildIfExists(folder, current);
                if (child != null)
                {
                    current = child;
                    continue;
                }

                current = CreateFolder(current.SystemId, folder, current.FieldTemplateSystemId);
            }
        }

        private string[] SplitPath(string path)
        {
            return path.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private Folder GetChildIfExists(string name, Folder parent)
        {
            var childFolders = _folderService.GetChildFolders(parent.SystemId);
            if (childFolders.Any(f => string.Equals(f.Name, name, StringComparison.InvariantCultureIgnoreCase)))
            {
                return childFolders.Single(r => r.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            }
            return null;
        }

        private Folder CreateFolder(Guid parentId, string name, Guid folderTemplateId)
        {
            var f = new Folder(folderTemplateId, name)
            {
                ParentFolderSystemId = parentId,
                SystemId = Guid.NewGuid()
            };

            _folderService.Create(f);

            return f;
        }

        public override Folder GetFolder(string path)
        {
            Folder current = _rootFolder;

            foreach (var folder in SplitPath(path))
            {
                var child = GetChildIfExists(folder, current);
                current = child ?? throw new FolderNotFoundException();
            }

            return current;
        }

        public override File GetFile(Guid fileId)
        {
            return _fileService.Get(fileId);
        }
        
        public override IEnumerable<File> GetFiles(Folder folder, bool includeSubFolders)
        {
            if(folder == null)
            {
                yield break;
            }

            using (var query = _dataService.CreateQuery<File>())
            {
                query.Filter(descriptor => FileFilterDescriptorExtensions.FolderSystemId(descriptor, folder.SystemId));

                foreach(var fileId in query.ToSystemIdList())
                {
                    yield return _fileService.Get(fileId);
                }
            }

            if (includeSubFolders)
            {
                var childFolders = _folderService.GetChildFolders(folder.SystemId);
                foreach (var childFolder in childFolders)
                {
                    foreach (var file in GetFiles(childFolder, true))
                    {
                        yield return file;
                    }
                }
            }
        }

        public override void MoveFile(Guid fileId, Folder targetFolder)
        {
            var file = _fileService.Get(fileId);

            if (ReferenceEquals(file, null)) throw new ArgumentNullException("fileId");
            if (ReferenceEquals(targetFolder, null)) throw new ArgumentNullException("folder");
            
            var existingFiles = GetFiles(targetFolder, false).Where(
                    r => r.Name.Equals(file.Name, StringComparison.InvariantCultureIgnoreCase) && !r.SystemId.Equals(file.SystemId)).ToList();
            
            foreach (var e in existingFiles)
            {
                _fileService.Delete(e);
            }

            file = file.MakeWritableClone();
            file.FolderSystemId = targetFolder.SystemId;
            _fileService.Update(file);
        }

        public override void SaveChanges(File file)
        {
            _fileService.Update(file);
        }

        private class RootFolder : Folder
        {
            private FieldTemplateService _fieldTemplateService;

            public RootFolder(FieldTemplateService fieldTemplateService)
            {
                _fieldTemplateService = fieldTemplateService;
            }

            public override Guid SystemId { get => Guid.Empty; }

            public override Guid FieldTemplateSystemId { get => _fieldTemplateService.Get<FolderFieldTemplate>(DefaultFolderTemplate).SystemId; }
        }
    }
}
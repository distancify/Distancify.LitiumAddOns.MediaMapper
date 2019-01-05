using System;
using System.Collections.Generic;
using System.Linq;
using Litium.FieldFramework;
using Litium.Runtime.DependencyInjection;
using Litium.Media;

namespace Distancify.LitiumAddOns.MediaMapper.Services.FieldSetters
{
    [Service(
        ServiceType = typeof(IFieldSetter),
        Lifetime = DependencyLifetime.Singleton)]
    public class MediaPointerImageArrayFieldSetter : IFieldSetter
    {
        private readonly MediaArchive _mediaArchive;

        public MediaPointerImageArrayFieldSetter(MediaArchive mediaArchive)
        {
            _mediaArchive = mediaArchive;
        }

        /// <summary>
        /// Override to provide custom sorting rules to images
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        protected virtual IEnumerable<File> Sort(IEnumerable<File> files)
        {
            return files;
        } 

        public void Set(FieldContainer entity, IFieldDefinition field, File file)
        {
            var images = entity.GetValue<List<Guid>>(field.Id)?
                .Select(r => _mediaArchive.GetFile(r))
                .Where(r => r != null)
                .ToList() ?? new List<File>();

            images.RemoveAll(r => string.Equals(r.Name, file.Name, StringComparison.OrdinalIgnoreCase));
            images.Add(_mediaArchive.GetFile(file.SystemId));

            images = Sort(images).ToList();

            entity.AddOrUpdateValue(field.Id, images.Select(r => r.SystemId).ToList());
        }

        public bool CanSet(IFieldDefinition field)
        {
            return field.FieldType.Equals(SystemFieldTypeConstants.MediaPointerImage + "Array", StringComparison.Ordinal);
        }
    }
}

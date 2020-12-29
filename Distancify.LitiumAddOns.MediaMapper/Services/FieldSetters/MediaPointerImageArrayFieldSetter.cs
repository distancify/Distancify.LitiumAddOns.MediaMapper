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

        public static Action<FieldContainer, IFieldDefinition, List<File>> Sort { get; set; }

        public void Set(FieldContainer entity, IFieldDefinition field, File file)
        {
            var images = entity.GetValue<List<Guid>>(field.Id)?
                .Select(r => _mediaArchive.GetFile(r))
                .Where(r => r != null)
                .ToList() ?? new List<File>();

            if (images.Any(r => r.SystemId == file.SystemId))
            {
                // Field already contain this file. Don't do anything.
                return;
            }

            images.RemoveAll(r => string.Equals(r.Name, file.Name, StringComparison.OrdinalIgnoreCase));
            images.Add(_mediaArchive.GetFile(file.SystemId));

            Sort?.Invoke(entity, field, images);

            entity.AddOrUpdateValue(field.Id, images.Select(r => r.SystemId).ToList());
        }

        public bool CanSet(IFieldDefinition field)
        {
            return field.FieldType.Equals(SystemFieldTypeConstants.MediaPointerImage + "Array", StringComparison.Ordinal);
        }
    }
}

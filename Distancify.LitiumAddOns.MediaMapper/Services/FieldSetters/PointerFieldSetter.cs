using System;
using System.Collections.Generic;
using System.Linq;
using Distancify.SerilogExtensions;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Media;
using Litium.Runtime.DependencyInjection;

namespace Distancify.LitiumAddOns.MediaMapper.Services.FieldSetters
{
    [Service(
        ServiceType = typeof(IFieldSetter),
        Lifetime = DependencyLifetime.Singleton)]
    public class PointerFieldSetter : IFieldSetter
    {
        private readonly MediaArchive _mediaArchive;

        public PointerFieldSetter(MediaArchive mediaArchive)
        {
            _mediaArchive = mediaArchive;
        }

        public static Action<FieldContainer, IFieldDefinition, List<File>> Sort { get; set; }

        public void Set(FieldContainer entity, IFieldDefinition field, File file)
        {
            var options = field.Option as Litium.FieldFramework.FieldTypes.PointerOption;
            var pointerType = GetTypeFor(options.EntityType);
            if (!pointerType.IsInstanceOfType(file))
            {
                throw new Exception($"An object of type {file.GetType()} can not be set to a pointer ({field.Id}) of type {options.EntityType}");
            }

            if (options.MultiSelect)
            {
                var files = entity.GetValue<List<PointerItem>>(field.Id)?
                    .Select(r => _mediaArchive.GetFile(r.EntitySystemId))
                    .Where(r => r != null)
                    .ToList() ?? new List<File>();

                files.RemoveAll(r => string.Equals(r.Name, file.Name, StringComparison.OrdinalIgnoreCase));
                files.Add(_mediaArchive.GetFile(file.SystemId));

                Sort?.Invoke(entity, field, files);

                entity.AddOrUpdateValue(field.Id, files.Select(r => new PointerItem() { EntitySystemId = r.SystemId }).ToList());
            }
            else
            {
                entity.AddOrUpdateValue(field.Id, new PointerItem() { EntitySystemId = file.SystemId });
            }
        }

        private Type GetTypeFor(string pointerType)
        {
            switch (pointerType)
            {
                case PointerTypeConstants.MediaFile:
                case PointerTypeConstants.MediaImage:
                case PointerTypeConstants.MediaVideo:
                    return typeof(File);
                default:
                    return typeof(Guid);
            }
        }

        public bool CanSet(IFieldDefinition field)
        {
            return field.FieldType.Equals(SystemFieldTypeConstants.Pointer, StringComparison.Ordinal);
        }
    }
}

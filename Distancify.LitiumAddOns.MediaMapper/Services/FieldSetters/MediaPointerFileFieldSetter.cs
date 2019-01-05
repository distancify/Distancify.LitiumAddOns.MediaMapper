using System;
using Litium.FieldFramework;
using Litium.Media;
using Litium.Runtime.DependencyInjection;

namespace Distancify.LitiumAddOns.MediaMapper.Services.FieldSetters
{
    [Service(
        ServiceType = typeof(IFieldSetter),
        Lifetime = DependencyLifetime.Singleton)]
    public class MediaPointerFileFieldSetter : IFieldSetter
    {
        public void Set(FieldContainer entity, IFieldDefinition field, File file)
        {
            entity.AddOrUpdateValue(field.Id, file.SystemId);
        }

        public bool CanSet(IFieldDefinition field)
        {
            return field.FieldType.Equals(SystemFieldTypeConstants.MediaPointerFile, StringComparison.Ordinal);
        }
    }
}

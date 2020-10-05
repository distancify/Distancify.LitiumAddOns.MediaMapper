using System;
using Litium.FieldFramework;
using Litium.Media;
using Litium.Runtime.DependencyInjection;

namespace Distancify.LitiumAddOns.MediaMapper.Services.FieldSetters
{
    [Service(ServiceType = typeof(IFieldSetter))]
    public class MediaPointerImageFieldSetter : IFieldSetter
    {
        public bool CanSet(IFieldDefinition field)
        {
            return field.FieldType.Equals(SystemFieldTypeConstants.MediaPointerImage, StringComparison.Ordinal);
        }

        public void Set(FieldContainer entity, IFieldDefinition field, File file)
        {
            entity.AddOrUpdateValue(field.Id, file.SystemId);
        }
    }
}

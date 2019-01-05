using Litium.FieldFramework;
using Litium.Media;

namespace Distancify.LitiumAddOns.MediaMapper
{
    public interface IFieldSetter
    {
        bool CanSet(IFieldDefinition field);
        void Set(FieldContainer entity, IFieldDefinition field, File file);
    }
}

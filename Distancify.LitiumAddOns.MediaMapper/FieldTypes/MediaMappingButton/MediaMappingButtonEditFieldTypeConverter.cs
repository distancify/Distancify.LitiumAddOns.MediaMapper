using Litium.Runtime.DependencyInjection;
using Litium.Web.Administration.FieldFramework;
using Newtonsoft.Json.Linq;

namespace Distancify.LitiumAddOns.MediaMapper.FieldTypes.MediaMappingButton
{
    [Service(Name = FieldConstants.FieldTypes.MediaMappingButton)]
    public class MediaMappingButtonEditFieldTypeConverter : IEditFieldTypeConverter
    {
        public string EditControllerName => null;
        public string EditControllerTemplate => null;
        public string SettingsControllerName => null;
        public string SettingsControllerTemplate => null;

        public string EditComponentName => "MediaMapper#MediaMappingButton";
        public string SettingsComponentName => string.Empty;

        public object CreateOptionsModel() => null;

        public object ConvertFromEditValue(EditFieldTypeConverterArgs args, JToken item)
        {
            return string.Empty;
        }

        public JToken ConvertToEditValue(EditFieldTypeConverterArgs args, object item)
        {
            return new JValue(string.Empty);
        }
    }
}

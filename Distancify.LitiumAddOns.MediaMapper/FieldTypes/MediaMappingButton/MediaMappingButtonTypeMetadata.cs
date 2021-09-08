using System;
using System.Collections.Generic;
using Litium.FieldFramework;

namespace Distancify.LitiumAddOns.MediaMapper.FieldTypes.MediaMappingButton
{
    public class MediaMappingButtonTypeMetadata : FieldTypeMetadataBase
    {
        public override string Id => FieldConstants.FieldTypes.MediaMappingButton;
        public override bool IsArray => false;
        public override Type JsonType => typeof(string);

        public override IFieldType CreateInstance(IFieldDefinition fieldDefinition)
        {
            var item = new MediaMappingButtonType();
            item.Init(fieldDefinition);
            return item;
        }

        public class MediaMappingButtonType : FieldTypeBase
        {
            public override object GetValue(ICollection<FieldData> fieldDatas)
            {
                return string.Empty;
            }

            public override ICollection<FieldData> PersistFieldData(object item)
            {
                return PersistFieldDataInternal(item);
            }

            protected override ICollection<FieldData> PersistFieldDataInternal(object item)
            {
                return new[] { new FieldData { } };
            }

            public override object ConvertFromJsonValue(object item)
            {
                return string.Empty;
            }

            public override object ConvertToJsonValue(object item)
            {
                return string.Empty;
            }
        }
    }
}

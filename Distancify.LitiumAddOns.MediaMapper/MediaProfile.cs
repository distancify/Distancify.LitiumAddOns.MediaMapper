using System.Collections.Immutable;
using Litium.Media;

namespace Distancify.LitiumAddOns.MediaMapper
{
    public class MediaProfile
    {
        public readonly File File;
        public readonly ImmutableDictionary<string, object> Metadata;
        public readonly string FieldId;
        public readonly (string productId, bool isVariant)[] Products;

        /// <summary>
        /// Path to media archive folder relative to archive folder in configuration
        /// </summary>
        public readonly string ArchivePath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="metadata"></param>
        /// <param name="type"></param>
        /// <param name="archivePath">Path to media archive folder relative to archive folder in configuration</param>
        public MediaProfile(File file,
            (string productId, bool isVariant)[] products,
            string fieldId,
            ImmutableDictionary<string, object> metadata,
            string archivePath = null)
        {
            File = file;
            Products = products;
            FieldId = fieldId;
            Metadata = metadata;
            ArchivePath = archivePath;
        }
    }
}
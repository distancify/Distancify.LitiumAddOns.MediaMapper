using System.Collections.Immutable;
using Distancify.LitiumAddOns.MediaMapper.Models;
using Litium.Media;

namespace Distancify.LitiumAddOns.MediaMapper
{
    public class MediaProfile
    {
        public readonly File File;
        public readonly ImmutableDictionary<string, object> Fields;
        public readonly IImmutableList<MediaEntityMapping> MediaEntityMappings;

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
        internal MediaProfile(File file,
            IImmutableList<MediaEntityMapping> mediaEntityMappings,
            ImmutableDictionary<string, object> fields,
            string archivePath)
        {
            File = file;
            Fields = fields;
            ArchivePath = archivePath;
            MediaEntityMappings = mediaEntityMappings;
        }
    }
}
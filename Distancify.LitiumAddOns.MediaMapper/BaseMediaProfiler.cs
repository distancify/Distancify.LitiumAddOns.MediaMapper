using System.Linq;
using Litium.Media;

namespace Distancify.LitiumAddOns.MediaMapper
{
    public abstract class BaseMediaProfiler : IMediaProfiler
    {
        protected abstract MediaProfile CreateMediaProfile(File file);

        public MediaProfile GetMediaProfile(MediaProfileBuilder builder)
        {
            var profile = CreateMediaProfile(builder.File);

            if (profile == null)
            {
                return null;
            }

            var archivePath = profile.ArchivePath;
            if (profile.MediaEntityMappings.Any())
            {
                var productId = profile.MediaEntityMappings.Select(r => r.EntityId).OrderBy(r => r).First();
                archivePath = archivePath ?? string.Format("{0}/{1}", productId.Substring(productId.Length - 2), productId);

                foreach (var entity in profile.MediaEntityMappings)
                {
                    builder.MapTo(entity.EntityType, entity.EntityId, entity.FieldId);
                }
            }

            foreach (var field in profile.Fields)
            {
                builder.SetField(field.Key, field.Value);
            }

            builder.ArchiveTo(archivePath);
            return builder.Create();
        }

        public abstract bool HasMatchingProfile(string fileName);
    }
}
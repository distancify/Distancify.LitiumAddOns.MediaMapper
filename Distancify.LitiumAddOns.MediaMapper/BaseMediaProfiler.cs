using System.Linq;
using Litium.Media;

namespace Distancify.LitiumAddOns.MediaMapper
{
    public abstract class BaseMediaProfiler : IMediaProfiler
    {
        protected abstract MediaProfile CreateMediaProfile(File file);

        public MediaProfile GetMediaProfile(File file)
        {   
            var profile = CreateMediaProfile(file);

            if (profile == null || !profile.Products.Any()) return null;

            var productId = profile.Products.Select(r => r.productId).OrderBy(r => r).First();

            var archivePath = profile.ArchivePath ?? string.Format("{0}/{1}", productId.Substring(productId.Length - 2),
                productId);

            return new MediaProfile(file, profile.Products, profile.FieldId, profile.Metadata, archivePath);
        }

        public abstract bool HasMatchingProfile(string fileName);
    }
}

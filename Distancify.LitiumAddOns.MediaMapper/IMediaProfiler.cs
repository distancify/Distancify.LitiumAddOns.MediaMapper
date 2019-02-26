using Litium.Media;
using Litium.Runtime.DependencyInjection;

namespace Distancify.LitiumAddOns.MediaMapper
{
    [Service(
        ServiceType = typeof(IMediaProfiler),
        Lifetime = DependencyLifetime.Singleton)]
    public interface IMediaProfiler
    {
        /// <summary>
        /// Creates a MediaProfile for a media item
        /// </summary>
        /// <param name="builder">A MediaProfileBuilder instance of the current item</param>
        /// <returns>If null, MediaMapper will leave the media item alone to stay in the upload folder</returns>
        MediaProfile GetMediaProfile(MediaProfileBuilder builder);
    }
}

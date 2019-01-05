using Litium.Media;
using Litium.Runtime.DependencyInjection;

namespace Distancify.LitiumAddOns.MediaMapper
{
    [Service(
        ServiceType = typeof(IMediaProfiler),
        Lifetime = DependencyLifetime.Singleton)]
    public interface IMediaProfiler
    {
        MediaProfile GetMediaProfile(File file);
        bool HasMatchingProfile(string fileName);
    }
}

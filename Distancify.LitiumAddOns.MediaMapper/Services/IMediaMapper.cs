
using Litium.Media;

namespace Distancify.LitiumAddOns.MediaMapper.Services
{
    public interface IMediaMapper
    {
        void Map();
        Folder GetUploadFolder();
    }
}

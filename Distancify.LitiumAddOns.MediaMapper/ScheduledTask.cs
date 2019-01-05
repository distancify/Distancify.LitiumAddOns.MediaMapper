using Distancify.LitiumAddOns.MediaMapper.Services;
using Distancify.LitiumAddOns.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Distancify.LitiumAddOns.MediaMapper
{
    public class ScheduledTask : NonConcurrentTask
    {
        private readonly IList<IMediaMapper> _mediaMappers;

        public ScheduledTask(IEnumerable<IMediaMapper> mediaMappers)
        {
            _mediaMappers = mediaMappers.ToList();
        }
        
        protected override void Run()
        {
            foreach (var m in _mediaMappers)
            {
                m.Map();
            }
        }
    }
}
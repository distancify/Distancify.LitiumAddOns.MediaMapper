using Distancify.LitiumAddOns.MediaMapper.Services;
using Distancify.LitiumAddOns.Tasks;
using System.Collections.Generic;
using System.Linq;
using Litium.Owin.InversionOfControl;

namespace Distancify.LitiumAddOns.MediaMapper
{
    public class ScheduledTask : NonConcurrentTask
    {
        private readonly IIoCContainer _container;

        public ScheduledTask(IIoCContainer container)
        {
            _container = container;
        }
        
        protected override void Run()
        {
            foreach (var m in _container.ResolveAll<IMediaMapper>())
            {
                m.Map();
            }
        }
    }
}
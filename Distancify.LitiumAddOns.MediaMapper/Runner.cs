using Distancify.LitiumAddOns.MediaMapper.Services;
using System.Linq;
using Litium.Owin.InversionOfControl;
using Litium.Events;
using Litium.Runtime;
using Litium.Media.Events;
using Litium.Foundation;
using System.Configuration;
using Distancify.SerilogExtensions;
using Litium.Runtime.DistributedLock;
using System;

namespace Distancify.LitiumAddOns.MediaMapper
{

    /// <summary>
    /// Runner is responsible for subscribing to trigger all media mapping. It reads configuration, tracks events and schedules retries.
    /// </summary>
    [Autostart]
    public class Runner
    {
        private const string lockKey = "DistancifyMediaMapperRunner";

        public Runner(
            IIoCContainer container,
            EventBroker eventBroker,
            DistributedLockService distributedLockService) : this(
            container,
            eventBroker,
            distributedLockService,
            ConfigurationManager.AppSettings["MediaMapperEnabled"])
        {

        }

        public Runner(
            IIoCContainer container,
            EventBroker eventBroker,
            DistributedLockService distributedLockService,
            string enabledSetting)
        {
            if (string.IsNullOrEmpty(enabledSetting) || !bool.TryParse(enabledSetting, out bool enabled) || !enabled)
            {
                this.Log().Information("MediaMapper not enabled. Set web.config AppSetting \"MediaMapperEnabled\" to \"true\" to enable.");
                return;
            }

            var subscription = eventBroker.Subscribe<FileCreated>(EventScope.Local, ev =>
            {
                using (Solution.Instance.SystemToken.Use())
                using (distributedLockService.AcquireLock(lockKey, TimeSpan.FromSeconds(10)))
                {
                    foreach (var m in container.ResolveAll<IMediaMapper>().Where(r => r.GetUploadFolder()?.SystemId == ev.Item.FolderSystemId))
                    {
                        m.Map();
                    }
                }
            });
        }
    }
}
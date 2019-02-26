using Distancify.LitiumAddOns.MediaMapper.Services;
using System.Linq;
using Litium.Owin.InversionOfControl;
using Litium.Events;
using Litium.Runtime;
using Litium.Media.Events;
using System.Threading;
using Litium.Foundation;
using System.Threading.Tasks;
using System.Configuration;
using Distancify.SerilogExtensions;

namespace Distancify.LitiumAddOns.MediaMapper
{

    /// <summary>
    /// Runner is responsible for subscribing to trigger all media mapping. It reads configuration, tracks events and schedules retries.
    /// </summary>
    [Autostart]
    public class Runner
    {
        private Task _task;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public Runner(IIoCContainer container, EventBroker eventBroker, IApplicationLifetime applicationLifetime) : this(
            container,
            eventBroker,
            applicationLifetime,
            ConfigurationManager.AppSettings["MediaMapperEnabled"])
        {

        }

        public Runner(IIoCContainer container, EventBroker eventBroker, IApplicationLifetime applicationLifetime, string enabledSetting)
        {
            if (string.IsNullOrEmpty(enabledSetting) || !bool.TryParse(enabledSetting, out bool enabled) || !enabled)
            {
                this.Log().Information("MediaMapper not enabled. Set web.config AppSetting \"MediaMapperEnabled\" to \"true\" to enable.");
                return;
            }

            var subscription = eventBroker.Subscribe<FileCreated>(ev =>
            {
                using (Solution.Instance.SystemToken.Use())
                {
                    foreach (var m in container.ResolveAll<IMediaMapper>().Where(r => r.GetUploadFolder()?.SystemId == ev.Item.FolderSystemId))
                    {
                        m.Map();
                    }
                }
            });

            applicationLifetime.ApplicationStarted.Register(() =>
            {
                _task = Task.Run(() =>
                {
                    using (Solution.Instance.SystemToken.Use())
                    {
                        while (!_cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            if (SleepUnlessCancelled(5, _cancellationTokenSource.Token)) return;

                            foreach (var m in container.ResolveAll<IMediaMapper>())
                            {
                                m.Map();
                            }
                        }
                    }
                }, _cancellationTokenSource.Token);
            });

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                _cancellationTokenSource.Cancel();
                subscription.Dispose();
            });
        }

        private bool SleepUnlessCancelled(int minutes, CancellationToken token)
        {
            for (int i = 0; i < minutes * 60; i++)
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested) return _cancellationTokenSource.Token.IsCancellationRequested;
                Thread.Sleep(1000);
            }
            return _cancellationTokenSource.Token.IsCancellationRequested;
        }
    }
}
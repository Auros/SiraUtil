using SiraUtil.Logging;
using SiraUtil.Web.SiraSync.Implementations;
using SiraUtil.Web.Zenject;
using SiraUtil.Zenject;
using System.Collections.Generic;
using System.Reflection;

namespace SiraUtil.Web.SiraSync.Zenject
{
    internal class SiraSyncManager
    {
        private readonly Zenjector _managerZenjector;
        private readonly ZenjectManager _zenjectManager;
        private readonly HttpServiceManager _httpServiceManager;
        private readonly Dictionary<Assembly, ISiraSyncService> _services = [];

        public SiraSyncManager(Zenjector managerZenjector, ZenjectManager zenjectManager, HttpServiceManager httpServiceManager)
        {
            _zenjectManager = zenjectManager;
            _managerZenjector = managerZenjector;
            _httpServiceManager = httpServiceManager;
        }

        internal void AddService(Assembly assembly)
        {
            if (!_services.ContainsKey(assembly))
            {
                Zenjector? zenjector = _zenjectManager.ZenjectorFromAssembly(assembly);
                if (zenjector is not null)
                {
                    if (zenjector.HttpServiceType is not null)
                    {
                        ISiraSyncService service = null!;
                        if (zenjector.SiraSyncServiceType is SiraSyncServiceType.GitHub)
                        {
                            SiraLog siraLog = new();
                            siraLog.Setup(_managerZenjector.Logger!, nameof(GitHubSiraSyncService), false);
                            GitHubSiraSyncService ghService = new(siraLog, _httpServiceManager.ServiceFromAssembly(assembly));
                            ghService.Set(zenjector.SiraSyncOwner, zenjector.SiraSyncID);
                            service = ghService;
                            _services.Add(assembly, service);
                        }
                    }
                    else
                    {
                        Plugin.Log.Warn("There is no http service registered! Make sure you call .UseHttpService() on your Zenjector.");
                    }
                }
                else
                {
                    Plugin.Log.Warn("There is no zenjector associated with this assembly. Make sure to get your Zenjector from BSIPA's [Init] injector.");
                }
            }
        }

        internal ISiraSyncService ServiceFromAssembly(Assembly assembly)
        {
            if (_services.TryGetValue(assembly, out ISiraSyncService service))
                return service;

            Plugin.Log.Warn($"{assembly.GetName().Name}, you are depending on an {nameof(ISiraSyncService)}, but you haven't setup your own! You can setup your own by calling .UseSiraSync() on your zenjector.");
            return null!;
        }

        internal void Clear()
        {
            _services.Clear();
        }
    }
}
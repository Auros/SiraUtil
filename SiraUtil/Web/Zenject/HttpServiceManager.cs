using IPA.Loader;
using IPA.Utilities;
using SiraUtil.Web.Implementations;
using SiraUtil.Zenject;
using System.Collections.Generic;
using System.Reflection;

namespace SiraUtil.Web.Zenject
{
    internal class HttpServiceManager
    {
        private readonly ZenjectManager _zenjectManager;
        private readonly PluginMetadata _managerMetadata;
        private readonly Dictionary<Assembly, IHttpService> _services = new();

        public HttpServiceManager(ZenjectManager zenjectManager, PluginMetadata managerMetadata)
        {
            _zenjectManager = zenjectManager;
            _managerMetadata = managerMetadata;
        }

        internal void AddService(Assembly assembly)
        {
            if (!_services.ContainsKey(assembly))
            {
                var zenjector = _zenjectManager.ZenjectorFromAssembly(assembly);
                if (zenjector is not null)
                {
                    IHttpService service = null!;
                    if (zenjector.HttpServiceType is not null)
                    {
                        if (zenjector.HttpServiceType is HttpServiceType.UnityWebRequests)
                            service = new UWRHttpService();
                        _services.Add(assembly, service);


                        service.UserAgent = $"{zenjector.Metadata.Name}/{zenjector.Metadata.HVersion} ({service.GetType().Name}; {_managerMetadata.HVersion}; Beat Saber; {UnityGame.GameVersion})";
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

        internal IHttpService ServiceFromAssembly(Assembly assembly)
        {
            if (_services.TryGetValue(assembly, out IHttpService service))
                return service;

            Plugin.Log.Warn($"{assembly.GetName().Name}, you are depending on an IHttpService, but you haven't setup your own! You can setup your own by calling .UseHttpService() on your zenjector.");
            return null!;
        }

        internal void Clear()
        {
            _services.Clear();
        }
    }
}
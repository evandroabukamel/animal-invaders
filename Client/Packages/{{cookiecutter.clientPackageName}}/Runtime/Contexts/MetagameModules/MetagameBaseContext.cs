using Wildlife.MetagameBase;
using Wildlife.MetagameBase.DeviceInfo;

namespace Contexts.MetagameModules
{
    public struct MetagameBaseContext
    {
        public IMetagameClientBuilder ClientBuilder;
        public IDeviceInfoRetriever DeviceInfoRetriever;
        public IHookSerializer HookSerializer;
        public IMetagameClient Client;
        public IMetagameRequestFactory RequestFactory;
        public IMetagameRequestHandler RequestHandler;
    }
}
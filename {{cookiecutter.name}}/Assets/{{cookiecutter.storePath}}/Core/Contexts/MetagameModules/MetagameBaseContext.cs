using Wildlife.MetagameBase;
using Wildlife.MetagameBase.DeviceInfo;

namespace Contexts.MetagameModules
{
    ///<summary>
    ///A set of metagame resources related to the metagame base module which includes a metagame client builder,
    /// device information, a hook serializer, a metagame client, a metagame request factory and a metagame request handler.
    ///</summary>
    public struct MetagameBaseContext
    {
        ///<summary>
        ///The metagame client builder for creating a building a metagame client with a Pitaya config, binding and queue dispatcher.
        ///</summary>
        public IMetagameClientBuilder ClientBuilder;
        ///<summary>
        ///The information retriever for accessing current device data such as FIU and device type.
        ///</summary>
        public IDeviceInfoRetriever DeviceInfoRetriever;
        ///<summary>
        ///The hook serializer for marshalling and unmarshalling hook data.
        ///</summary>
        public IHookSerializer HookSerializer;
        ///<summary>
        ///The metagame client for connecting nd making requests through Pitaya.
        ///</summary>
        public IMetagameClient Client;
        ///<summary>
        ///The request factory for building metagame requests.
        ///</summary>
        public IMetagameRequestFactory RequestFactory;
        ///<summary>
        ///The request handler for building and queueing metagame requests.
        ///</summary>
        public IMetagameRequestHandler RequestHandler;
    }
}
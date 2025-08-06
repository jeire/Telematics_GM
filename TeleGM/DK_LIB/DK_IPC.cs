using System;
using System.Runtime.Remoting.Channels.Ipc;
using System.Security.Permissions;

namespace GmTelematics
{
    // Remote object.
    public class RemoteObject : MarshalByRefObject
    {
        private int callCount = 0;

        public int GetCount()
        {
            Console.WriteLine("GetCount has been called.");
            callCount++;
            return (callCount);
        }
    }

    class DK_IPCSERVER
    {
        public event EventRealTimeMsg IPCRealTimeTxRxMsg;         //대리자가 날릴 실제 이벤트 메소드
        private DK_LOGGER DKLogger;

        private void GateWay_IPC(string cParam) //로깅할때 데이터가 다시 실시간으로 manager 로 보내자.
        {
            IPCRealTimeTxRxMsg(0, cParam);
        }

        public DK_IPCSERVER()
        {
            DKLogger = new DK_LOGGER("SET", false);
            DKLogger.SendTxRxEvent += new EventTxRxMsg(GateWay_IPC);
        }

        private void SaveLog(string strLog, string strCommandName)
        {
            strLog = strLog.Replace("\n", "");
            DKLogger.WriteCommLog(strLog, "IPC:" + strCommandName, false);
            
        }

        public void InitializeServer()
        {
            // Create the server channel.
            SaveLog("[TX] " + "localhost:9090", "InitializeServer");
            IpcChannel serverChannel = new IpcChannel("localhost:9090");

            // Register the server channel.
            System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(serverChannel);

            // Show the name of the channel.
            SaveLog("[RX] " + "CHANNEL NAME : " + serverChannel.ChannelName, "InitializeServer");
            //Console.WriteLine("The name of the channel is {0}.", serverChannel.ChannelName);

            // Show the priority of the channel.
            SaveLog("[RX] " + "PRIORITY CHANNEL : " + serverChannel.ChannelPriority, "InitializeServer");
            //Console.WriteLine("The priority of the channel is {0}.", serverChannel.ChannelPriority);

            // Show the URIs associated with the channel.
            System.Runtime.Remoting.Channels.ChannelDataStore channelData = (System.Runtime.Remoting.Channels.ChannelDataStore)serverChannel.ChannelData;
            foreach (string uri in channelData.ChannelUris)
            {
                //Console.WriteLine("The channel URI is {0}.", uri);
                SaveLog("[RX] " + "URI CHANNEL : " + uri, "InitializeServer");
            }

            // Expose an object for remote calls.
            System.Runtime.Remoting.RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteObject), "RemoteObject.rem",
                    System.Runtime.Remoting.WellKnownObjectMode.Singleton);

            // Parse the channel's URI.
            string[] urls = serverChannel.GetUrlsForUri("RemoteObject.rem");
            if (urls.Length > 0)
            {
                string objectUrl = urls[0];
                string objectUri;
                string channelUri = serverChannel.Parse(objectUrl, out objectUri);
                SaveLog("[RX] " + "OBJECT URI : " + objectUri, "InitializeServer");
                SaveLog("[RX] " + "CHANNEL URI : " + channelUri, "InitializeServer");
                SaveLog("[RX] " + "OBJECT CHANNEL : " + objectUri, "InitializeServer");

                //Console.WriteLine("The object URI is {0}.", objectUri);
                //Console.WriteLine("The channel URI is {0}.", channelUri);
                //Console.WriteLine("The object URL is {0}.", objectUrl);
            }

            // Wait for the user prompt.
            //Console.WriteLine("Press ENTER to exit the server.");
            //Console.ReadLine();
            //Console.WriteLine("The server is exiting.");
            serverChannel.StopListening(channelData);
            SaveLog("[RX] " + "END SERVER", "InitializeServer");
        }
    }

    class DK_IPCGUEST
    {
        public DK_IPCGUEST()
        {
            
        }

        public void InitializeGuest()
        {
            // Create the channel.
            IpcChannel channel = new IpcChannel();

            // Register the channel.
            System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(channel);

            // Register as client for remote object.
            System.Runtime.Remoting.WellKnownClientTypeEntry remoteType = new System.Runtime.Remoting.WellKnownClientTypeEntry(
                    typeof(RemoteObject), "ipc://localhost:9090/RemoteObject.rem");
            System.Runtime.Remoting.RemotingConfiguration.RegisterWellKnownClientType(remoteType);

            // Create a message sink.
            string objectUri;
            System.Runtime.Remoting.Messaging.IMessageSink messageSink = channel.CreateMessageSink("ipc://localhost:9090/RemoteObject.rem", null, out objectUri);
            Console.WriteLine("The URI of the message sink is {0}.", objectUri);
            if (messageSink != null)
            {
                Console.WriteLine("The type of the message sink is {0}.", messageSink.GetType().ToString());
            }

            // Create an instance of the remote object.
            RemoteObject service = new RemoteObject();

            // Invoke a method on the remote object.
            Console.WriteLine("The client is invoking the remote object.");
            Console.WriteLine("The remote object has been called {0} times.", service.GetCount());
        }
    }
}

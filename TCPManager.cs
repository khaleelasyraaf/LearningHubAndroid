using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eneter.Messaging.EndPoints.TypedMessages;
using Eneter.Messaging.MessagingSystems.MessagingSystemBase;
using Eneter.Messaging.MessagingSystems.TcpMessagingSystem;

namespace LearningHubAndroid
{
    public class MyRequest
    {
        public string Text { get; set; }
    }

    // Response message type
    public class MyResponse
    {
        public int Length { get; set; }
        public string ResponseMessage { get; set; }
    }
    public class TCPManager
    {
        #region Events
        public event EventHandler<SensorDataReceivedEventArgs> SensorDataChanged;
        /// <summary>
        /// Event raised when the grip pressure has changed over the last iteration
        /// </summary>
        protected virtual void OnSensorDataChanged(SensorDataReceivedEventArgs AccEvent)
        {
            EventHandler<SensorDataReceivedEventArgs> handler = SensorDataChanged;
            if (handler != null)
            {
                handler(this, AccEvent);
            }
        }

        public class SensorDataReceivedEventArgs : EventArgs
        {
            public string Answer { get; set; }
            public string Acc_X { get; set; }
            public string Acc_Y { get; set; }
            public string Acc_Z { get; set; }
            public string Gyro_X { get; set; }
            public string Gyro_Y { get; set; }
            public string Gyro_Z { get; set; }
            public string Light { get; set; }
        }

        #endregion

        #region Variables

        public IDuplexTypedMessageReceiver<String, String> myReceiver;
        public IDuplexTypedMessagesFactory aReceiverFactory;
        public IMessagingSystemFactory aMessaging;
        public IDuplexInputChannel anInputChannel;
        //Default IpAddress
        public string IPAddress = "tcp://192.168.0.101:8800/";

        private string Answer = "";
        private string Acc_X = "";
        private string Acc_Y = "";
        private string Acc_Z = "";
        private string Gyro_X = "";
        private string Gyro_Y = "";
        private string Gyro_Z = "";
        private string Light = "";
        #endregion

        public TCPManager(string ipAddress)
        {
            IPAddress = ipAddress;
            // Create message receiver receiving 'MyRequest' and receiving 'MyResponse'.
            aReceiverFactory = new DuplexTypedMessagesFactory();
            myReceiver = aReceiverFactory.CreateDuplexTypedMessageReceiver<String, String>();

            // Subscribe to handle messages.
            myReceiver.MessageReceived += OnMessageReceived;

            // Create TCP messaging.
            // Note: 192.168.0.100 is the IP from the wireless router (no internet)
            // and 8800 is the socket.
            aMessaging = new TcpMessagingSystemFactory();
            anInputChannel = aMessaging.CreateDuplexInputChannel(IPAddress);

            // Attach the input channel and start to listen to messages.
            try
            {
                myReceiver.AttachDuplexInputChannel(anInputChannel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        /// <summary>
        /// CLoses the thread when application is stopped
        /// 
        /// </summary>
        public void CloseTCPListener()
        {

            // Detach the input channel and stop listening.
            // It releases the thread listening to messages.
            myReceiver.DetachDuplexInputChannel();
        }

        /// <summary>
        /// Sends data to the andriod application
        /// </summary>
        public void SendQuestion(String s)
        {
            // Create the response message.
            MyResponse aResponse = new MyResponse();
            aResponse.ResponseMessage = s;
            aResponse.Length = s.Length;

            if (SenderID == null || SenderID == "")
            {
                Debug.WriteLine("Send ID value is :"+ SenderID);
                return;
            }
            // Send the response message back to the client.
            myReceiver.SendResponseMessage(SenderID, aResponse.ResponseMessage);
        }

        string SenderID = "";
        /// <summary>
        /// Event handler that is called when the message is received from the andriod device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessageReceived(object sender,
              TypedRequestReceivedEventArgs<String> e)
        {
            //Console.WriteLine("Received: " + e);
            string s = e.RequestMessage;
            string indexParameter = s.Substring(0, 2);
            Console.WriteLine("indexParameter: " + indexParameter);
            switch (indexParameter)
            {
                case "AX":
                    Acc_X = s.Substring(s.IndexOf(":") + 1);
                    break;
                case "AY":
                    Acc_Y = s.Substring(s.IndexOf(":") + 1);
                    break;
                case "AZ":
                    Acc_Z = s.Substring(s.IndexOf(":") + 1);
                    break;
                case "GX":
                    Gyro_X = s.Substring(s.IndexOf(":") + 1);
                    break;
                case "GY":
                    Gyro_Y = s.Substring(s.IndexOf(":") + 1);
                    break;
                case "GZ":
                    Gyro_Z = s.Substring(s.IndexOf(":") + 1);
                    break;
                case "QQ":
                    Answer = s.Substring(s.IndexOf(":") + 1);
                    break;
                case "LI":
                    Light = s.Substring(s.IndexOf(":") + 1);
                    break;

            }
            SensorDataReceivedEventArgs args = new SensorDataReceivedEventArgs();
            args.Answer = Answer;
            args.Acc_X = Acc_X;
            args.Acc_Y = Acc_Y;
            args.Acc_Z = Acc_Z;
            args.Gyro_X = Gyro_X;
            args.Gyro_Y = Gyro_Y;
            args.Gyro_Z = Gyro_Z;            
            args.Light = Light;
            OnSensorDataChanged(args);

            SenderID = e.ResponseReceiverId;

            // Create the response message.
            //MyResponse aResponse = new MyResponse();
            //aResponse.Length = e.RequestMessage.Length;

            // Send the response message back to the client.
            //myReceiver.SendResponseMessage(e.ResponseReceiverId, aResponse.Length.ToString());
        }

    }
}

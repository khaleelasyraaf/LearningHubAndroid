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

        private string Answer = "Waiting for an answer";
        private string Acc_X = "0";
        private string Acc_Y = "0";
        private string Acc_Z = "0";
        private string Gyro_X = "0";
        private string Gyro_Y = "0";
        private string Gyro_Z = "0";
        private string Light = "0";
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
        /// Event handler that is called when the message is received from the android device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessageReceived(object sender,
              TypedRequestReceivedEventArgs<String> e)
        {
            string s = e.RequestMessage;
            string indexParameter = s.Substring(0, 3);
            Console.WriteLine("indexParameter: " + indexParameter);
            switch (indexParameter)
            {

                case "Acc":
                    int aindexX = s.IndexOf("X:");
                    int aindexY = s.IndexOf("Y:");
                    int aindexZ = s.IndexOf("Z:");
                    Acc_X = s.Substring(aindexX + 2, (aindexY-aindexX)-2);
                    Acc_Y = s.Substring(aindexY + 2, (aindexZ-aindexY)-2);
                    Acc_Z = s.Substring(aindexZ + 2);
                    break;
                case "Gyr":
                    int gindexX = s.IndexOf("X:");
                    int gindexY = s.IndexOf("Y:");
                    int gindexZ = s.IndexOf("Z:");
                    Gyro_X = s.Substring(gindexX + 2, (gindexY - gindexX) - 2);
                    Gyro_Y = s.Substring(gindexY + 2, (gindexZ - gindexY) - 2);
                    Gyro_Z = s.Substring(gindexZ + 2);
                    break;
                case "Lig":
                    Light = s.Substring(s.IndexOf("L:") + 2);
                    break;
                case "Que":
                    Answer = s.Substring(s.IndexOf("Q:") + 2);
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

        }

    }
}

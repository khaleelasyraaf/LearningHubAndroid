using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorHub;

namespace LearningHubAndroid
{
    /// <summary>
    /// Class that handles all learning hub related instructions
    /// </summary>
    public class LearningHubManager
    {
        public static ConnectorHub.ConnectorHub myConnector;
        TCPManager myTCPmanager;     

        private string s = "s";
        private string Answer = "";
        private string Acc_X = "";
        private string Acc_Y = "";
        private string Acc_Z = "";
        private string Gyro_X = "";
        private string Gyro_Y = "";
        private string Gyro_Z = "";
        private string Light = "";

        public bool _isRecording = false;
        public bool IsRecording
        {
            get { return _isRecording; }
            set
            {
                _isRecording = value;
            }
        }


        public LearningHubManager(TCPManager myTCPmanager)
        {
            initializeLearningHub();
            this.myTCPmanager = myTCPmanager;
            myTCPmanager.SensorDataChanged += MyTCPmanager_SensorDataChanged;
        }

        private void MyTCPmanager_SensorDataChanged(object sender, TCPManager.SensorDataReceivedEventArgs e)
        {
            Answer = e.Answer;
            Acc_X = e.Acc_X;
            Acc_Y = e.Acc_Y;
            Acc_Z = e.Acc_Z;
            Gyro_X = e.Gyro_X;
            Gyro_Y = e.Gyro_Y;
            Gyro_Z = e.Gyro_Z;
            Light = e.Light;      
            SendData();
        }


        /// <summary>
        /// Method for initiating Learning Hub
        /// </summary>
        public void initializeLearningHub()
        {
            myConnector = new ConnectorHub.ConnectorHub();
            myConnector.init();
            myConnector.sendReady();
            SetValueNames();
        }

        /// <summary>
        /// Method for setting the values that LH has to store
        /// </summary>
        public void SetValueNames()
        {
            List<String> names = new List<string>();
            names.Add("Answer");
            names.Add("Acc_X");
            names.Add("Acc_Y");
            names.Add("Acc_Z");
            names.Add("Gyro_X");
            names.Add("Gyro_Y");
            names.Add("Gyro_Z");
            names.Add("Light");
            myConnector.setValuesName(names);

        }
        /// <summary>
        /// Call this  method to store frames. Calls Learning Hub's Storeframe Method
        /// </summary>
        public void SendData()
        {
            List<string> values = new List<string>();
            values.Add(Answer);
            values.Add(Acc_X);
            values.Add(Acc_Y);
            values.Add(Acc_Z);
            values.Add(Gyro_X);
            values.Add(Gyro_Y);
            values.Add(Gyro_Z);
            values.Add(Light);

            myConnector.storeFrame(values);

        }

        public void StartRecording()
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Threading;

namespace LearningHubAndroid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TCPManager myTCPManager;
        LearningHubManager myLearningHubManager;

        //bool value for switching the record button text and the color
        public static bool isRecordingData = false;

        private string ipAddress = "tcp://192.168.0.101:8800/";

        private string Answer = "Waiting for an answer";
        private string Acc_X = "0";
        private string Acc_Y = "0";
        private string Acc_Z = "0";
        private string Gyro_X = "0";
        private string Gyro_Y = "0";
        private string Gyro_Z = "0";
        private string Light = "0";

        public MainWindow()
        {
            InitializeComponent();
            myTCPManager = new TCPManager(ipAddress);
            myTCPManager.SensorDataChanged += MyTCPManager_SensorDataChanged;
            myLearningHubManager = new LearningHubManager(myTCPManager);
            LearningHubManager.myConnector.startRecordingEvent += MyConnector_startRecordingEvent;
            LearningHubManager.myConnector.stopRecordingEvent += MyConnector_stopRecordingEvent;
        }

        private void MyTCPManager_SensorDataChanged(object sender, TCPManager.SensorDataReceivedEventArgs e)
        {
            Answer = e.Answer;
            Acc_X = e.Acc_X;
            Acc_Y = e.Acc_Y;
            Acc_Z = e.Acc_Z;
            Gyro_X = e.Gyro_X;
            Gyro_Y = e.Gyro_Y;
            Gyro_Z = e.Gyro_Z;
            Light = e.Light;
            UpdateAccelerometer(Acc_X, Acc_Y, Acc_Z);
            UpdateGyroscope(Gyro_X, Gyro_Y, Gyro_Z);
            UpdateLight(Light);
            UpdateAnswer(Answer);

        }

        private void MyConnector_stopRecordingEvent(object sender)
        {
            myLearningHubManager.IsRecording = false;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(
                        () =>
                        {
                            StartRecordingData();
                        }));
        }

        private void MyConnector_startRecordingEvent(object sender)
        {
            myLearningHubManager.IsRecording = true;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(
                        () =>
                        {
                            StartRecordingData();
                        }));
        }

        /// <summary>
        /// Method to update the accelerometer values
        /// </summary>
        /// <param name="Acc_X"></param>
        /// <param name="Acc_Y"></param>
        /// <param name="Acc_Z"></param>
        public void UpdateAccelerometer(string Acc_X, string Acc_Y, string Acc_Z)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(
                        () =>
                        {
                            AccelerometerTxt.Text = "X:"+ Acc_X + Environment.NewLine + "Y: " + Acc_Y + Environment.NewLine + "Z: "+ Acc_Z;
                        }));
        }

        /// <summary>
        /// Method to update the gyroscope values
        /// </summary>
        /// <param name="Gyro_X"></param>
        /// <param name="Gyro_Y"></param>
        /// <param name="Gyro_Z"></param>
        public void UpdateGyroscope(string Gyro_X, string Gyro_Y, string Gyro_Z)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(
                        () =>
                        {
                            GyroscopeTxt.Text = "X:" + Gyro_X + Environment.NewLine + "Y: " + Gyro_Y + Environment.NewLine + "Z: " + Gyro_Z;
                        }));
        }

        /// <summary>
        /// Method to update the light value
        /// </summary>
        /// <param name="Light"></param>
        public void UpdateLight(string Light)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(
                        () =>
                        {
                            LightTxt.Text = "Light:" + Light;
                        }));
        }

        /// <summary>
        /// Method to update the answer/text received from the Android device
        /// </summary>
        /// <param name="Answer"></param>
        public void UpdateAnswer(string Answer)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(
                        () =>
                        {
                            AnswerTxt.Text = "Answer:" + Answer;
                        }));
        }

        // Button to record the sensor data 
        private void RecordingButton_Click(object sender, RoutedEventArgs e)
        {
            myLearningHubManager.IsRecording = !myLearningHubManager.IsRecording;
            StartRecordingData();
        }

        // Button to send the question/text to the Android device
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("SendButtonClicked");
            String Q = QuestionTxt.Text;
            myTCPManager.SendQuestion(Q);

        }

        public void StartRecordingData()
        {
            if (isRecordingData == false)
            {
                isRecordingData = true;
                RecordingButton.Content = "Stop Recording";
                RecordingButton.Background = new SolidColorBrush(Colors.Green);

            }
            else if (isRecordingData == true)
            {
                isRecordingData = false;
                RecordingButton.Content = "Start Recording";
                RecordingButton.Background = new SolidColorBrush(Colors.White);
            }
            Debug.WriteLine("isRecordingData= " + isRecordingData);
        }

    }
}

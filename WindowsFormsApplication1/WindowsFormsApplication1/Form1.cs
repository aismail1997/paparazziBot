using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IBMWIoTP;
using Newtonsoft.Json.Linq;
using System.Speech.Synthesis;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private static string orgId = "73k64g";
        private static string appId = "picamera";
        private static string apiKey = "a-73k64g-mkg2xgnlmx";
        private static string authToken = "ZKn7n3IWgX3JLl*pRH";
        private static string deviceType = "Raspberry_Pi3";
        private static string deviceId = "Device02";
        private static string imagestatus;
        private static int check = 0;

        private ApplicationClient applicationClient = new ApplicationClient(orgId, appId, apiKey, authToken);

        public Form1()
        {
            InitializeComponent();
            setup();
            try
            {                 
                applicationClient.eventCallback += processEvent;
                applicationClient.deviceStatusCallback += processDeviceStatus;
                applicationClient.appStatusCallback += processAppStatus;

                applicationClient.subscribeToDeviceStatus(deviceType, deviceId);
                applicationClient.subscribeToApplicationStatus();
            }
            catch (Exception ex) {
                textBox1.Text = "ex:" + ex.Message;
            }
        }

        void setup()
        {
            try { 
                applicationClient.connect();
                applicationClient.subscribeToDeviceEvents(deviceType, deviceId);
            } catch(Exception ex) {
                textBox1.Text = "ex:" + ex.Message;
            }
            textBox1.Text = "Started Connection...";
            //Console.WriteLine("Started Connection...");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // move forward
            string cmdName = "move";
            string data = "F";
            sendcmd(cmdName, data);
            textBox1.Text = "Moving Backward...";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Move back
            string cmdName = "move";
            string data = "B";
            sendcmd(cmdName, data);
            textBox1.Text = "Moving Forward...";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Turn right
            string cmdName = "move";
            string data = "R";
            sendcmd(cmdName, data);
            textBox1.Text = "Turning right...";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // take a picture
            string cmdName = "takeapic";
            string data = "P";
            textBox1.Text = "Taking a pic...";
            sendcmd(cmdName, data);
            while (check == 0) {}
            check = 0;
            textBox1.Text = imagestatus;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // turn left
            string cmdName = "move";
            string data = "L";
            sendcmd(cmdName, data);
            textBox1.Text = "Turning Left...";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // stop
            string cmdName = "move";
            string data = "S";
            sendcmd(cmdName, data);
            textBox1.Text = "Stopping...";
        }
        private void sendcmd(string cmdName, string data) {
            string cmdFormat = "json";
            int qualityOfServies = 0;
            try
            {
                applicationClient.publishCommand(deviceType, deviceId, cmdName, cmdFormat, data, qualityOfServies);
            }
            catch (Exception ex)
            {
                textBox1.Text = "ex:" + ex.Message;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Ending connection...";
            try {
                applicationClient.disconnect();
            } catch (Exception ex) {
                textBox1.Text = "ex:" + ex.Message;
                // exit application
            }
            Application.Exit();
        }

        public static void processCommand(string cmdName, string format, string data)
        {
        }
        
        public static void processEvent(string deviceType, string deviceId, string eventName, string format, string data)
        {            
            Console.WriteLine(data);
            //if (!eventName.Equals("image_buffer"))
            //{
                JObject myjson = JObject.Parse(data);

                JToken faces = myjson["d"]["images"][0]["faces"];
                if (faces.HasValues)
                {
                    faces = faces[0];
                }

                if (!faces.HasValues)
                {
                    imagestatus = "Couldn't find anyone";
                    //Console.WriteLine(imagestatus);

                }
                else {
                    if (faces["identity"] == null)
                    {
                        imagestatus = "Found a " + faces["gender"]["gender"] + " of the age range: "  + faces["age"]["min"] + " - " + faces["age"]["max"];
                        //Console.WriteLine(imagestatus);
                    }
                    else
                    {
                        imagestatus = "Celebrity Found! " + (string)faces["identity"]["name"];
                        //Console.WriteLine(imagestatus);
                    }
                }
            check = 1;
                speech(imagestatus);
        }

        public static void processDeviceStatus(string deviceType, string deviceId, string data)
        {
            //Console.WriteLine(data);
        }

        public static void processAppStatus(string appId, String data)
        {
            //Console.WriteLine(data);
        }

        public static void speech(string str)
        {
            SpeechSynthesizer synthesizer = new SpeechSynthesizer();
            synthesizer.Volume = 100;  // 0...100
            synthesizer.Rate = -2;     // -10...10

            // Synchronous
            synthesizer.Speak(str);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // take a picture
            string cmdName = "Auto";
            string data = textBox2.Text;
            textBox1.Text = "Auto mode activated...";
            sendcmd(cmdName, data);
            while (check == 0) { }
            check = 0;
            textBox1.Text = imagestatus;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

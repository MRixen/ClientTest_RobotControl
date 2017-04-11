using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientTest_RobotControl
{
    public partial class Form1 : Form
    {

        private Thread thread_sender, thread_receiver, thread_dataHandler;
        private int port_server_send = 4001;
        private int port_server_receive = 4002;
        private string ip = "127.0.0.1";
        private TcpClient tcpClient_sender = new TcpClient();
        private TcpClient tcpClient_receiver = new TcpClient();
        private bool isRunning = true;
        private NetworkStream networkStream_sender, networkStream_receiver;
        private Byte[] recBytes = new Byte[8];
        private Byte[] sendBytes = new Byte[4];
        private int DELAY_TIME_THREAD = 10; // ms
        private const int MAX_WAIT_UNTIL_RETURN = 2000;
        private int duration = 0;
        private bool lockAction1 = false;
        private bool lockAction2 = false;
        private int[] statesBack = { 0, 0 };

        enum RobotActions
        {
            action_nothingToDo,
            action_newPosition,
            action_saveToEeprom,
            action_disablePidController,
            action_enablePidController
        };

        enum Outgoing_Package_Content
        {
            out_action,
            out_motorId,
            out_actionState
        };

        enum Incoming_Package_Content
        {
            in_action,
            in_motorId,
            in_velocity,
            in_angle_1,
            in_angle_2,
            in_motorDir
        };

        enum ActionStates
        {
            state_init,
            state_complete,
            state_pending
        };


        public Form1()
        {
            InitializeComponent();

            // Start sender loop
            thread_sender = new Thread(loop_sender);
            thread_sender.Start();

            // Start receiver loop
            thread_receiver = new Thread(loop_receiver);
            thread_receiver.Start();

            // Manage receive / send
            thread_dataHandler = new Thread(loop_dataHandler);
            thread_dataHandler.Start();
        }

        private void loop_dataHandler()
        {
            while (true)
            {
                if (tcpClient_sender.Connected & tcpClient_receiver.Connected)
                {
                    try
                    {
                        recBytes = receiveData();

                        // Check motor id
                        if (recBytes[1] == 1) {
                            if ((recBytes[(int)Incoming_Package_Content.in_action] == (int)RobotActions.action_nothingToDo))
                            {
                                lockAction1 = false;
                                for (int i = 0; i < sendBytes.Length; i++) sendBytes[i] = 0;
                            }

                            if ((!lockAction1) & (recBytes[(int)Incoming_Package_Content.in_action] == (int)RobotActions.action_newPosition))
                            {
                                lockAction1 = true;
                                Debug.WriteLine("action_newPosition1");
                                sendBytes[0] = recBytes[0]; // action
                                sendBytes[1] = recBytes[1]; // Motor id
                                sendBytes[2] = (byte)ActionStates.state_pending; // Motor angle
                            }
                        }

                        // Check motor id
                        if (recBytes[1] == 2)
                        {
                            if ((recBytes[(int)Incoming_Package_Content.in_action] == (int)RobotActions.action_nothingToDo))
                            {
                                lockAction2 = false;
                                for (int i = 0; i < sendBytes.Length; i++) sendBytes[i] = 0;
                            }

                            if ((!lockAction2) & (recBytes[(int)Incoming_Package_Content.in_action] == (int)RobotActions.action_newPosition))
                            {
                                lockAction2 = true;
                                Debug.WriteLine("action_newPosition2");
                                sendBytes[0] = recBytes[0]; // action
                                sendBytes[1] = recBytes[1]; // Motor id
                                sendBytes[2] = (byte)ActionStates.state_pending; // Motor angle
                            }
                        }

                        sendData(sendBytes);

                        //if (duration >= MAX_WAIT_UNTIL_RETURN)
                        //{
                        //    Debug.WriteLine("State complete");
                        //    sendBytes[0] = recBytes[0]; // action
                        //    sendBytes[1] = recBytes[1]; // Motor id
                        //    sendBytes[2] = (byte)ActionStates.state_complete; // Motor angle
                        //    sendData(sendBytes);
                        //    duration = 0;
                        //}

                        //Thread.Sleep(DELAY_TIME_THREAD);
                        //duration = duration + DELAY_TIME_THREAD;
                    }
                    catch (NullReferenceException e)
                    {
                        closeClients();
                        Environment.Exit(0);
                    }
                }
            }
        }

        private void RequestStop()
        {
            isRunning = false;
        }

        private void closeClients()
        {
            tcpClient_sender.Close();
            tcpClient_receiver.Close();
            Console.WriteLine(" >> Clients closed.");
        }

        private void loop_sender()
        {
            while (true)
            {
                try
                {
                    while (!tcpClient_sender.Connected)
                    {
                        Debug.WriteLine(" >> Start client sender");
                        tcpClient_sender.Connect(ip, port_server_send);
                    Debug.WriteLine(" >> Connected as sender");
                    }
                    networkStream_sender = tcpClient_sender.GetStream();
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    Debug.WriteLine(" >> Cant connect as sender to server.\n");
                    Debug.WriteLine(" >> " + e.ToString());
                }
            }
        }

        private void loop_receiver()
        {
            while (true)
            {
                try
                {
                    while (!tcpClient_receiver.Connected)
                    {
                        Debug.WriteLine(" >> Start client receiver");
                        tcpClient_receiver.Connect(ip, port_server_receive);
                    Debug.WriteLine(" >> Connected as receiver");
                    }
                    networkStream_receiver = tcpClient_receiver.GetStream();
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    Debug.WriteLine(" >> Cant connect as receiver to server.\n");
                    Debug.WriteLine(" >> " + e.ToString());
                }
            }
        }

        private Byte[] receiveData()
        {
            Byte[] receiveBytes = new byte[8];
            if (networkStream_sender != null)
            {
                if (tcpClient_receiver != null)
                {
                    networkStream_sender.Read(receiveBytes, 0, 8);
                    short action = receiveBytes[0];
                    short motorId = receiveBytes[1];
                    short motorVel = receiveBytes[2];
                    byte mPosTemp = receiveBytes[3];
                    receiveBytes[3] = receiveBytes[4];
                    receiveBytes[4] = mPosTemp;
                    int motorAngle = BitConverter.ToInt16(receiveBytes, 3);
                    short motorDir = receiveBytes[5];
                    Debug.WriteLine( motorId + ";" +  motorAngle + ";" + motorVel + ";" + action + ";" +  motorDir);
                }

            }
            return receiveBytes;
        }

        private void sendData(Byte[] data)
        {
            if (networkStream_receiver != null)
            {
                if (tcpClient_sender != null)
                {
                    //string stringToSend = "TestMessage;1:1:3;";
                    //Byte[] sendBytes = Encoding.ASCII.GetBytes(stringToSend);
                    networkStream_receiver.Write(data, 0, data.Length);
                    networkStream_receiver.Flush();
                }

            }
        }

        private void button_Motor1_ok_Click(object sender, EventArgs e)
        {
            statesBack[0] = (int)ActionStates.state_complete;
        }


        private void button_Motor2_ok_Click(object sender, EventArgs e)
        {
            statesBack[1] = (int)ActionStates.state_complete;
        }

        private void button_Motor3_ok_Click(object sender, EventArgs e)
        {

        }

        private void button_Motor4_ok_Click(object sender, EventArgs e)
        {

        }
    }
}

using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer
{
    class Sever
    {
        Socket serverSocket = null;
        IPEndPoint localEndPoint;
        bool isRuning = false;
        string[] teacherNames = { "teacher1" };
        string[] studentNames = { "student1", "student2", "student3", "student4" };

        List<ClientState> registerClientList;

        public Sever(int port)
        {
            //初始化建立serverSocket
            localEndPoint = new IPEndPoint(IPAddress.Any, port);

            serverSocket = new Socket(localEndPoint.AddressFamily,//AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            registerClientList = new List<ClientState>();
        }

        //服务器运行 一边监听client链接 一边发送数据
        public void Runing()
        {
            isRuning = true;
            //创建新线程监听用户的连接
            try
            {
                serverSocket.Bind(localEndPoint);
                serverSocket.Listen(100);
            }
            catch (Exception e)
            {
                throw new System.Exception("Error: sever fail to bind,or listen." + e.Message);
            }
            //开启客户端连接监听线程
            Thread acceptThread = new Thread(AcceptClient);
            acceptThread.Start();

            //while (true)
            //{
            //    if (newClient != null)
            //    {
            //        ClientState cs = new ClientState(newClient);
            //        clientList.Add(cs);
            //        newClient.BeginReceive(cs.buffer, 0, cs.bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), cs);
            //        newClient = null;
            //    }
            //}
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            ClientState cs = (ClientState)ar.AsyncState;
            Socket socket = cs.client;

            int receiveLen = 0;
            try
            {
                receiveLen = socket.EndReceive(ar);
            }
            catch(Exception e)
            {
                socket.Close();
                lock (registerClientList)
                {
                    registerClientList.Remove(cs);
                }
                Console.WriteLine("Exception: ReceiveCallback msg:"+e.Message);
                Console.WriteLine("Client disconnect, client num:" + registerClientList.Count);
                return;
            }
            
            Console.WriteLine("receiveLen:"+receiveLen);
            lock(registerClientList)
            {
                for(int i=0;i<registerClientList.Count;i++)
                {
                    if(!registerClientList[i].Equals(cs))
                    {
                        if(registerClientList[i].client.Connected)
                        {
                            registerClientList[i].client.Send(cs.buffer, 0, receiveLen, SocketFlags.None);
                        }
                        else
                        {
                            registerClientList[i].client.Close();
                            registerClientList.Remove(cs);
                            Console.WriteLine("Client disconnect, client num:" + registerClientList.Count);
                        }
                    }
                }
            }
            socket.BeginReceive(cs.buffer, 0, cs.bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), cs);
        }

        public void AcceptClient()
        {
            while (isRuning)
            {
                Socket newClient = serverSocket.Accept();
                TcpClient tcpClient = new TcpClient();
                tcpClient.Client = newClient;
                NetworkStream stream = tcpClient.GetStream();
                TransferCommand command = ProtoBuf.Serializer.DeserializeWithLengthPrefix<TransferCommand>(stream, PrefixStyle.Base128);
                //如果新加入的用户是老师 或学生
                if (IsTeacherName(command.userName) || IsStudentName(command.userName)) 
                {
                    ClientState cs = new ClientState(newClient);
                    lock (registerClientList)
                    {
                        registerClientList.Add(cs);
                    }
                    if (IsTeacherName(command.userName))
                        command.userStyle = TransferCommand.UserStyle.Teacher;
                    else
                        command.userStyle = TransferCommand.UserStyle.Student;
                    ProtoBuf.Serializer.SerializeWithLengthPrefix<TransferCommand>(stream, command, PrefixStyle.Base128);
                    newClient.BeginReceive(cs.buffer, 0, cs.bufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), cs);
                    Console.WriteLine("add Client, Client num:" + registerClientList.Count);
                }
                //如果用户身份无法识别则断开用户
                else
                {
                    newClient.Close();
                }
                newClient = null;
            }
        }

        private bool IsStudentName(string str)
        {
            for (int i = 0; i < studentNames.Count(); i++)
            {
                if (String.Compare(studentNames[i], str, true) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsTeacherName(string str)
        {
            for (int i = 0; i < teacherNames.Count(); i++)
            {
                if (String.Compare(teacherNames[i],str,true)==0)
                {
                    return true;
                }
            }
            return false;
        }

    }

    

    public class ClientState
    {
        public Socket client = null;
        public int index = 0;
        public int bufferSize = 1024 * 2;
        public byte[] buffer;
        public int msgLen = 0;

        public bool isRegistered;
        public TransferCommand.UserStyle userStyle;

        public ClientState(Socket client1)
        {
            client = client1;
            index = 0;
            msgLen = 0;
            buffer = new byte[bufferSize];
        }
    }
}

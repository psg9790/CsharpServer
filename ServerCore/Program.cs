using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{

    class Program
    {
        static Listener _listener = new Listener();

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                Session session = new Session();    // 연결 시 세션 생성
                session.Start(clientSocket);    // Recv 자동화, Send 대기

                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome To MMORPG Server!");
                session.Send(sendBuff); // Send 요청

                Thread.Sleep(1000);
                session.Disconnect();
                session.Disconnect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, OnAcceptHandler);  // recv를 받으면 자동으로 OnAcceptHandler

            Console.WriteLine("Listening...");
            while (true)
            {

            }
        }
    }
}

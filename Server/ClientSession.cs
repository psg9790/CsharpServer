﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");
            //byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome To MMORPG Server!");
            //Packet packet = new Packet() { size = 100, packetId = 10 };
            /*
                        ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
                        byte[] buffer = BitConverter.GetBytes(packet.size);
                        byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
                        Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
                        Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
                        ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);
            */
            //Send(sendBuff); // Send 요청
            Thread.Sleep(5000);
            Disconnect();
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);

        }


        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");

        }
        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transffered bytes: {numOfBytes}");

        }
    }

}

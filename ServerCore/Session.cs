using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    public abstract class Session
    {
        Socket _socket;
        int _disconnected = 0;

        object _lock = new object();
        Queue<byte[]> _sendQueue = new Queue<byte[]>();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        public void Start(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _recvArgs.SetBuffer(new byte[1024], 0, 1024);

            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        public void Send(byte[] sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);   // send 할 내용을 큐에 넣는다
                if (_pendingList.Count == 0)  // 작업 중이지 않으면
                {
                    RegisterSend(); // 전송 등록한다
                }
            }
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)    // 멀티쓰레드 방지
                return;
            OnDisconnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        #region 네트워크통신

        void RegisterSend()
        {
            while (_sendQueue.Count > 0)
            {
                byte[] buff = _sendQueue.Dequeue();
                _pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
            }
            _sendArgs.BufferList = _pendingList;

            bool pending = _socket.SendAsync(_sendArgs);    // 비동기 send를 시작한다
            if (pending == false)   // 즉시 완료할 수 있다면
            {
                OnSendCompleted(null, _sendArgs);   // send 완료 작업을 바로 수행한다.
            }

        }
        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();

                        OnSend(_sendArgs.BytesTransferred);


                        if (_sendQueue.Count > 0)   // 추가된 작업이 더 있으면
                        {
                            RegisterSend(); // 자동으로 추가 전송
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {e}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }

        void RegisterRecv()
        {
            bool pending = _socket.ReceiveAsync(_recvArgs);
            if (pending == false)
            {
                OnRecvCompleted(null, _recvArgs);
            }
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));
                    /*string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From Client] {recvData}");*/

                    RegisterRecv();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e}");
                }
            }
            else
            {
                Disconnect();
            }
        }
        #endregion
    }
}

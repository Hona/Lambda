#region License

/*
Copyright (c) 2015 Betson Roy

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace QueryMaster.GameServer
{
    /// <summary>
    ///     Encapsulates a method that has a parameter of type string which is the log message received from server.
    ///     Invoked when a log message is received from server.
    /// </summary>
    /// <param name="log">Received log message.</param>
    public delegate void LogCallback(string log);

    /// <summary>
    ///     Provides methods to listen to logs and to set up events on desired type of log message.
    /// </summary>
    public class Logs : QueryMasterBase
    {
        private readonly int BufferSize = 1400;
        private readonly List<LogEvents> EventsInstanceList = new List<LogEvents>();
        private readonly int HeaderSize;
        private readonly int Port;
        private readonly byte[] recvData;
        internal LogCallback Callback;
        internal IPEndPoint ServerEndPoint;
        private Socket UdpSocket;

        internal Logs(EngineType type, int port, IPEndPoint serverEndPoint)
        {
            Port = port;
            ServerEndPoint = serverEndPoint;
            recvData = new byte[BufferSize];
            switch (type)
            {
                case EngineType.GoldSource:
                    HeaderSize = 10;
                    break;
                case EngineType.Source:
                    HeaderSize = 7;
                    break;
            }
        }

        /// <summary>
        ///     Gets a value that indicates whether its listening.
        /// </summary>
        public bool IsListening { get; private set; }


        /// <summary>
        ///     Start listening to logs.
        /// </summary>
        public void Start()
        {
            ThrowIfDisposed();
            if (IsListening)
                throw new QueryMasterException("QueryMaster already listening to logs.");
            IsListening = true;
            UdpSocket = new Socket(AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, ProtocolType.Udp);
            UdpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            UdpSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            UdpSocket.BeginReceive(recvData, 0, recvData.Length, SocketFlags.None, Recv, null);
        }

        /// <summary>
        ///     Stop listening to logs.
        /// </summary>
        public void Stop()
        {
            ThrowIfDisposed();
            if (UdpSocket != null)
                UdpSocket.Close();
            IsListening = false;
        }

        /// <summary>
        ///     Listen to logs sent by the server.
        /// </summary>
        /// <param name="callback">Called when a log message is received.</param>
        public void Listen(LogCallback callback)
        {
            ThrowIfDisposed();
            Callback = callback;
        }

        /// <summary>
        ///     Returns an instance of <see cref="LogEvents" /> that provides event and filtering mechanism.
        /// </summary>
        /// <returns>Instance of <see cref="LogEvents" /> </returns>
        public LogEvents GetEventsInstance()
        {
            ThrowIfDisposed();
            var eventObj = new LogEvents(ServerEndPoint);
            EventsInstanceList.Add(eventObj);
            return eventObj;
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    if (UdpSocket != null)
                        UdpSocket.Close();
                    foreach (var i in EventsInstanceList) i.Dispose();
                }

                base.Dispose(disposing);
                IsDisposed = true;
            }
        }

        private void Recv(IAsyncResult res)
        {
            var bytesRecv = 0;
            try
            {
                bytesRecv = UdpSocket.EndReceive(res);
            }
            catch (ObjectDisposedException)
            {
                return;
            }

            if (bytesRecv > HeaderSize)
            {
                var logLine = Encoding.UTF8.GetString(recvData, HeaderSize, bytesRecv - HeaderSize);
                Callback?.Invoke(string.Copy(logLine));
                foreach (var i in EventsInstanceList) i.ProcessLog(string.Copy(logLine));
            }

            UdpSocket.BeginReceive(recvData, 0, recvData.Length, SocketFlags.None, Recv, null);
        }
    }
}
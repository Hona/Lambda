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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using QueryMaster.MasterServer.DataObjects;

namespace QueryMaster.MasterServer
{
    /// <summary>
    ///     Invoked when addressess are received from master server.
    /// </summary>
    /// <param name="batchInfo">Server endpoints</param>
    public delegate void BatchReceivedCallback(BatchInfo batchInfo);

    /// <summary>
    ///     Represents Master Server.Provides method(s) to query master server.
    /// </summary>
    public class Server : QueryMasterBase
    {
        private const int BufferSize = 1400;
        private readonly ConnectionInfo _conInfo;
        private readonly IPEndPoint _seedEndpoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0);
        private readonly List<Task> _taskList = new List<Task>();
        private AttemptCallback _attemptCallback;
        private int _batchCount;
        private BatchReceivedCallback _callback;
        private CancellationTokenSource _cts;
        private ErrorCallback _errorCallback;
        private IpFilter _filter;
        private IPEndPoint _remoteEndPoint, _lastEndPoint;
        private Socket _socket;

        internal Server(ConnectionInfo conInfo, AttemptCallback attemptCallback, IPEndPoint remoteEndPoint)
        {
            _conInfo = conInfo;
            _attemptCallback = attemptCallback;
            _remoteEndPoint = remoteEndPoint;
        }

        /// <summary>
        ///     Get region.
        /// </summary>
        private Region Region { get; set; }

        /// <summary>
        ///     Starts receiving socket addresses of servers.
        /// </summary>
        /// <param name="region">The region of the world that you wish to find servers in.</param>
        /// <param name="callback">Called when a batch of Socket addresses are received.</param>
        /// <param name="filter">Used to set filter on the type of server required.</param>
        /// <param name="batchCount">
        ///     Number of batches to fetch.-1 would return all addressess.(1 batch = 1 udppacket = 231
        ///     addressess).
        /// </param>
        /// <param name="errorCallback">Invoked in case of error.</param>
        public void GetAddresses(Region region, BatchReceivedCallback callback, IpFilter filter = null,
            int batchCount = 1, ErrorCallback errorCallback = null)
        {
            ThrowIfDisposed();
            StopReceiving();
            Region = region;
            _callback = callback;
            _errorCallback = errorCallback;
            _batchCount = batchCount == -1 ? int.MaxValue : batchCount;
            _filter = filter;
            _lastEndPoint = null;
            Initialize();
            _taskList.First().Start();
        }

        /// <summary>
        ///     Provides next batch of addressess.
        /// </summary>
        /// <param name="batchCount">
        ///     Number of batches to fetch.-1 would return all addressess.(1 batch = 1 udppacket = 231
        ///     addressess).
        /// </param>
        /// <param name="refresh">Whether to clear internal state and obtain addresses from start.</param>
        public void GetNextBatch(int batchCount = 1, bool refresh = false)
        {
            ThrowIfDisposed();
            _taskList.Add(_taskList.Last().ContinueWith(x =>
            {
                if (IsDisposed)
                    return;
                if (_callback == null)
                    throw new InvalidOperationException("Call GetAddresses before calling this method.");
                if (_cts.IsCancellationRequested)
                    return;
                if (refresh)
                {
                    _lastEndPoint = null;
                }
                else if (_lastEndPoint.Equals(_seedEndpoint))
                {
                    _cts?.Cancel();
                    throw new MasterServerException("Already received all the addresses.");
                }

                _batchCount = batchCount == -1 ? int.MaxValue : batchCount;
                StartReceiving();
            }));
        }

        private void Initialize()
        {
            _socket = new Socket(AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, ProtocolType.Udp)
            {
                SendTimeout = _conInfo.SendTimeout,
                ReceiveTimeout = _conInfo.ReceiveTimeout
            };
            _socket.Connect(_conInfo.EndPoint);
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            _taskList.Clear();
            _taskList.Add(new Task(StartReceiving, _cts.Token));
        }

        private void StartReceiving()
        {
            byte[] msg = null, recvBytes = null;
            var isNewMsg = true;
            int attemptCounter = 0, attempts = _conInfo.Retries + 1, batchCounter = 0;
            var isLastBatch = false;
            var endPoint = _lastEndPoint ?? _seedEndpoint;

            try
            {
                while (batchCounter < _batchCount)
                {
                    bool hasRecvMsg;
                    if (isNewMsg)
                    {
                        msg = MasterUtil.BuildPacket(endPoint.ToString(), Region, _filter);
                        recvBytes = new byte[BufferSize];
                        isNewMsg = false;
                    }

                    try
                    {
                        attemptCounter++;
                        if (_attemptCallback != null)
                        {
                            var counter = attemptCounter;
                            ThreadPool.QueueUserWorkItem(x => _attemptCallback(counter));
                        }
                        _socket.Send(msg);
                        var recv = _socket.Receive(recvBytes);
                        recvBytes = recvBytes.Take(recv).ToArray();
                        hasRecvMsg = true;
                    }
                    catch (ObjectDisposedException)
                    {
                        return;
                    }
                    catch (SocketException ex)
                    {
                        _cts.Token.ThrowIfCancellationRequested();
                        if (ex.SocketErrorCode == SocketError.TimedOut)
                        {
                            hasRecvMsg = false;
                            if (attemptCounter >= attempts)
                                throw;
                        }
                        else
                        {
                            throw;
                        }
                    }

                    if (hasRecvMsg)
                    {
                        attemptCounter = 0;
                        batchCounter++;
                        var endPoints = MasterUtil.ProcessPacket(recvBytes);
                        endPoint = endPoints.Last();
                        isNewMsg = true;
                        _lastEndPoint = endPoint;
                        if (endPoints.Last().Equals(_seedEndpoint))
                        {
                            endPoints.RemoveAt(endPoints.Count - 1);
                            isLastBatch = true;
                        }

                        _callback(new BatchInfo
                        {
                            Region = Region,
                            Source = _conInfo.EndPoint,
                            ReceivedEndpoints = new QueryMasterCollection<IPEndPoint>(endPoints),
                            IsLastBatch = isLastBatch
                        });
                        if (isLastBatch)
                        {
                            _cts.Cancel();
                            break;
                        }
                    }

                    _cts.Token.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _errorCallback?.Invoke(ex);
            }
        }

        private void StopReceiving()
        {
            if (_taskList.Count == 0) return;
            _cts?.Cancel();
            _socket?.Dispose();
            Task.WaitAll(_taskList.ToArray());
            _taskList.Clear();
            _cts = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed) return;
            if (disposing)
            {
                StopReceiving();
                if (_cts != null)
                {
                    _cts.Dispose();
                    _cts = null;
                }

                _taskList.Clear();
                _callback = null;
                _errorCallback = null;
                _attemptCallback = null;
            }

            base.Dispose(disposing);
            IsDisposed = true;
        }
    }
}
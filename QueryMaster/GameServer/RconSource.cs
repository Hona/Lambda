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
using System.Linq;
using System.Text;

namespace QueryMaster.GameServer
{
    internal class RconSource : Rcon
    {
        private readonly ConnectionInfo ConInfo;
        internal TcpQuery socket;

        private RconSource(ConnectionInfo conInfo)
        {
            ConInfo = conInfo;
        }

        internal static Rcon Authorize(ConnectionInfo conInfo, string msg)
        {
            return new QueryMasterBase().Invoke<Rcon>(() =>
                {
                    var obj = new RconSource(conInfo) {socket = new TcpQuery(conInfo)};
                    var recvData = new byte[50];
                    var packet = new RconSrcPacket
                        {Body = msg, Id = (int) PacketId.ExecCmd, Type = (int) PacketType.Auth};
                    recvData = obj.socket.GetResponse(RconUtil.GetBytes(packet));
                    int header;
                    try
                    {
                        header = BitConverter.ToInt32(recvData, 4);
                    }
                    catch (Exception e)
                    {
                        e.Data.Add("ReceivedData", recvData ?? new byte[1]);
                        throw;
                    }

                    return obj;
                }, conInfo.Retries + 1, null, conInfo.ThrowExceptions);
        }

        public override string SendCommand(string command, bool isMultipacketResponse = false)
        {
            ThrowIfDisposed();
            return Invoke(() => sendCommand(command, isMultipacketResponse), 1, null, ConInfo.ThrowExceptions);
        }

        private string sendCommand(string command, bool isMultipacketResponse)
        {
            var senPacket = new RconSrcPacket
                {Body = command, Id = (int) PacketId.ExecCmd, Type = (int) PacketType.Exec};
            var recvData = socket.GetMultiPacketResponse(RconUtil.GetBytes(senPacket));
            var str = new StringBuilder();
            try
            {
                for (var i = 0; i < recvData.Count; i++)
                {
                    //consecutive rcon command replies start with an empty packet 
                    if (BitConverter.ToInt32(recvData[i], 4) == (int) PacketId.Empty)
                        continue;
                    if (recvData[i].Length - BitConverter.ToInt32(recvData[i], 0) == 4)
                        str.Append(RconUtil.ProcessPacket(recvData[i]).Body);
                    else
                        str.Append(RconUtil.ProcessPacket(recvData[i]).Body +
                                   Util.BytesToString(recvData[++i].Take(recvData[i].Length - 2).ToArray()));
                }
            }
            catch (Exception e)
            {
                e.Data.Add("ReceivedData", recvData.SelectMany(x => x).ToArray());
                throw;
            }

            return str.ToString();
        }

        public override void AddlogAddress(string ip, ushort port)
        {
            ThrowIfDisposed();
            SendCommand("logaddress_add " + ip + ":" + port);
        }

        public override void RemovelogAddress(string ip, ushort port)
        {
            ThrowIfDisposed();
            SendCommand("logaddress_del " + ip + ":" + port);
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                    if (socket != null)
                        socket.Dispose();
                base.Dispose(disposing);
                IsDisposed = true;
            }
        }
    }
}
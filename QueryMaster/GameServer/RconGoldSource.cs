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

namespace QueryMaster.GameServer
{
    internal class RconGoldSource : Rcon
    {
        internal static readonly byte[] RconChIdQuery =
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0x63, 0x68, 0x61, 0x6c, 0x6c, 0x65, 0x6e, 0x67, 0x65, 0x20, 0x72, 0x63, 0x6f, 0x6e
        };

        internal static readonly byte[]
            RconQuery =
            {
                0xFF, 0xFF, 0xFF, 0xFF, 0x72, 0x63, 0x6f, 0x6e, 0x20
            }; //+<challenge id>+"<rcon password>"+<value>

        private readonly ConnectionInfo _conInfo;

        internal string ChallengeId = string.Empty;

        internal string RConPass = string.Empty;
        internal UdpQuery Socket;

        private RconGoldSource(ConnectionInfo conInfo)
        {
            Socket = new UdpQuery(conInfo);
            _conInfo = conInfo;
        }

        internal static Rcon Authorize(ConnectionInfo conInfo, string pass)
        {
            var obj = new RconGoldSource(conInfo);
            obj.ChallengeId = obj.GetChallengeId();
            obj.RConPass = pass;
            if (obj != null)
            {
                var reply = obj.SendCommand("");
                if (reply != null && !reply.Contains("Bad rcon_password"))
                    return obj;
            }

            obj.Dispose();
            return null;
        }


        public override string SendCommand(string command, bool isMultiPacketresponse = false)
        {
            ThrowIfDisposed();
            return Invoke(() => sendCommand(command, isMultiPacketresponse), 1, null, _conInfo.ThrowExceptions);
        }

        private string sendCommand(string command, bool isMultiPacketresponse)
        {
            var rconMsg = Util.MergeByteArrays(RconQuery, Util.StringToBytes(ChallengeId),
                Util.StringToBytes(" \"" + RConPass + "\" " + command));
            var recvData = new byte[2000];
            string s;
            if (string.IsNullOrEmpty(command))
                recvData = Socket.GetResponse(rconMsg, EngineType.GoldSource, isMultiPacketresponse);
            else
                recvData = Socket.GetResponse(rconMsg, EngineType.GoldSource, isMultiPacketresponse);
            try
            {
                s = Util.BytesToString(recvData).Remove(0, 1);
            }
            catch (Exception e)
            {
                e.Data.Add("ReceivedData", recvData ?? new byte[1]);
                throw;
            }

            return s;
        }

        private string GetChallengeId()
        {
            return Invoke(() =>
            {
                byte[] recvData = null;
                var challengeId = string.Empty;
                try
                {
                    recvData = Socket.GetResponse(RconChIdQuery, EngineType.GoldSource);
                    var parser = new Parser(recvData);
                    challengeId = parser.ReadString().Split(' ')[2].Trim();
                }
                catch (Exception e)
                {
                    e.Data.Add("ReceivedData", recvData ?? new byte[1]);
                    throw;
                }

                return challengeId;
            }, _conInfo.Retries + 1, null, _conInfo.ThrowExceptions);
        }

        public override void AddlogAddress(string ip, ushort port)
        {
            ThrowIfDisposed();
            SendCommand("logaddress_add " + ip + " " + port);
        }

        public override void RemovelogAddress(string ip, ushort port)
        {
            ThrowIfDisposed();
            SendCommand("logaddress_del " + ip + " " + port);
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                    if (Socket != null)
                        Socket.Dispose();
                base.Dispose(disposing);
                IsDisposed = true;
            }
        }
    }
}
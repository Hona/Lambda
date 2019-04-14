using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using LambdaUI.Constants;
using LambdaUI.Minecraft.Payloads;
using LambdaUI.Models;
using Newtonsoft.Json;

namespace LambdaUI.Minecraft
{
    // Based off https://gist.github.com/csh/2480d14fbbb33b4bbae3
    internal static class ServerPing
    {
        private static NetworkStream _stream;
        private static List<byte> _buffer;
        private static int _offset;

        internal static async Task<MinecraftServerModel> Ping()
        {
            _stream = null;
            _buffer = null;
            _offset = 0;

            var client = new TcpClient();
            await client.ConnectAsync(ServerConstants.MinecraftServerIpAddress, 25565);

            if (!client.Connected)
                throw new Exception("Unable to connect to the Minecraft server");


            _buffer = new List<byte>();
            _stream = client.GetStream();

            SendHandshake();

            SendStatusRequest();

            var buffer = new byte[32768];
            await _stream.ReadAsync(buffer, 0, buffer.Length);

            try
            {
                var length = ReadVarInt(buffer);
                var packet = ReadVarInt(buffer);
                var jsonLength = ReadVarInt(buffer);

                var json = ReadString(buffer, jsonLength);
                File.WriteAllText(@"E:\dump.txt", json);
                var ping = JsonConvert.DeserializeObject<PingPayload>(json);

                var output = new MinecraftServerModel
                {
                    Motd = ping.Motd.Text,
                    Protocol = ping.Version.Protocol.ToString(),
                    Version = ping.Version.ToString(),
                    PlayersMax = ping.Players.Max.ToString(),
                    PlayersOnline = ping.Players.Online.ToString()
                };
                if (ping.Players.Sample != null && ping.Players.Sample.Count > 0)
                    output.OnlinePlayerList = ping.Players.Sample.ConvertAll(x => x.Name);

                client.Close();
                client.Dispose();
                _stream.Close();
                _stream.Dispose();

                return output;
            }
            catch (IOException)
            {
                /*
                 * If an IOException is thrown then the server didn't 
                 * send us a VarInt or sent us an invalid one.
                 */
                throw new Exception("Unable to read packet length from server, are you sure it's a Minecraft server?");
            }
        }

        private static void SendStatusRequest()
        {
/*
             * Send a "Status Request" packet
             * http://wiki.vg/Server_List_Ping#Ping_Process
             */
            Flush(0);
        }

        private static void SendHandshake()
        {
/*
             * Send a "Handshake" packet
             * http://wiki.vg/Server_List_Ping#Ping_Process
             */
            WriteVarInt(47);
            WriteString(ServerConstants.MinecraftServerIpAddress);
            WriteShort(25565);
            WriteVarInt(1);
            Flush(0);
        }

        #region Read/Write methods

        internal static byte ReadByte(byte[] buffer)
        {
            var b = buffer[_offset];
            _offset += 1;
            return b;
        }

        internal static byte[] Read(byte[] buffer, int length)
        {
            var data = new byte[length];
            Array.Copy(buffer, _offset, data, 0, length);
            _offset += length;
            return data;
        }

        internal static int ReadVarInt(byte[] buffer)
        {
            var value = 0;
            var size = 0;
            int b;
            while (((b = ReadByte(buffer)) & 0x80) == 0x80)
            {
                value |= (b & 0x7F) << (size++ * 7);
                if (size > 5) throw new IOException("This VarInt is an imposter!");
            }

            return value | ((b & 0x7F) << (size * 7));
        }

        internal static string ReadString(byte[] buffer, int length)
        {
            var data = Read(buffer, length);
            return Encoding.UTF8.GetString(data);
        }

        internal static void WriteVarInt(int value)
        {
            while ((value & 128) != 0)
            {
                _buffer.Add((byte) ((value & 127) | 128));
                value = (int) (uint) value >> 7;
            }

            _buffer.Add((byte) value);
        }

        internal static void WriteShort(short value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        internal static void WriteString(string data)
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            WriteVarInt(buffer.Length);
            _buffer.AddRange(buffer);
        }

        internal static void Flush(int id = -1)
        {
            var buffer = _buffer.ToArray();
            _buffer.Clear();

            var add = 0;
            var packetData = new[] {(byte) 0x00};
            if (id >= 0)
            {
                WriteVarInt(id);
                packetData = _buffer.ToArray();
                add = packetData.Length;
                _buffer.Clear();
            }

            WriteVarInt(buffer.Length + add);
            var bufferLength = _buffer.ToArray();
            _buffer.Clear();

            _stream.WriteAsync(bufferLength, 0, bufferLength.Length);
            _stream.WriteAsync(packetData, 0, packetData.Length);
            _stream.WriteAsync(buffer, 0, buffer.Length);
        }

        #endregion
    }
}
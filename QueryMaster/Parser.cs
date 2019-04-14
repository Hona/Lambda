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

namespace QueryMaster
{
    internal class Parser
    {
        private readonly byte[] _data;
        private readonly int _lastPosition;
        private int _currentPosition = -1;

        internal Parser(byte[] data)
        {
            _data = data;
            _currentPosition = -1;
            _lastPosition = _data.Length - 1;
        }

        internal bool HasUnParsedBytes => _currentPosition <= _lastPosition;

        internal byte ReadByte()
        {
            _currentPosition++;
            if (_currentPosition > _lastPosition)
                throw new ParseException("Index was outside the bounds of the byte array.");

            return _data[_currentPosition];
        }

        internal ushort ReadUShort()
        {
            ushort num = 0;

            _currentPosition++;
            if (_currentPosition + 3 > _lastPosition)
                throw new ParseException("Unable to parse bytes to ushort.");

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(_data, _currentPosition, 2);

            num = BitConverter.ToUInt16(_data, _currentPosition);
            _currentPosition++;

            return num;
        }

        internal int ReadInt()
        {
            var num = 0;

            _currentPosition++;
            if (_currentPosition + 3 > _lastPosition)
                throw new ParseException("Unable to parse bytes to int.");

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(_data, _currentPosition, 4);

            num = BitConverter.ToInt32(_data, _currentPosition);
            _currentPosition += 3;

            return num;
        }

        internal ulong ReadULong()
        {
            ulong num = 0;

            _currentPosition++;
            if (_currentPosition + 7 > _lastPosition)
                throw new ParseException("Unable to parse bytes to ulong.");

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(_data, _currentPosition, 8);

            num = BitConverter.ToUInt64(_data, _currentPosition);
            _currentPosition += 7;

            return num;
        }

        internal float ReadFloat()
        {
            float num = 0;

            _currentPosition++;
            if (_currentPosition + 3 > _lastPosition)
                throw new ParseException("Unable to parse bytes to float.");

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(_data, _currentPosition, 4);

            num = BitConverter.ToSingle(_data, _currentPosition);
            _currentPosition += 3;

            return num;
        }

        internal string ReadString()
        {
            var str = string.Empty;
            var temp = 0;

            _currentPosition++;
            temp = _currentPosition;

            while (_data[_currentPosition] != 0x00)
            {
                _currentPosition++;
                if (_currentPosition > _lastPosition)
                    throw new ParseException("Unable to parse bytes to string.");
            }

            str = Encoding.UTF8.GetString(_data, temp, _currentPosition - temp);

            return str;
        }

        internal void SkipBytes(byte count)
        {
            _currentPosition += count;
            if (_currentPosition > _lastPosition)
                throw new ParseException("skip count was outside the bounds of the byte array.");
        }

        internal byte[] GetUnParsedBytes()
        {
            return _data.Skip(_currentPosition + 1).ToArray();
        }
    }
}
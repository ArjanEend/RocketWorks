using RocketWorks.Serialization;
using System;
using System.Text;

namespace RocketWorks.Networking
{
    /*
    // Binary stream Writer. Supports simple types, buffers, arrays, structs, and nested types
        */
    public class NetworkWriter
    {
        const int k_MaxStringLength = 1024 * 32;
        private NetworkBuffer buffer;
        static Encoding encoding;
        static byte[] stringBuffer;

        public NetworkWriter()
        {
            buffer = new NetworkBuffer();
            if (encoding == null)
            {
                encoding = new UTF8Encoding();
                stringBuffer = new byte[k_MaxStringLength];
            }
        }

        public NetworkWriter(byte[] buffer)
        {
            this.buffer = new NetworkBuffer(buffer);
            if (encoding == null)
            {
                encoding = new UTF8Encoding();
                stringBuffer = new byte[k_MaxStringLength];
            }
        }

        public short Position { get { return (short)buffer.Position; } }

        public byte[] ToArray()
        {
            var newArray = new byte[buffer.AsArraySegment().Count];
            Array.Copy(buffer.AsArraySegment().Array, newArray, buffer.AsArraySegment().Count);
            return newArray;
        }

        public byte[] AsArray()
        {
            return AsArraySegment().Array;
        }

        internal ArraySegment<byte> AsArraySegment()
        {
            return buffer.AsArraySegment();
        }

        // http://sqlite.org/src4/doc/trunk/www/varint.wiki

        public void WritePackedUInt32(UInt32 value)
        {
            if (value <= 240)
            {
                Write((byte)value);
                return;
            }
            if (value <= 2287)
            {
                Write((byte)((value - 240) / 256 + 241));
                Write((byte)((value - 240) % 256));
                return;
            }
            if (value <= 67823)
            {
                Write((byte)249);
                Write((byte)((value - 2288) / 256));
                Write((byte)((value - 2288) % 256));
                return;
            }
            if (value <= 16777215)
            {
                Write((byte)250);
                Write((byte)(value & 0xFF));
                Write((byte)((value >> 8) & 0xFF));
                Write((byte)((value >> 16) & 0xFF));
                return;
            }

            // all other values of uint
            Write((byte)251);
            Write((byte)(value & 0xFF));
            Write((byte)((value >> 8) & 0xFF));
            Write((byte)((value >> 16) & 0xFF));
            Write((byte)((value >> 24) & 0xFF));
        }

        public void WritePackedUInt64(UInt64 value)
        {
            if (value <= 240)
            {
                Write((byte)value);
                return;
            }
            if (value <= 2287)
            {
                Write((byte)((value - 240) / 256 + 241));
                Write((byte)((value - 240) % 256));
                return;
            }
            if (value <= 67823)
            {
                Write((byte)249);
                Write((byte)((value - 2288) / 256));
                Write((byte)((value - 2288) % 256));
                return;
            }
            if (value <= 16777215)
            {
                Write((byte)250);
                Write((byte)(value & 0xFF));
                Write((byte)((value >> 8) & 0xFF));
                Write((byte)((value >> 16) & 0xFF));
                return;
            }
            if (value <= 4294967295)
            {
                Write((byte)251);
                Write((byte)(value & 0xFF));
                Write((byte)((value >> 8) & 0xFF));
                Write((byte)((value >> 16) & 0xFF));
                Write((byte)((value >> 24) & 0xFF));
                return;
            }
            if (value <= 1099511627775)
            {
                Write((byte)252);
                Write((byte)(value & 0xFF));
                Write((byte)((value >> 8) & 0xFF));
                Write((byte)((value >> 16) & 0xFF));
                Write((byte)((value >> 24) & 0xFF));
                Write((byte)((value >> 32) & 0xFF));
                return;
            }
            if (value <= 281474976710655)
            {
                Write((byte)253);
                Write((byte)(value & 0xFF));
                Write((byte)((value >> 8) & 0xFF));
                Write((byte)((value >> 16) & 0xFF));
                Write((byte)((value >> 24) & 0xFF));
                Write((byte)((value >> 32) & 0xFF));
                Write((byte)((value >> 40) & 0xFF));
                return;
            }
            if (value <= 72057594037927935)
            {
                Write((byte)254);
                Write((byte)(value & 0xFF));
                Write((byte)((value >> 8) & 0xFF));
                Write((byte)((value >> 16) & 0xFF));
                Write((byte)((value >> 24) & 0xFF));
                Write((byte)((value >> 32) & 0xFF));
                Write((byte)((value >> 40) & 0xFF));
                Write((byte)((value >> 48) & 0xFF));
                return;
            }

            // all others
            {
                Write((byte)255);
                Write((byte)(value & 0xFF));
                Write((byte)((value >> 8) & 0xFF));
                Write((byte)((value >> 16) & 0xFF));
                Write((byte)((value >> 24) & 0xFF));
                Write((byte)((value >> 32) & 0xFF));
                Write((byte)((value >> 40) & 0xFF));
                Write((byte)((value >> 48) & 0xFF));
                Write((byte)((value >> 56) & 0xFF));
            }
        }

        public void Write(char value)
        {
            buffer.WriteByte((byte)value);
        }

        public void Write(byte value)
        {
            buffer.WriteByte(value);
        }

        public void Write(sbyte value)
        {
            buffer.WriteByte((byte)value);
        }

        public void Write(short value)
        {
            buffer.WriteByte2((byte)(value & 0xff), (byte)((value >> 8) & 0xff));
        }

        public void Write(ushort value)
        {
            buffer.WriteByte2((byte)(value & 0xff), (byte)((value >> 8) & 0xff));
        }

        public void Write(int value)
        {
            // little endian...
            buffer.WriteByte4(
                (byte)(value & 0xff),
                (byte)((value >> 8) & 0xff),
                (byte)((value >> 16) & 0xff),
                (byte)((value >> 24) & 0xff));
        }

        public void Write(uint value)
        {
            buffer.WriteByte4(
                (byte)(value & 0xff),
                (byte)((value >> 8) & 0xff),
                (byte)((value >> 16) & 0xff),
                (byte)((value >> 24) & 0xff));
        }

        public void Write(long value)
        {
            buffer.WriteByte8(
                (byte)(value & 0xff),
                (byte)((value >> 8) & 0xff),
                (byte)((value >> 16) & 0xff),
                (byte)((value >> 24) & 0xff),
                (byte)((value >> 32) & 0xff),
                (byte)((value >> 40) & 0xff),
                (byte)((value >> 48) & 0xff),
                (byte)((value >> 56) & 0xff));
        }

        public void Write(ulong value)
        {
            buffer.WriteByte8(
                (byte)(value & 0xff),
                (byte)((value >> 8) & 0xff),
                (byte)((value >> 16) & 0xff),
                (byte)((value >> 24) & 0xff),
                (byte)((value >> 32) & 0xff),
                (byte)((value >> 40) & 0xff),
                (byte)((value >> 48) & 0xff),
                (byte)((value >> 56) & 0xff));
        }

        static UIntFloat s_FloatConverter;

        public void Write(float value)
        {
            s_FloatConverter.floatValue = value;
            Write(s_FloatConverter.intValue);
        }

        public void Write(double value)
        {
            s_FloatConverter.doubleValue = value;
            Write(s_FloatConverter.longValue);
        }

        public void Write(decimal value)
        {
            Int32[] bits = decimal.GetBits(value);
            Write(bits[0]);
            Write(bits[1]);
            Write(bits[2]);
            Write(bits[3]);
        }

        public void Write(string value)
        {
            if (value == null)
            {
                buffer.WriteByte2(0, 0);
                return;
            }

            int len = encoding.GetByteCount(value);

            if (len >= k_MaxStringLength)
            {
                throw new IndexOutOfRangeException("Serialize(string) too long: " + value.Length);
            }

            Write((ushort)(len));
            int numBytes = encoding.GetBytes(value, 0, value.Length, stringBuffer, 0);
            buffer.WriteBytes(stringBuffer, (ushort)numBytes);
        }

        public void Write(bool value)
        {
            if (value)
                buffer.WriteByte(1);
            else
                buffer.WriteByte(0);
        }

        public void Write(byte[] buffer, int count)
        {
            if (count > UInt16.MaxValue)
            {
                RocketLog.Log("NetworkWriter Write: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes.");
                return;
            }
            this.buffer.WriteBytes(buffer, (UInt16)count);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (count > UInt16.MaxValue)
            {
                RocketLog.Log("NetworkWriter Write: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes.");
                return;
            }
            this.buffer.WriteBytesAtOffset(buffer, (ushort)offset, (ushort)count);
        }

        public void WriteBytesAndSize(byte[] buffer, int count)
        {
            if (buffer == null || count == 0)
            {
                Write((UInt16)0);
                return;
            }

            if (count > UInt16.MaxValue)
            {
                RocketLog.Log("NetworkWriter WriteBytesAndSize: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes.");
                return;
            }

            Write((UInt16)count);
            this.buffer.WriteBytes(buffer, (UInt16)count);
        }

        //NOTE: this will write the entire buffer.. including trailing empty space!
        public void WriteBytesFull(byte[] buffer)
        {
            if (buffer == null)
            {
                Write((UInt16)0);
                return;
            }
            if (buffer.Length > UInt16.MaxValue)
            {
                RocketLog.Log("NetworkWriter WriteBytes: buffer is too large (" + buffer.Length + ") bytes. The maximum buffer size is 64K bytes.");
                return;
            }
            Write((UInt16)buffer.Length);
            this.buffer.WriteBytes(buffer, (UInt16)buffer.Length);
        }

        public void Write(IRocketable value)
        {
            //value.Rocketize(this);
        }

        public void SeekZero()
        {
            buffer.SeekZero();
        }

        public void StartMessage()
        {
            SeekZero();

            // two bytes for size, will be filled out in FinishMessage
            buffer.WriteByte2(0, 0);
        }

        public void FinishMessage()
        {
            // writes correct size into space at start of buffer
            buffer.FinishMessage();
        }
    }
}

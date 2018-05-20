using System;
using System.Runtime.InteropServices;

namespace RocketWorks.Networking
{
    // A growable buffer class used by NetworkReader and NetworkWriter.
    // this is used instead of MemoryStream and BinaryReader/BinaryWriter to avoid allocations.
    public class NetworkBuffer
    {
        byte[] buffer;
        uint pos;
        const int INITIAL_SIZE = 64;
        const float GROWWTH_FACTOR = 1.5f;
        const int BUFSIZE_WARNING = 1024 * 1024 * 128;

        public uint Position { get { return pos; } }
        public int Length { get { return buffer.Length; } }

        public NetworkBuffer()
        {
            buffer = new byte[INITIAL_SIZE];
        }

        // this does NOT copy the buffer
        public NetworkBuffer(byte[] buffer)
        {
            this.buffer = buffer;
        }

        public byte ReadByte()
        {
            if (pos >= buffer.Length)
            {
                throw new IndexOutOfRangeException("NetworkReader:ReadByte out of range:" + ToString());
            }

            return buffer[pos++];
        }

        public void ReadBytes(byte[] buffer, uint count)
        {
            if (pos + count > this.buffer.Length)
            {
                throw new IndexOutOfRangeException("NetworkReader:ReadBytes out of range: (" + count + ") " + ToString());
            }

            for (ushort i = 0; i < count; i++)
            {
                buffer[i] = this.buffer[pos + i];
            }
            pos += count;
        }

        internal ArraySegment<byte> AsArraySegment()
        {
            return new ArraySegment<byte>(buffer, 0, (int)pos);
        }

        public void WriteByte(byte value)
        {
            WriteCheckForSpace(1);
            buffer[pos] = value;
            pos += 1;
        }

        public void WriteByte2(byte value0, byte value1)
        {
            WriteCheckForSpace(2);
            buffer[pos] = value0;
            buffer[pos + 1] = value1;
            pos += 2;
        }

        public void WriteByte4(byte value0, byte value1, byte value2, byte value3)
        {
            WriteCheckForSpace(4);
            buffer[pos] = value0;
            buffer[pos + 1] = value1;
            buffer[pos + 2] = value2;
            buffer[pos + 3] = value3;
            pos += 4;
        }

        public void WriteByte8(byte value0, byte value1, byte value2, byte value3, byte value4, byte value5, byte value6, byte value7)
        {
            WriteCheckForSpace(8);
            buffer[pos] = value0;
            buffer[pos + 1] = value1;
            buffer[pos + 2] = value2;
            buffer[pos + 3] = value3;
            buffer[pos + 4] = value4;
            buffer[pos + 5] = value5;
            buffer[pos + 6] = value6;
            buffer[pos + 7] = value7;
            pos += 8;
        }

        // every other Write() function in this class writes implicitly at the end-marker m_Pos.
        // this is the only Write() function that writes to a specific location within the buffer
        public void WriteBytesAtOffset(byte[] buffer, ushort targetOffset, ushort count)
        {
            uint newEnd = (uint)(count + targetOffset);

            WriteCheckForSpace((ushort)newEnd);

            if (targetOffset == 0 && count == buffer.Length)
            {
                buffer.CopyTo(this.buffer, (int)pos);
            }
            else
            {
                //CopyTo doesnt take a count :(
                for (int i = 0; i < count; i++)
                {
                    this.buffer[targetOffset + i] = buffer[i];
                }
            }

            // although this writes within the buffer, it could move the end-marker
            if (newEnd > pos)
            {
                pos = newEnd;
            }
        }

        public void WriteBytes(byte[] buffer, ushort count)
        {
            WriteCheckForSpace(count);

            if (count == buffer.Length)
            {
                buffer.CopyTo(this.buffer, (int)pos);
            }
            else
            {
                //CopyTo doesnt take a count :(
                for (int i = 0; i < count; i++)
                {
                    this.buffer[pos + i] = buffer[i];
                }
            }
            pos += count;
        }

        public void WriteCheckForSpace(ushort count)
        {
            if (pos + count < buffer.Length)
                return;

            int newLen = (int)Math.Ceiling(buffer.Length * GROWWTH_FACTOR);
            while (pos + count >= newLen)
            {
                newLen = (int)Math.Ceiling(newLen * GROWWTH_FACTOR);
                if (newLen > BUFSIZE_WARNING)
                {
                    RocketLog.Log("NetworkBuffer size is " + newLen + " bytes!", this);
                }
            }

            // only do the copy once, even if newLen is increased multiple times
            byte[] tmp = new byte[newLen];
            buffer.CopyTo(tmp, 0);
            buffer = tmp;
        }

        public void FinishMessage()
        {
            // two shorts (size and msgType) are in header.
            ushort sz = (ushort)(pos - (sizeof(ushort)));
            //Mask the byte
            buffer[0] = (byte)(sz & 0xff);
            //bit shift to get the last byte
            buffer[1] = (byte)((sz >> 8) & 0xff);
        }

        public void SeekZero()
        {
            pos = 0;
        }

        public void Replace(byte[] buffer)
        {
            this.buffer = buffer;
            pos = 0;
        }

        public override string ToString()
        {
            return String.Format("NetBuf sz:{0} pos:{1}", buffer.Length, pos);
        }
    } // end NetBuffer

    // -- helpers for float conversion --
    [StructLayout(LayoutKind.Explicit)]
    internal struct UIntFloat
    {
        [FieldOffset(0)]
        public float floatValue;

        [FieldOffset(0)]
        public uint intValue;

        [FieldOffset(0)]
        public double doubleValue;

        [FieldOffset(0)]
        public ulong longValue;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct UIntDecimal
    {
        [FieldOffset(0)]
        public ulong longValue1;

        [FieldOffset(8)]
        public ulong longValue2;

        [FieldOffset(0)]
        public decimal decimalValue;
    }

    internal class FloatConversion
    {
        public static float ToSingle(uint value)
        {
            UIntFloat uf = new UIntFloat();
            uf.intValue = value;
            return uf.floatValue;
        }

        public static double ToDouble(ulong value)
        {
            UIntFloat uf = new UIntFloat();
            uf.longValue = value;
            return uf.doubleValue;
        }

        public static decimal ToDecimal(ulong value1, ulong value2)
        {
            UIntDecimal uf = new UIntDecimal();
            uf.longValue1 = value1;
            uf.longValue2 = value2;
            return uf.decimalValue;
        }
    }
}

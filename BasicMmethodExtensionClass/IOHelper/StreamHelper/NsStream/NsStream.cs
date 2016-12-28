﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Helper.Byte;

namespace BasicMmethodExtensionClass.IOHelper.StreamHelper.NsStream
{
    /// <summary>
    /// 表示内存数据流
    /// 非线程安全类型  
    /// </summary>
    [DebuggerDisplay("Position = {Position}, Length = {Length}, Endian = {Endian}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public class NsStream : MemoryStream, INsStream
    {
        /// <summary>
        /// 获取同步锁对象
        /// </summary>
        public object SyncRoot { get; set; }

        /// <summary>
        /// 获取或设置字节存储次序
        /// 默认为Endians.Big
        /// </summary>
        public Endians Endian { get; set; }

        /// <summary>
        /// 获取用字节表示的流长度
        /// </summary>
        new public int Length
        {
            get
            {
                return (int)base.Length;
            }
        }

        /// <summary>
        /// 获取用字节表示的流长度
        /// </summary>
        public long LongLength
        {
            get
            {
                return base.Length;
            }
        }

        /// <summary>
        /// 获取或设置流中的当前位置
        /// </summary>
        new public int Position
        {
            get
            {
                return (int)base.Position;
            }
            set
            {
                base.Position = value;
            }
        }

        /// <summary>
        /// 获取或设置流中的当前位置
        /// </summary>
        public long LongPosition
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;
            }
        }

        /// <summary>
        /// 内存数据流
        /// </summary>
        public NsStream()
            : this(Endians.Big)
        {
        }

        /// <summary>
        /// 内存数据流
        /// </summary>
        /// <param name="endians">节存储次序</param>
        public NsStream(Endians endians)
        {
            SyncRoot = new object();
            Endian = endians;
        }

        /// <summary>
        /// 获取指定位置的字节
        /// </summary>
        /// <param name="index">索引</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public byte this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return GetBuffer()[index];
            }
        }

        /// <summary>
        /// 从流中读取一个字节，并将流内的位置向前推进一个字节
        /// </summary>       
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public bool ReadBoolean()
        {
            return base.ReadByte() != 0;
        }

        /// <summary>
        /// 从流中读取一个字节，并将流内的位置向前推进一个字节，如果已到达流的末尾，则返回 -1
        /// </summary>      
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        new public byte ReadByte()
        {
            return (byte)base.ReadByte();
        }

        /// <summary>
        /// 从流中读取2个字节，并将流内的位置向前推进2个字节，
        /// 返回其Int16表示类型
        /// </summary>     
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public short ReadInt16()
        {
            var range = ReadByteRange(sizeof(short));
            return ByteConverter.ToInt16(range.Buffer, range.Offset, Endian);
        }

        /// <summary>
        /// 从流中读取2个字节，并将流内的位置向前推进2个字节，
        /// 返回其UInt16表示类型
        /// </summary>      
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public uint ReadUInt16()
        {
            var range = ReadByteRange(sizeof(ushort));
            return ByteConverter.ToUInt16(range.Buffer, range.Offset, Endian);
        }

        /// <summary>
        /// 从流中读取4个字节，并将流内的位置向前推进4个字节，
        /// 返回其Int32表示类型
        /// </summary>          
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public int ReadInt32()
        {
            var range = ReadByteRange(sizeof(int));
            return ByteConverter.ToInt32(range.Buffer, range.Offset, Endian);
        }

        /// <summary>
        /// 从流中读取4个字节，并将流内的位置向前推进4个字节，
        /// 返回其UInt32表示类型
        /// </summary>     
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public uint ReadUInt32()
        {
            var range = ReadByteRange(sizeof(uint));
            return ByteConverter.ToUInt32(range.Buffer, range.Offset, Endian);
        }

        /// <summary>
        /// 从流中读取8个字节，并将流内的位置向前推进8个字节，
        /// 返回其Int64表示类型
        /// </summary>         
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public long ReadInt64()
        {
            var range = ReadByteRange(sizeof(long));
            return ByteConverter.ToInt64(range.Buffer, range.Offset, Endian);
        }

        /// <summary>
        /// 从流中读取8个字节，并将流内的位置向前推进8个字节，
        /// 返回其UInt64表示类型
        /// </summary>        
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public ulong ReadUInt64()
        {
            var range = ReadByteRange(sizeof(ulong));
            return ByteConverter.ToUInt64(range.Buffer, range.Offset, Endian);
        }

        /// <summary>
        /// 从流中读取到末尾的字节，并将流内的位置向前推进相同的字节
        /// </summary>
        /// <returns></returns>
        public byte[] ReadArray()
        {
            return ReadArray((Length - Position));
        }


        /// <summary>
        /// 从流中读取count字节，并将流内的位置向前推进count字节
        /// </summary>
        /// <param name="count">要读取的字节数</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public byte[] ReadArray(int count)
        {
            var range = ReadByteRange(count);
            var bytes = new byte[count];

            Buffer.BlockCopy(range.Buffer, range.Offset, bytes, 0, count);
            return bytes;
        }


        /// <summary>
        /// 从流中读取Position到末尾的所有字节，并将流内的位置推到末尾
        /// 返回以指定编码转换的字符串
        /// </summary>  
        /// <param name="encode">编码</param>        
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public string ReadString(Encoding encode)
        {
            return ReadString(Length - Position, encode);
        }

        /// <summary>
        /// 从流中读取count字节，并将流内的位置向前推进count字节
        /// 返回以指定编码转换的字符串
        /// </summary>        
        /// <param name="count">字节数</param>
        /// <param name="encode">编码</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public string ReadString(int count, Encoding encode)
        {
            if (encode == null)
            {
                throw new ArgumentNullException();
            }

            var range = ReadByteRange(count);
            return encode.GetString(range.Buffer, range.Offset, range.Count);
        }


        /// <summary>
        /// 从流中读取count字节的范围标记
        /// 并将流内的位置向前推进count个字节
        /// </summary>
        /// <param name="count">字节数</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IByteRange ReadByteRange(int count)
        {
            var range = new ByteRange(GetBuffer(), Position, count);
            Position = Position + count;
            return range;
        }

        /// <summary>
        /// 清空所有数据    
        /// 等同SetLength(0L)
        /// </summary>
        /// <returns></returns>
        public void Clear()
        {
            Position = 0;
            base.SetLength(0L);
        }

        /// <summary>
        /// 从开始位置清除数据        
        /// </summary>
        /// <param name="count">清除的字节数</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Clear(int count)
        {
            if (count < 0 || count > Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            var length = Length - count;
            if (length > 0)
            {
                CopyTo(count, GetBuffer(), 0, length);
            }

            Position = 0;
            base.SetLength(length);
        }

        /// <summary>
        /// 从开始位置将指定长度的数据复制到目标数组
        /// </summary>
        /// <param name="dstArray">目标数组</param>     
        /// <param name="count">要复制的字节数</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void CopyTo(byte[] dstArray, int count)
        {
            CopyTo(dstArray, 0, count);
        }

        /// <summary>
        /// 从开始位置将指定长度的数据复制到目标数组
        /// </summary>
        /// <param name="dstArray">目标数组</param>
        /// <param name="dstOffset">目标数组偏移量</param>
        /// <param name="count">要复制的字节数</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void CopyTo(byte[] dstArray, int dstOffset, int count)
        {
            CopyTo(0, dstArray, dstOffset, count);
        }

        /// <summary>
        /// 从指定偏移位置将数据复制到目标数组
        /// </summary>
        /// <param name="srcOffset">偏移量</param>
        /// <param name="dstArray">目标数组</param>
        /// <param name="dstOffset">目标数组偏移量</param>
        /// <param name="count">要复制的字节数</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void CopyTo(int srcOffset, byte[] dstArray, int dstOffset, int count)
        {
            Buffer.BlockCopy(GetBuffer(), srcOffset, dstArray, dstOffset, count);
        }

        /// <summary>
        /// 从Position位置开始查找第一个匹配的值
        /// 返回相对于Position的偏移量
        /// </summary>
        /// <param name="binary">要匹配的数据</param>
        /// <returns></returns>
        public int IndexOf(byte[] binary)
        {
            if (binary == null || binary.Length == 0)
            {
                return -1;
            }

            if (Position + binary.Length > Length)
            {
                return -1;
            }

            var maxPosition = Length - binary.Length;
            for (var p = this.Position; p <= maxPosition; p++)
            {
                if (SequenceEqual(p, binary))
                {
                    return p - Position;
                }
            }

            return -1;
        }

        /// <summary>
        /// 是否和目标binary相等
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="binary">要匹配的数据</param>
        /// <returns></returns>
        unsafe private bool SequenceEqual(int index, byte[] binary)
        {
            fixed (byte* p1 = &GetBuffer()[index], p2 = &binary[0])
            {
                for (var i = 0; i < binary.Length; i++)
                {
                    if (*(p1 + i) != *(p2 + i))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 将当前流的长度设为指定值
        /// </summary>
        /// <param name="length">长度</param>
        public void SetLength(int length)
        {
            base.SetLength(length);
        }

        /// <summary>
        /// 调试视图
        /// </summary>
        private class DebugView
        {
            /// <summary>
            /// 查看的对象
            /// </summary>
            private readonly NsStream _view;

            /// <summary>
            /// 调试视图
            /// </summary>
            /// <param name="view">查看的对象</param>
            public DebugView(NsStream view)
            {
                _view = view;
            }

            /// <summary>
            /// 查看的内容
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public byte[] Values
            {
                get
                {
                    var array = new byte[_view.Length];
                    _view.CopyTo(array, _view.Length);
                    return array;
                }
            }
        }
    }
}

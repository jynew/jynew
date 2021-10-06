/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using UnityEngine;
using System;
using System.Collections.Generic;

public class BytesBuffer
{
    static public readonly int encode_max_size = 0x2000000;//字节数组长度，十六进制为单位，总容量为32MB
    protected byte[] _buffer;
    public byte[] buffer { get { return _buffer; } }
    int _readPos;
    public int readPos { get { return _readPos; } }
    int _size;
    public int size { get { return _size; } }
    int _capacity;
    bool _sizeWarning;

	public BytesBuffer(int capacity, bool sizeWarning = true)
    {
        this._readPos = 0;
        this._size = 0;
        this._capacity = capacity;
		this._sizeWarning = sizeWarning;
		this._buffer = new byte[_capacity];
    }
    #region READ/WRITE
    public void WriteBool(bool value)
    {
        int sz = sizeof(byte);
        Expand(sz);
        _buffer[_size] = value ? (byte)1 : (byte)0;
        _size += sz;
    }
    public bool ReadBool()
    {
        bool value = _buffer[_readPos] == 1 ? true : false;
        _readPos += sizeof(byte);
        return value;
    }
    public void WriteByte(byte value)
    {
        int sz = sizeof(byte);
        Expand(sz);
        _buffer[_size] = value;
        _size += sz;
    }
    public byte ReadByte()
    {
        byte value = _buffer[_readPos];
        _readPos += sizeof(byte);
        return value;
    }
    public void WriteUInt16(ushort value)
    {
        int sz = sizeof(ushort);
        Expand(sz);
        ByteUtil.UShort2Bytes(value, _buffer, _size);
        _size += sz;
    }
    public ushort ReadUInt16()
    {
        ushort value = BitConverter.ToUInt16(_buffer, _readPos);
        _readPos += sizeof(ushort);
        return value;
    }
    public void WriteInt16(short value)
    {
        int sz = sizeof(short);
        Expand(sz);
        ByteUtil.Short2Bytes(value, _buffer, _size);
        _size += sz;
    }
    public void WriteEnum(System.Enum value)
    {
        int sz = sizeof(short);
        Expand(sz);
        ByteUtil.Short2Bytes((short)value.GetHashCode(), _buffer, _size);
        _size += sz;
    }
    public short ReadInt16()
    {
        short value = BitConverter.ToInt16(_buffer, _readPos);
        _readPos += sizeof(short);
        return value;
    }
    public void WriteUInt32(uint value)
    {
        int sz = sizeof(uint);
        Expand(sz);
        ByteUtil.UInt2Bytes(value, _buffer, _size);
        _size += sz;
    }
    public uint ReadUInt32()
    {
        uint value = BitConverter.ToUInt32(_buffer, _readPos);
        _readPos += sizeof(uint);
        return value;
    }
    public void WriteInt32(int value)
    {
        int sz = sizeof(int);
        Expand(sz);
        ByteUtil.Int2Bytes(value, _buffer, _size);
        _size += sz;
    }
    public int ReadInt32()
    {
        int value = BitConverter.ToInt32(_buffer, _readPos);
        _readPos += sizeof(int);
        return value;
    }
	public void WriteULong(ulong value)
	{
		int sz = sizeof(ulong);
		Expand(sz);
		ByteUtil.ULong2Bytes(value, _buffer, _size);
		_size += sz;
	}
	public ulong ReadULong()
	{
        ulong value = BitConverter.ToUInt64(_buffer, _readPos);
		_readPos += sizeof(ulong);
		return value;
	}
	public void WriteLong(long value)
    {
        int sz = sizeof(long);
        Expand(sz);
        ByteUtil.Long2Bytes(value, _buffer, _size);
        _size += sz;
    }
    public long ReadLong()
    {
        long value = BitConverter.ToInt64(_buffer, _readPos);
        _readPos += sizeof(long);
        return value;
    }
    public void WriteBytes(byte[] bytes, int offset, int count)
    {
        Expand(count);
        for (int i = 0; i < count; i++)
            _buffer[_size + i] = bytes[offset + i];
        _size += count;
    }
	//public void WriteCompressBytes(byte[] bytes)
	//{
 //       var uncompressbytes = UnCompress(bytes);
 //       int count = uncompressbytes.Length;
 //       Expand(count);
	//	for (int i = 0; i < count; i++)
	//		_buffer[_size + i] = uncompressbytes[i];
	//	_size += count;
	//}
    public void WriteFloat(float value)
    {
        byte[] temp = BitConverter.GetBytes(value);
        Debug.Assert(temp.Length == sizeof(float), "{0} != {1}", temp.Length, sizeof(float));
        WriteBytes(temp, 0, temp.Length);
    }
    public float ReadFloat()
    {
        float value = BitConverter.ToSingle(_buffer, _readPos);
        _readPos += sizeof(float);
        return value;
    }
    public void WriteVector2(Vector2 value)
    {
        WriteFloat(value.x);
        WriteFloat(value.y);
    }
    public Vector2 ReadVector2()
    {
        float x = ReadFloat();
        float y = ReadFloat();
        Vector2 value = new Vector2(x, y);
        return value;
    }
    public void WriteVector3(Vector3 value)
    {
        WriteFloat(value.x);
        WriteFloat(value.y);
        WriteFloat(value.z);
    }
    public Vector3 ReadVector3()
    {
        float x = ReadFloat();
        float y = ReadFloat();
        float z = ReadFloat();
        Vector3 value = new Vector3(x, y, z);
        return value;
    }
    public void WriteVector4(Vector4 value)
    {
        WriteFloat(value.x);
        WriteFloat(value.y);
        WriteFloat(value.z);
        WriteFloat(value.w);
    }
    public Vector4 ReadVector4()
    {
        float x = ReadFloat();
        float y = ReadFloat();
        float z = ReadFloat();
        float w = ReadFloat();
        Vector4 value = new Vector4(x, y, z, w);
        return value;
    }
    public void WriteString(string str)
    {
        if (str == null)
        {
            WriteUInt16(ushort.MaxValue);//字串为null则写入ushort.MaxValue
        }
        else if (str == string.Empty)//空串则写入0
        {
            WriteUInt16(0);
        }
        else
        {
            byte[] strBytes = System.Text.UTF8Encoding.UTF8.GetBytes(str);
            Debug.Assert(strBytes.Length < ushort.MaxValue, "字符串过长：{0}:{1}", strBytes.Length, str);
            ushort byteLen = (ushort)strBytes.Length;
            WriteUInt16(byteLen);//先写字节长度
            WriteBytes(strBytes, 0, byteLen);
        }
    }
    public string ReadString()
    {
        ushort byteLen = ReadUInt16();
        if (byteLen == ushort.MaxValue)
        {
            return null;
        }
        else if (byteLen > 0)
        {
            string str = System.Text.UTF8Encoding.UTF8.GetString(_buffer, _readPos, byteLen);
            _readPos += byteLen;
            return str;
        }
        else
            return string.Empty;
    }
    public void WriteColor(Color color)
    {
        WriteFloat(color.r);
        WriteFloat(color.g);
        WriteFloat(color.b);
        WriteFloat(color.a);
    }
    public Color ReadColor()
    {
        Color color = new Color();
        color.r = ReadFloat();
        color.g = ReadFloat();
        color.b = ReadFloat();
        color.a = ReadFloat();
        return color;
    }
	public void WriteColor32(Color32 color)
	{
		WriteByte(color.r);
        WriteByte(color.g);
        WriteByte(color.b);
        WriteByte(color.a);
	}
	public Color ReadColor32()
	{
		Color32 color = new Color32();
		color.r = ReadByte();
		color.g = ReadByte();
		color.b = ReadByte();
		color.a = ReadByte();
		return color;
	}
	public void WriteIntArray(int[] arr)
    {
        int count = arr == null ? 0 : arr.Length;
        WriteUInt16((ushort)count);
        for (int i = 0; i < count; i++)
            WriteInt32(arr[i]);
    }
    public int[] ReadIntArray()
    {
        int count = ReadUInt16();
        int[] arr = new int[count];
        for (int i = 0; i < count; i++)
            arr[i] = ReadInt32();
        return arr;
    }
    public void WriteBoolList(List<bool> value)
    {
        int count = value == null ? 0 : value.Count;
        WriteUInt16((ushort)count);
        for (int i = 0; i < count; i++)
            WriteBool(value[i]);
    }
    public List<bool> ReadBoolList()
    {
        int count = ReadUInt16();
        List<bool> value = new List<bool>(count);
        for (int i = 0; i < count; i++)
            value.Add(ReadBool());
        return value;
    }
    public void WriteIntList(List<int> value)
    {
        int count = value == null ? 0 : value.Count;
        WriteUInt16((ushort)count);
        for (int i = 0; i < count; i++)
            WriteInt32(value[i]);
    }
    public List<int> ReadIntList()
    {
        int count = ReadUInt16();
        List<int> value = new List<int>(count);
        for (int i = 0; i < count; i++)
            value.Add(ReadInt32());
        return value;
    }
    public void WriteIReadOnlyIntList(IReadOnlyList<int> value)
    {
        int count = value == null ? 0 : value.Count;
        WriteUInt16((ushort)count);
        for (int i = 0; i < count; i++)
            WriteInt32(value[i]);
    }
    public IReadOnlyList<int> ReadIReadOnlyIntList()
    {
        int count = ReadUInt16();
        List<int> value = new List<int>(count);
        for (int i = 0; i < count; i++)
            value.Add(ReadInt32());
        return value;
    }
    public void WriteUIntList(List<uint> value)
    {
        int count = value == null ? 0 : value.Count;
        WriteUInt16((ushort)count);
        for (int i = 0; i < count; i++)
            WriteUInt32(value[i]);
    }
    public List<uint> ReadUIntList()
    {
        int count = ReadUInt16();
        List<uint> value = new List<uint>(count);
        for (int i = 0; i < count; i++)
            value.Add(ReadUInt32());
        return value;
    }
    public void WriteLongList(List<long> value)
    {
        int count = value == null ? 0 : value.Count;
        WriteUInt16((ushort)count);
        for (int i = 0; i < count; i++)
            WriteLong(value[i]);
    }
    public List<long> ReadLongList()
    {
        int count = ReadUInt16();
        List<long> value = new List<long>(count);
        for (int i = 0; i < count; i++)
            value.Add(ReadLong());
        return value;
    }
    public void WriteFixFloatList(List<FixFloat> value)
    {
        int count = value == null ? 0 : value.Count;
        WriteUInt16((ushort)count);
        for (int i = 0; i < count; i++)
            WriteFixFloat(value[i]);
    }
    public List<FixFloat> ReadFixFloatList()
    {
        int count = ReadUInt16();
        List<FixFloat> value = new List<FixFloat>(count);
        for (int i = 0; i < count; i++)
            value.Add(ReadFixFloat());
        return value;
    }
    public void WriteStringList(List<string> value)
    {
        int count = value == null ? 0 : value.Count;
        WriteUInt16((ushort)count);
        for (int i = 0; i < count; i++)
            WriteString(value[i]);
    }
    public List<string> ReadStringList()
    {
        int count = ReadUInt16();
        List<string> value = new List<string>(count);
        for (int i = 0; i < count; i++)
            value.Add(ReadString());
        return value;
    }
    #endregion
    public void Seek(int index)
    {
        _readPos = index;
    }
    public void Clear()
    {
        _size = 0;
        _readPos = 0;
    }
    public void ShiftSize(int sz)
    {
        _size += sz;
    }
    public BytesBuffer Clone()
    {
        BytesBuffer clone = new BytesBuffer(size);
        clone.WriteBytes(buffer, 0, size);
        return clone;
    }
    public bool IsReachEnd()
    {
        return _readPos >= _size ? true : false;
    }
    void Expand(int sz)
    {
        if (_capacity - _size < sz)
        {
            long bak_sz = _capacity;
            while (_capacity - _size < sz)
            {
                _capacity = _capacity * 2;
            }
            if (_sizeWarning && _capacity >= encode_max_size)
            {
                Debug.LogError("object is too large (>" + encode_max_size + ")");
            }
            byte[] new_buffer = new byte[_capacity];
            for (long i = 0; i < bak_sz; i++)
                new_buffer[i] = _buffer[i];
            _buffer = new_buffer;
        }
    }
}

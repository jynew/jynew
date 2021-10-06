using UnityEngine;
using System.Collections;
using System;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
public class ByteUtil
{
    static public int Bool2Bytes(bool value, byte[] bytes, int offset)
    {
        bytes[offset] = value ? (byte)1 : (byte)0;
        return sizeof(byte);
    }
	static public int ULong2Bytes(ulong value, byte[] bytes, int offset)
	{
		bytes[offset] = (byte)(value & 0xff);
		bytes[offset + 1] = (byte)((value >> 8) & 0xff);
		bytes[offset + 2] = (byte)((value >> 16) & 0xff);
		bytes[offset + 3] = (byte)((value >> 24) & 0xff);
		bytes[offset + 4] = (byte)((value >> 32) & 0xff);
		bytes[offset + 5] = (byte)((value >> 40) & 0xff);
		bytes[offset + 6] = (byte)((value >> 48) & 0xff);
		bytes[offset + 7] = (byte)((value >> 56) & 0xff);
		return sizeof(ulong);
	}
	static public int Long2Bytes(long value, byte[] bytes, int offset)
    {
        bytes[offset] = (byte)(value & 0xff);
        bytes[offset + 1] = (byte)((value >> 8) & 0xff);
        bytes[offset + 2] = (byte)((value >> 16) & 0xff);
        bytes[offset + 3] = (byte)((value >> 24) & 0xff);
        bytes[offset + 4] = (byte)((value >> 32) & 0xff);
        bytes[offset + 5] = (byte)((value >> 40) & 0xff);
        bytes[offset + 6] = (byte)((value >> 48) & 0xff);
        bytes[offset + 7] = (byte)((value >> 56) & 0xff);
        return sizeof(long);
    }
    static public int UShort2Bytes(ushort value, byte[] bytes, int offset)
    {
        bytes[offset] = (byte)(value & 0xff);
        bytes[offset + 1] = (byte)((value >> 8) & 0xff);
        return sizeof(ushort);
    }
    static public int Short2Bytes(short value, byte[] bytes, int offset)
    {
        bytes[offset] = (byte)(value & 0xff);
        bytes[offset + 1] = (byte)((value >> 8) & 0xff);
        return sizeof(short);
    }
    static public int UInt2Bytes(uint value, byte[] bytes, int offset)
    {
        bytes[offset] = (byte)(value & 0xff);
        bytes[offset + 1] = (byte)((value >> 8) & 0xff);
        bytes[offset + 2] = (byte)((value >> 16) & 0xff);
        bytes[offset + 3] = (byte)((value >> 24) & 0xff);
        return sizeof(uint);
    }
    static public int Int2Bytes(int value, byte[] bytes, int offset)
    {
        bytes[offset] = (byte)(value & 0xff);
        bytes[offset + 1] = (byte)((value >> 8) & 0xff);
        bytes[offset + 2] = (byte)((value >> 16) & 0xff);
        bytes[offset + 3] = (byte)((value >> 24) & 0xff);
        return sizeof(int);
    }
    public static byte[] Compress(byte[] bytes, int offset, int length)
    {
        MemoryStream ms = new MemoryStream();
        GZipOutputStream compressedzipStream = new GZipOutputStream(ms);
        compressedzipStream.Write(bytes, offset, length);
        compressedzipStream.Close();
        return ms.ToArray();
    }

    public static byte[] UnCompress(byte[] byteArray, int offset, int length)
    {
        GZipInputStream gzi = new GZipInputStream(new MemoryStream(byteArray, offset, length));
        MemoryStream re = new MemoryStream(50000);
        int count;
        byte[] data = new byte[50000];
        while ((count = gzi.Read(data, 0, data.Length)) != 0)
            re.Write(data, 0, count);
        byte[] overarr = re.ToArray();
        return overarr;
    }
}

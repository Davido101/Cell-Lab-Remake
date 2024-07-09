using Ookii.Dialogs;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BinaryHandler : MonoBehaviour
{
    BinaryReader reader;
    Stream stream;

    public BinaryHandler(string path)
    {
        FileStream fileStream = File.OpenRead(path);
        reader = new BinaryReader(fileStream);
        stream = fileStream;
    }

    public BinaryHandler(byte[] bytes)
    {
        MemoryStream memoryStream = new MemoryStream(bytes);
        reader = new BinaryReader(memoryStream);
        stream = memoryStream;
    }

    public bool ReadBool()
    {
        return reader.ReadBoolean();
    }

    public short ReadShort()
    {
        byte[] short_bytes = new byte[2];
        reader.Read(short_bytes, 0, 2);
        if (BitConverter.IsLittleEndian)
            short_bytes = short_bytes.Reverse().ToArray();
        return BitConverter.ToInt16(short_bytes);
    }

    public int ReadInt()
    {
        byte[] int_bytes = new byte[4];
        reader.Read(int_bytes, 0, 4);
        if (BitConverter.IsLittleEndian)
            int_bytes = int_bytes.Reverse().ToArray();
        return BitConverter.ToInt32(int_bytes);
    }

    public float ReadFloat()
    {
        byte[] float_bytes = new byte[4];
        reader.Read(float_bytes, 0, 4);
        if (BitConverter.IsLittleEndian)
            float_bytes = float_bytes.Reverse().ToArray();
        return BitConverter.ToSingle(float_bytes);
    }

    public double ReadDouble()
    {
        byte[] double_bytes = new byte[8];
        reader.Read(double_bytes, 0, 8);
        if (BitConverter.IsLittleEndian)
            double_bytes = double_bytes.Reverse().ToArray();
        return BitConverter.ToDouble(double_bytes);
    }

    public byte[] ReadAll()
    {
        byte[] bytes = new byte[reader.BaseStream.Length - reader.BaseStream.Position];
        reader.Read(bytes, 0, Convert.ToInt32(reader.BaseStream.Length - reader.BaseStream.Position));
        return bytes;
    }

    public byte[] Read(int bytes)
    {
        byte[] read = reader.ReadBytes(bytes);
        return read;
    }

    public bool Verify(byte b)
    {
        return b == reader.ReadByte();
    }

    public bool Verify(byte[] seq)
    {
        byte[] bytes = reader.ReadBytes(seq.Length);
        return seq.SequenceEqual(bytes);
    }

    public byte? ReadByte()
    {
        byte[] bytes = reader.ReadBytes(1);
        if (bytes.Length != 1)
            return null;
        else
            return bytes[0];
    }

    public void Overwrite(byte[] data)
    {
        MemoryStream memoryStream = new MemoryStream(data);
        reader.Close();
        reader.Dispose();
        stream.Close();
        stream.Dispose();
        reader = new BinaryReader(memoryStream);
        stream = memoryStream;
    }

    public void Discard(int count)
    {
        reader.ReadBytes(count);
    }

    ~BinaryHandler()
    {
        reader.Close();
        reader.Dispose();
        stream.Close();
        stream.Dispose();
    }
}

using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class FileOperationProhibited : Exception
{
    public FileOperationProhibited()
    {

    }

    public FileOperationProhibited(string message) : base(message)
    {

    }
}

public class BinaryHandler : MonoBehaviour
{
    BinaryReader reader;
    BinaryWriter writer;
    Stream stream;
    bool read;

    public BinaryHandler()
    {
        this.read = false;
        MemoryStream memoryStream = new MemoryStream();
        writer = new BinaryWriter(memoryStream);
        stream = memoryStream;
    }

    public BinaryHandler(string path, bool read)
    {
        this.read = read;
        if (read)
        {
            FileStream fileStream = File.OpenRead(path);
            reader = new BinaryReader(fileStream);
            stream = fileStream;
        }
        else
        {
            FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            writer = new BinaryWriter(fileStream);
            stream = fileStream;
        }
    }

    public BinaryHandler(byte[] bytes)
    {
        MemoryStream memoryStream = new MemoryStream(bytes);
        reader = new BinaryReader(memoryStream);
        stream = memoryStream;
    }

    public bool ReadBool()
    {
        if (!read)
            throw new FileOperationProhibited("Can't read bool when file was opened for writing.");
        return reader.ReadBoolean();
    }

    public short ReadShort()
    {
        if (!read)
            throw new FileOperationProhibited("Can't read short when file was opened for writing.");
        byte[] short_bytes = new byte[2];
        reader.Read(short_bytes, 0, 2);
        if (BitConverter.IsLittleEndian)
            short_bytes = short_bytes.Reverse().ToArray();
        return BitConverter.ToInt16(short_bytes);
    }

    public int ReadInt()
    {
        if (!read)
            throw new FileOperationProhibited("Can't read int when file was opened for writing.");
        byte[] int_bytes = new byte[4];
        reader.Read(int_bytes, 0, 4);
        if (BitConverter.IsLittleEndian)
            int_bytes = int_bytes.Reverse().ToArray();
        return BitConverter.ToInt32(int_bytes);
    }

    public float ReadFloat()
    {
        if (!read)
            throw new FileOperationProhibited("Can't read float when file was opened for writing.");
        byte[] float_bytes = new byte[4];
        reader.Read(float_bytes, 0, 4);
        if (BitConverter.IsLittleEndian)
            float_bytes = float_bytes.Reverse().ToArray();
        return BitConverter.ToSingle(float_bytes);
    }

    public double ReadDouble()
    {
        if (!read)
            throw new FileOperationProhibited("Can't read double when file was opened for writing.");
        byte[] double_bytes = new byte[8];
        reader.Read(double_bytes, 0, 8);
        if (BitConverter.IsLittleEndian)
            double_bytes = double_bytes.Reverse().ToArray();
        return BitConverter.ToDouble(double_bytes);
    }

    public byte[] ReadAll()
    {
        if (!read)
            throw new FileOperationProhibited("Can't read all when file was opened for writing.");
        byte[] bytes = new byte[stream.Length - stream.Position];
        stream.Read(bytes, 0, Convert.ToInt32(stream.Length - stream.Position));
        return bytes;
    }

    public byte[] Read(int bytes)
    {
        if (!this.read)
            throw new FileOperationProhibited("Can't read when file was opened for writing.");
        byte[] read = reader.ReadBytes(bytes);
        return read;
    }

    public bool Verify(byte b)
    {
        if (!read)
            throw new FileOperationProhibited("Can't verify when file was opened for writing.");
        return b == reader.ReadByte();
    }

    public bool Verify(byte[] seq)
    {
        if (!read)
            throw new FileOperationProhibited("Can't verify when file was opened for writing.");
        byte[] bytes = reader.ReadBytes(seq.Length);
        return seq.SequenceEqual(bytes);
    }

    public byte? ReadByte()
    {
        if (!read)
            throw new FileOperationProhibited("Can't read byte when file was opened for writing.");
        byte[] bytes = reader.ReadBytes(1);
        if (bytes.Length != 1)
            return null;
        else
            return bytes[0];
    }

    public void Overwrite(byte[] data)
    {
        if (!read)
            throw new FileOperationProhibited("Can't overwrite when file was opened for writing.");
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
        if (!read)
            throw new FileOperationProhibited("Can't discard when file was opened for writing.");
        reader.ReadBytes(count);
    }

    public void WriteBool(bool value)
    {
        if (read)
            throw new FileOperationProhibited("Can't write bool when file was opened for reading.");
        writer.Write(value);
    }

    public void WriteShort(short value)
    {
        if (read)
            throw new FileOperationProhibited("Can't write short when file was opened for reading.");
        writer.Write(Binary.ReverseEndianness(value));
    }

    public void WriteInt(int value)
    {
        if (read)
            throw new FileOperationProhibited("Can't write int when file was opened for reading.");
        writer.Write(Binary.ReverseEndianness(value));
    }

    public void WriteFloat(float value)
    {
        if (read)
            throw new FileOperationProhibited("Can't write float when file was opened for reading.");
        writer.Write(Binary.ReverseEndianness(value));
    }

    public void WriteDouble(double value)
    {
        if (read)
            throw new FileOperationProhibited("Can't write double when file was opened for reading.");
        writer.Write(Binary.ReverseEndianness(value));
    }

    public void WriteBytes(byte[] bytes)
    {
        if (read)
            throw new FileOperationProhibited("Can't write bytes when file was opened for reading.");
        writer.Write(bytes, 0, bytes.Length);
    }

    public void WriteByte(byte b)
    {
        if (read)
            throw new FileOperationProhibited("Can't write byte when file was opened for reading.");
        writer.Write(b);
    }

    public byte[] GetData()
    {
        byte[] bytes = new byte[stream.Length];
        stream.Position = 0;
        stream.Read(bytes, 0, (int)stream.Length);
        return bytes;
    }

    public void Flush()
    {
        if (read)
            throw new FileOperationProhibited("Can't flush when file was opened for reading.");
        writer.Flush();
    }

    public void Close()
    {
        if (read)
        {
            reader.Close();
            reader.Dispose();
        }
        else
        {
            writer.Close();
            writer.Dispose();
        }
        stream.Close();
        stream.Dispose();
    }

    ~BinaryHandler()
    {
        if (read)
        {
            reader.Close();
            reader.Dispose();
        }
        else
        {
            writer.Close();
            writer.Dispose();
        }
        stream.Close();
        stream.Dispose();
    }
}

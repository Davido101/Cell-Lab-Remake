using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Transactions;
using TMPro;
using UnityEngine;

public class Binary : MonoBehaviour
{
    public static bool ReverseEndianness(bool value)
    {
        return value;
    }

    public static short ReverseEndianness(short value)
    {
        return BitConverter.ToInt16(BitConverter.GetBytes(value).Reverse().ToArray());
    }

    public static int ReverseEndianness(int value)
    {
        return BitConverter.ToInt32(BitConverter.GetBytes(value).Reverse().ToArray());
    }

    public static float ReverseEndianness(float value)
    {
        return BitConverter.ToSingle(BitConverter.GetBytes(value).Reverse().ToArray());
    }

    public static double ReverseEndianness(double value)
    {
        return BitConverter.ToDouble(BitConverter.GetBytes(value).Reverse().ToArray());
    }

    public static byte[] ConvertToJavaStream(byte[] data)
    {
        List<byte> javaStream = new List<byte>();
        int index = 0;

        javaStream.Add(0xac);
        javaStream.Add(0xed);
        javaStream.Add(0x00);
        javaStream.Add(0x05);
        while (true)
        {
            javaStream.Add(0x7a);
            int bytesLeft = data.Length - index;
            if (bytesLeft > 1024)
            {
                javaStream.Add(0x00);
                javaStream.Add(0x00);
                javaStream.Add(0x04);
                javaStream.Add(0x00);
                for (int i = 0; i < 1024; i++)
                {
                    javaStream.Add(data[index]);
                    index++;
                }
            }
            else
            {
                byte[] length = BitConverter.GetBytes(bytesLeft).Reverse().ToArray();
                javaStream.Add(length[0]);
                javaStream.Add(length[1]);
                javaStream.Add(length[2]);
                javaStream.Add(length[3]);
                for (int i = 0; i < bytesLeft; i++)
                {
                    javaStream.Add(data[index]);
                    index++;
                }
                return javaStream.ToArray();
            }
        }
    }
}

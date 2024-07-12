using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;

public class GZip : MonoBehaviour
{
    public static byte[] Decompress(byte[] input)
    {
        using (MemoryStream source = new MemoryStream(input))
        {
            using (GZipStream zip = new GZipStream(source, CompressionMode.Decompress))
            {
                List<byte> result = new List<byte>();
                byte[] buffer = new byte[1024 * 32];
                int bytesRead;

                while ((bytesRead = zip.Read(buffer, 0, buffer.Length)) > 0)
                {
                    buffer.ToList().ForEach(b => result.Add(b));
                }
                return result.ToArray();
            }
        }
    }

    public static byte[] Compress(byte[] input)
    {
        using (MemoryStream result = new MemoryStream())
        {
            using (GZipStream zip = new GZipStream(result, CompressionMode.Compress))
            {
                zip.Write(input, 0, input.Length);
                zip.Flush();
            }
            return result.ToArray();
        }
    }
}

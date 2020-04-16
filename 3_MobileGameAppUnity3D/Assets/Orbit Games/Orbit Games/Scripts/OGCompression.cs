using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

public static class OGCompression {
    /// <summary>
    /// Compresses the string.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    public static string Compress(this string text)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        var memoryStream = new MemoryStream();
        using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
        {
            gZipStream.Write(buffer, 0, buffer.Length);
        }

        memoryStream.Position = 0;

        var compressedData = new byte[memoryStream.Length];
        memoryStream.Read(compressedData, 0, compressedData.Length);

        var gZipBuffer = new byte[compressedData.Length + 4];
        Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
        return Convert.ToBase64String(gZipBuffer);
    }

    /// <summary>
    /// Decompresses the string.
    /// </summary>
    /// <param name="compressedText">The compressed text.</param>
    /// <returns></returns>
    public static string Decompress(this string compressedText)
    {
        byte[] gZipBuffer = Convert.FromBase64String(compressedText);
        using (var memoryStream = new MemoryStream())
        {
            int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
            memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

            var buffer = new byte[dataLength];

            memoryStream.Position = 0;
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                gZipStream.Read(buffer, 0, buffer.Length);
            }

            return Encoding.UTF8.GetString(buffer);
        }
    }


    /// <summary>
    /// Compresses the string.
    /// https://stackoverflow.com/questions/7343465/compression-decompression-string-with-c-sharp
    /// http://gigi.nullneuron.net/gigilabs/compressing-strings-using-gzip-in-c/
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    //public static string CompressGZip(this string text)
    //{
    //    byte[] buffer = Encoding.UTF8.GetBytes(text);
    //    return buffer.CompressGZip();
    //}
    //public static string CompressGZip(this byte[] buffer)
    //{
    //    var memoryStream = new MemoryStream();
    //    using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
    //    {
    //        gZipStream.Write(buffer, 0, buffer.Length);
    //    }

    //    var outputBytes = memoryStream.ToArray();
    //    var outputbase64 = Convert.ToBase64String(outputBytes);
    //    return outputbase64;
    //}
}

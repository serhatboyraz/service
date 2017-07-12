using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Anketbaz.Database.Helper
{
    public class Compression
    {
        #region Zlib

        //public static byte[] ZlibCompress(string data)
        //{
        //    if (zlib == null)
        //        zlib = new Zlib();

        //    byte[] dataArray = Encoding.UTF8.GetBytes(data);
        //    return zlib.Deflate(dataArray);
        //}

        //public static string ZlibDeCompress(byte[] compressedData)
        //{
        //    if (zlib == null)
        //        zlib = new Zlib();

        //    byte[] decompressdata = zlib.Inflate(compressedData, compressedData.Length);
        //    return System.Text.UTF8Encoding.UTF8.GetString(decompressdata);
        //}

        #endregion

        #region DeflateStream

        public static byte[] Compress(string data)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            MemoryStream ms = new MemoryStream();
            DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress);
            ds.Write(byteData, 0, byteData.Length);
            ds.Flush();
            ds.Close();
            return ms.ToArray();
        }

        public static string Decompress(byte[] data)
        {
            const int BUFFER_SIZE = 256;
            byte[] tempArray = new byte[BUFFER_SIZE];
            List<byte[]> tempList = new List<byte[]>();
            int count = 0, length = 0;

            MemoryStream ms = new MemoryStream(data);
            DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress);

            while ((count = ds.Read(tempArray, 0, BUFFER_SIZE)) > 0)
            {
                if (count == BUFFER_SIZE)
                {
                    tempList.Add(tempArray);
                    tempArray = new byte[BUFFER_SIZE];
                }
                else
                {
                    byte[] temp = new byte[count];
                    Array.Copy(tempArray, 0, temp, 0, count);
                    tempList.Add(temp);
                }
                length += count;
            }

            byte[] retVal = new byte[length];

            count = 0;
            foreach (byte[] temp in tempList)
            {
                Array.Copy(temp, 0, retVal, count, temp.Length);
                count += temp.Length;
            }

            return Encoding.UTF8.GetString(retVal);

            //return retVal;
        }

        public static byte[] DeflateCompress(string str)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream gzip = 
                    new DeflateStream(output, CompressionMode.Compress))
                {
                    using (StreamWriter writer = 
                        new StreamWriter(gzip, System.Text.Encoding.UTF8))
                    {
                        writer.Write(str);
                    }
                }

                return output.ToArray();
            }
        }

        public static string DeflateDeCompress(byte[] input)
        {
            using (MemoryStream inputStream = new MemoryStream(input))
            {
                using (DeflateStream gzip = 
                    new DeflateStream(inputStream, CompressionMode.Decompress))
                {
                    using (StreamReader reader = 
                        new StreamReader(gzip, System.Text.Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        #endregion

        #region GZip

        public static byte[] GZipCompress(string input)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            {
                return Compress(stream);
            }
        }

        private static byte[] Compress(Stream input)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                input.CopyTo(zipStream);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }

        public static string GZipDecompress(byte[] data)
        {
            var output = new MemoryStream();
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                zipStream.CopyTo(output);
                zipStream.Close();
                output.Position = 0;
                //return output;
            }

            using (var reader = new StreamReader(output))
            {
                string decompressedValue = reader.ReadToEnd();

                return decompressedValue;
            }
        }

        #endregion

        #region FastCompression

        public static string GetCompressedData(string data)
        {
            return Convert.ToBase64String(Compression.Compress(data));
        }

        public static string GetDeflatedData(string data)
        {
            return Compression.DeflateDeCompress(Convert.FromBase64String(data));
        }


        #endregion
    }
}

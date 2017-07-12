using System;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Anketbaz.Database.Helper
{
    public static class Helper
    {
        public static bool IsBase64(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return false;

            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        public static string GetResult(bool result, object data,bool isCompress = true)
        {
            dynamic resultObject = new ExpandoObject();
            resultObject.Result = result;
            resultObject.Data = data;
            string resultString =  JsonConvert.SerializeObject(resultObject);
            if (isCompress)
            {
                string compressedString = Compression.GetCompressedData(resultString);
                resultObject = null;
                resultString = null;
                return compressedString;
            }
            return resultString;
        }

        /// <summary>
        /// verilen tarih formatını yyyMMdd string formatında dönüş yapar
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToStringYyyyMMdd(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd");
        }

        /// <summary>
        /// verilen tarih formatını yyyMMdd string formatında dönüş yapar
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToStringHhmmss(this DateTime dateTime)
        {
            return dateTime.ToString("HHmmss");
        }

        /// <summary>
        /// verilen tarih formatını yyyMMdd string formatında dönüş yapar
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToStringYyyyMMddHhmmss(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmmss");
        }

        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GetMd5Hash(this string text)
        {
            if (text == null)
                return string.Empty;
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        public static string Control(this object o)
        {
            if (o == null)
                return string.Empty;
            return o.ToString();
        }
        public static bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                System.IO.FileStream fileStream =
                   new System.IO.FileStream(fileName, System.IO.FileMode.Create,
                                            System.IO.FileAccess.Write);

                fileStream.Write(byteArray, 0, byteArray.Length);

                fileStream.Close();

                return true;
            }
            catch (Exception exception)
            {
                // ignored
            }


            return false;
        }

        public static string SetImageFolder(string clientid)
        {
            string directory = ConfigurationManager.AppSettings["ImagePath"];
            string path = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;

            path = path + directory;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path += "//"+clientid;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
        public static void RemoveFile(string clientid,string fileName)
        {
            string directory = ConfigurationManager.AppSettings["ImagePath"];
            string path = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;

            path = path + directory;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path += "//"+clientid;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string filePath = path + "//" + fileName;
            if (File.Exists(filePath))
                File.Delete(filePath);            
        }
        public static string GetImageUrl(string clientid,string imageName)
        {
            string url = ConfigurationManager.AppSettings["ImageDomain"];
            url = string.Format("http://{0}/{1}/{2}", url, clientid, imageName);
            return url;
        }
    }
}

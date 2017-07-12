using System;
using System.IO;
using System.ServiceModel;
using System.Text;
using Anketbaz.Database.Helper;
using Newtonsoft.Json;

namespace Anketbaz.Service.FileService
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
                     InstanceContextMode = InstanceContextMode.PerCall,
                     IgnoreExtensionDataObject = true,
                     IncludeExceptionDetailInFaults = true)]

    public class FileService : IFileService
    {
        public string SetFile(Stream fileData, string jsonParams)
        {
            string paramters = Encoding.UTF8.GetString(Convert.FromBase64String(jsonParams));
            dynamic methodParameters = JsonConvert.DeserializeObject(paramters);
            string clientid = methodParameters.clientid;
            string filename = Guid.NewGuid() + ".jpg";

            string path = string.Format("{0}/{1}", Helper.SetImageFolder(clientid), filename);

            byte[] fileByteData = null;

            using (MemoryStream ms = new MemoryStream())
            {
                fileData.CopyTo(ms);
                fileByteData = ms.ToArray();
            }

            Helper.ByteArrayToFile(path, fileByteData);

            return Helper.GetResult(true, Helper.GetImageUrl(clientid, filename));
        }

        public string RemoveFile(string jsonParams)
        {
            string paramters = Encoding.UTF8.GetString(Convert.FromBase64String(jsonParams));
            dynamic methodParameters = JsonConvert.DeserializeObject(paramters);
            string clientid = methodParameters.clientid;
            string url = methodParameters.url;
            if (string.IsNullOrEmpty(url))
                return Helper.GetResult(false, "0x0028");
            string[] urlParams = url.Split('/');
            if (urlParams.Length > 0)
            {
                string fileid = urlParams[urlParams.Length - 1];
                Helper.RemoveFile(clientid, fileid);
            }
            return Helper.GetResult(true, true);
        }
    }
}

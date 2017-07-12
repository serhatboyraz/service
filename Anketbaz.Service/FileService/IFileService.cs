using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Anketbaz.Service.FileService
{
    [ServiceContract]
    public interface IFileService
    {
        [OperationContract]
        [DataContractFormat]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "SetFile/{jsonParams}")]
        string SetFile(Stream fileData, string jsonParams);

        [OperationContract]
        [DataContractFormat]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "RemoveFile/{jsonParams}")]
        string RemoveFile(string jsonParams);
    }
}

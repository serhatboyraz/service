using System.ServiceModel;
using System.ServiceModel.Web;

namespace Anketbaz.Service.Core
{
    [ServiceContract]
    public interface IDynamicService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "CallService?serviceName={serviceName}&methodName={methodName}&jsonParams={jsonParams}")]
        string CallService(string serviceName, string methodName, string jsonParams);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
             UriTemplate = "Test/{sayi}")]
        string Test(string sayi);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        string CallServicePost(string serviceName, string methodName, string jsonParams);

    }
}

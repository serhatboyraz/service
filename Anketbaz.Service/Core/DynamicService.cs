using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Anketbaz.Data;
using Anketbaz.Data.Logical;
using Anketbaz.Database.Helper;
using Anketbaz.Database.Log;
using Newtonsoft.Json;

namespace Anketbaz.Service.Core
{
    public class DynamicService : IDynamicService
    {
        public string CallService(string serviceName, string methodName, string jsonParams)
        {
            if (!jsonParams.IsBase64())
                return Helper.GetResult(false, "0x0001");

            Type classType = Type.GetType(string.Format("Anketbaz.Data.Services.{0}, Anketbaz.Data", serviceName));

            if (classType == null)
                return Helper.GetResult(false, "0x0002");

            object serviceInstance = Activator.CreateInstance(classType, null);


            MethodInfo method = classType.GetMethod(methodName);

            if (method == null)
                return Helper.GetResult(false, "0x0003");

            try
            {


                string paramters = Encoding.UTF8.GetString(Convert.FromBase64String(jsonParams));
                dynamic methodParameters = JsonConvert.DeserializeObject(paramters);

                OperationContext context = OperationContext.Current;
                MessageProperties messageProperties = context.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpointProperty =
                    messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                if (endpointProperty != null) methodParameters.ip = endpointProperty.Address;

                return method.Invoke(serviceInstance, new object[] { methodParameters }).ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + (ex.InnerException != null ? ex.InnerException.Message : string.Empty) +
                          FlattenException(ex));
                return Helper.GetResult(false, "0x0000");
            }


        }

        public string Test(string sayi)
        {
            int i = Convert.ToInt32(sayi);
            TestService.Add(i);

            try
            {
                var s = Anketbaz.Data.EntityConnectionService.User.GetList(x => x.authkey == "test");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + ex.InnerException.Message + ex.StackTrace);
            }

            return "Hello World!";
        }

        public string CallServicePost(string serviceName, string methodName, string jsonParams)
        {
            if (!jsonParams.IsBase64())
                return Helper.GetResult(false, "0x0001");

            Type classType = Type.GetType(string.Format("Anketbaz.Data.Services.{0}, Anketbaz.Data", serviceName));

            if (classType == null)
                return Helper.GetResult(false, "0x0002");

            object serviceInstance = Activator.CreateInstance(classType, null);


            MethodInfo method = classType.GetMethod(methodName);

            if (method == null)
                return Helper.GetResult(false, "0x0003");

            try
            {
                string paramters = Encoding.UTF8.GetString(Convert.FromBase64String(jsonParams));
                dynamic methodParameters = JsonConvert.DeserializeObject(paramters);

                OperationContext context = OperationContext.Current;
                MessageProperties messageProperties = context.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpointProperty =
                  messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                if (endpointProperty != null) methodParameters.ip = endpointProperty.Address;


                return method.Invoke(serviceInstance, new object[] { methodParameters }).ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + (ex.InnerException != null ? ex.InnerException.Message : string.Empty) + FlattenException(ex));
                return Helper.GetResult(false, "0x0000");
            }


        }

        private static string FlattenException(Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);

                exception = exception.InnerException;
            }

            return stringBuilder.ToString();
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DynamicWebApi
{
    public class ProxyImplementation : DynamicObject
    {
        private Type _targetType;
        private string _methodName;
        private string _url;
        private object _data;
        private string _httpVerb;

        public ProxyImplementation(Type type, string methodName, string url, string httpVerb, object data)
        {
            _targetType = type;
            _url = url;
            _methodName = methodName;
            _httpVerb = httpVerb;
            _data = data;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (binder.Name != _methodName)
            {
                throw new Exception(string.Format("Invalid method call {0}", _methodName));
            }

            var url = _url;
            var data = _data ?? string.Empty;

            for (var i = 0; i < binder.CallInfo.ArgumentCount; i++)
            {
                url = url.Replace("{" + i + "}", Uri.EscapeDataString(args[i].ToString()));
            }

            var response = TaskHandler(url, data);

            var methodReturnType = _targetType.GetMethod(binder.Name).ReturnType;

            var resultText = response.Content.ReadAsStringAsync().Result;

            resultText = Regex.Unescape(resultText).TrimStart('"').TrimEnd('"');

            try
            {
                result = (methodReturnType == typeof(String) || methodReturnType == typeof(void)) 
                                ? resultText : JsonConvert.DeserializeObject(resultText, methodReturnType);

                return true;
            }
            catch (Exception e)
            {
                var message = string.Format("Error deserializing {0}", resultText);

                throw new Exception(message, e);
            }
        }

        private Task<HttpResponseMessage> RequestHandler(string url, object data)
        {
            Task<HttpResponseMessage> task = null;

            try
            {
                if (_httpVerb == HTTPVerbs.GET)
                {
                    task = new HttpClient().GetAsync(url);
                }
                else if (_httpVerb == HTTPVerbs.DELETE)
                {
                    task = new HttpClient().DeleteAsync(url);
                }
                else
                {
                    var content = JsonConvert.SerializeObject(data);

                    HttpContent contentPost = new StringContent(content, Encoding.Default, "application/json");

                    var client = new HttpClient();

                    if (_httpVerb == HTTPVerbs.POST)
                    {
                        task = client.PostAsync(url, contentPost);

                    }
                    else
                    {
                        task = client.PutAsync(url, contentPost);
                    }
                }

                task.Wait();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return task;
        }

        private HttpResponseMessage TaskHandler(string url, object data)
        {
            HttpResponseMessage response;

            try
            {
                response = RequestHandler(url, data).Result;
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                var message = string.Format("Error connecting to {0}", url);

                throw new Exception(message, e);
            }

            return response;
        }
    }
}

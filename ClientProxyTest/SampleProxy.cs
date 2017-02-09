using DynamicWebApi;

namespace ClientProxyTest
{
    public class SampleProxy
    {
        public string GetInformation(string name)
        {
            var proxyInstance = ProxyFactory<ISample>
                .ForMethod(nameof(GetInformation))
                .ConnectToWebApi("https://www.google.com/")
                .GetInstance();

            var result = proxyInstance.GetInformation(name);

            return result;
        }
    }    
}

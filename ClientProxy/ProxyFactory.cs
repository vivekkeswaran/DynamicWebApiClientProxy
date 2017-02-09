using ImpromptuInterface;

namespace DynamicWebApi
{
    public class ProxyFactory<T> where T : class
    {
        private string _methodName;
        private string _url;
        private string _httpVerb = HTTPVerbs.GET;
        private object _data;

        public static ProxyFactory<T> ForMethod(string name)
        {
            var p = new ProxyFactory<T>();

            p._methodName = name;

            return p;
        }

        public ProxyFactory<T> ConnectToWebApi(string url, bool isDelete = false)
        {
            _url = url;
            _httpVerb = (isDelete) ? HTTPVerbs.DELETE : HTTPVerbs.GET;

            return this;
        }

        public ProxyFactory<T> UsingPostData(object postData)
        {
            _data = postData;
            _httpVerb = HTTPVerbs.POST;

            return this;
        }

        public ProxyFactory<T> UsingPutData(object putData)
        {
            _data = putData;
            _httpVerb = HTTPVerbs.PUT;

            return this;
        }

        public T GetInstance()
        {
            var p = new ProxyImplementation(typeof(T), _methodName, _url, _httpVerb, _data);

            var r = p.ActLike<T>();

            return r;
        }

    }

}

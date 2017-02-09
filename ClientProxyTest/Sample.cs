using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientProxyTest
{
    public class Sample : ISample
    {
        public string GetInformation(string name)
        {
            return string.Format("Hi, {0}", name);
        }
    }
}

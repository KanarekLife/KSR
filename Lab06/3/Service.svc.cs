using System.IO;
using System.Xml;

namespace _3
{
    // http://localhost:30703/Service.svc/index.html
    public class Service : IService
    {
        public int Dodaj(string a, string b)
        {
            return int.Parse(a) + int.Parse(b);
        }

        public XmlDocument Index()
        {
            var xml = new XmlDocument();
            xml.Load("D:\\KSR\\Lab06\\3\\wwwroot\\index.xhtml");
            return xml;
        }

        public Stream Script()
        {
            return File.OpenRead("D:\\KSR\\Lab06\\3\\wwwroot\\scripts.js");
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace _5
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        [WebGet(UriTemplate = "index.html")]
        [XmlSerializerFormat]
        XmlDocument Index();

        [OperationContract]
        [WebGet(UriTemplate = "scripts.js")]
        Stream Script();

        [OperationContract]
        [WebInvoke(UriTemplate = "Dodaj/{a}/{b}")]
        int Dodaj(string a, string b);
    }

    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ChannelFactory<IService>(new WebHttpBinding(), new EndpointAddress("http://localhost:30703/Service.svc/"));
            factory.Endpoint.EndpointBehaviors.Add(new WebHttpBehavior());
            var channel = factory.CreateChannel();
            Console.WriteLine($"(\"5\", \"3\") => {channel.Dodaj("5", "3")}");
            factory.Close();
            Console.ReadLine();
        }
    }
}

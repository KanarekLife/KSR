using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using KSR_WCF1;

namespace KSR_WCF1
{
    [ServiceContract]
    public interface IZadanie2
    {
        [OperationContract]
        string Test(string arg);
    }

    [ServiceContract]
    public interface IZadanie7
    {
        [OperationContract]
        [FaultContract(typeof(Wyjatek7))]
        void RzucWyjatek7(string a, int b);

    }

    [DataContract]
    public class Wyjatek7
    {
        [DataMember]
        public string opis;
        
        [DataMember]
        public string a;
        
        [DataMember]
        public int b;
    }
}

namespace _2
{
    class Server : IZadanie2, IZadanie7
    {
        public string Test(string arg)
        {
            Console.WriteLine($"Received: {arg}");
            return arg.ToUpperInvariant();
        }

        public void RzucWyjatek7(string a, int b)
        {
            throw new FaultException<Wyjatek7>(new Wyjatek7
            {
                a = a,
                b = b,
                opis = $"Zadanie 7: {a}, {b}"
            }, new FaultReason("Wyjatek 7 received"));
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Make sure to run with admin rights: netsh http add urlacl url=http://+:1100/ user=<user>

            // For task 3 URL in Add Service window is: net.pipe://localhost/metadane

            var host = new ServiceHost(typeof(Server), new [] {new Uri("http://localhost:1100"), new Uri("net.tcp://127.0.0.1:55765")});
            var b = host.Description.Behaviors.Find<ServiceMetadataBehavior>() ?? new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(b);
            host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexNamedPipeBinding(), "net.pipe://localhost/metadane");
            host.AddServiceEndpoint(typeof(IZadanie2), new NetNamedPipeBinding(), "net.pipe://localhost/ksr-wcf1-zad2");
            host.AddServiceEndpoint(typeof(IZadanie2), new NetTcpBinding(), "net.tcp://127.0.0.1:55765/");
            host.AddServiceEndpoint(typeof(IZadanie7), new NetNamedPipeBinding(), "net.pipe://localhost/ksr-wcf1-zad7");
            
            host.Open();
            Console.WriteLine("Server started. Press [Enter] to exit.");
            Console.ReadLine();
            host.Close();
        }
    }
}
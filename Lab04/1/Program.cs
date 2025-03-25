using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using KSR_WCF1;

namespace KSR_WCF1
{
    [ServiceContract]
    public interface IZadanie1
    {
        [OperationContract]
        string Test(string arg);
        
        [OperationContract]
        [FaultContract(typeof(Wyjatek))]
        void RzucWyjatek(bool czy_rzucic);
        
        [OperationContract]
        string OtoMagia(string magia);
    }

    [DataContract]
    public class Wyjatek
    {
        [DataMember]
        public string opis;
        
        [DataMember]
        public string magia { get; set; }
    }
}

namespace _1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var fact = new ChannelFactory<IZadanie1>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/ksr-wcf1-test"));
            var client = fact.CreateChannel();
            
            // Zadanie 1
            Console.WriteLine(client.Test("Hello, World!"));
            
            // Zadanie 5
            try
            {
                client.RzucWyjatek(true);
            }
            catch (FaultException<Wyjatek> ex)
            {
                Console.WriteLine($"Caught exception: {ex.Detail.opis}");
                Console.WriteLine(client.OtoMagia(ex.Detail.magia));
            }
            
            (client as IDisposable).Dispose();
            fact.Close();
        }
    }
}
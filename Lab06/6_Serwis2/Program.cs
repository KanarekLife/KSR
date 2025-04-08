using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace _6_Serwis2
{
    [ServiceContract]
    interface IUsluga
    {
        [OperationContract]
        int Dodaj(int a, int b);
    }

    class Usluga : IUsluga
    {
        public int Dodaj(int a, int b)
        {
            Console.WriteLine("Zapasowy");
            return a + b;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(Usluga));
            host.AddServiceEndpoint(typeof(IUsluga), new NetNamedPipeBinding(), "net.pipe://localhost/6_Serwis2");
            host.Open();
            Console.WriteLine("6_Serwis2 started. Press [Enter] to exit.");
            Console.ReadLine();
            host.Close();
        }
    }
}

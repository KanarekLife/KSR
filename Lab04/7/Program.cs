using System;
using System.ServiceModel;
using _7.Server;

namespace _7
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var client = new Server.Zadanie7Client();
            try
            {
                client.RzucWyjatek7("Test", 2137);
            }
            catch (FaultException<Wyjatek7> exception)
            {
                Console.WriteLine(exception.Detail.opis);
            }
        }
    }
}
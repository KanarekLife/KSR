using Klient.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Klient
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            await Zadanie1();
            Zadanie2();
        }

        private static async Task Zadanie1()
        {
            var client = new Zadanie1Client();
            var longTask = client.DlugieObliczeniaAsync();
            for (var x = 0; x <= 20; x++)
            {
                // ReSharper disable once MethodHasAsyncOverload
                client.Szybciej(x, 3 * x * x - 2 * x);
            }
            await longTask;
        }

        private static void Zadanie2()
        {
            var client = new Zadanie2Client(new InstanceContext(new Zadanie2Handler()));
            client.PodajZadania();
            Console.WriteLine("Click [ENTER] to stop listening");
            Console.Read();
        }

        private class Zadanie2Handler : IZadanie2Callback
        {
            public void Zadanie(string zadanie1, int pkt, bool zaliczone)
            {
                Console.WriteLine($"Podzadanie {zadanie1}, {pkt}, {zaliczone}");
            }
        }
    }
}

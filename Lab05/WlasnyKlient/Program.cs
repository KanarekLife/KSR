using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WlasnyKlient.ServiceReference1;

namespace WlasnyKlient
{
    class Program
    {
        private class Handler : IZadanie6Callback
        {
            public void Wynik(int wyn)
            {
                Console.WriteLine($"Zadanie 6 (dodawanie): {wyn}");
            }
        }

        static void Main(string[] args)
        {
            var client5 = new Zadanie5Client();
            Console.WriteLine($"Zadanie 5 (scalanie): {client5.ScalNapisy("hello ", "world")}");

            var client6 = new Zadanie6Client(new InstanceContext(new Handler()));
            client6.Dodaj(21, 37);

            Console.WriteLine("Click [ENTER] to stop listening");
            Console.Read();
        }
    }
}

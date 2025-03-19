using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2
{
    class Program
    {
        static void Main()
        {
            var t = Type.GetTypeFromProgID("KSR20.COM3Klasa.1");
            var k = Activator.CreateInstance(t);
            t.InvokeMember("Test", System.Reflection.BindingFlags.InvokeMethod, null, k, new object[] { "Hello from COM" });
        }
    }
}

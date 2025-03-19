using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

/*
 
Automatyczna rejestracja: Properties -> Build -> Register for COM interop (opcjonalnie, bo wymaga admina)
Properties -> Signing -> Sign the assembly
gacutil /i .\Zadanie5.dll (jako administrator)
regasm /codebase .\Zadanie5.dll (jako administrator)

*/

namespace _5
{
    [Guid("F59DA79E-29BB-476C-BFF4-2E9C0ADFDD4D"), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface Klasa2
    {
        uint Test(string s);
    }

    [Guid("F08FB011-E87D-472E-9886-659C2559FB10"), ComVisible(true), ClassInterface(ClassInterfaceType.None), ProgId("KSR20.COM3Klasa.2")]
    public class Klasa2Impl : Klasa2
    {
        public uint Test(string s)
        {
            Console.WriteLine($"Klasa 2: {s}");
            return 0;
        }
    }
}

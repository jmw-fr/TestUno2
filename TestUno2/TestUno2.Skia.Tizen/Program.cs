using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace TestUno2.Skia.Tizen
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new TizenHost(() => new TestUno2.App(), args);
            host.Run();
        }
    }
}

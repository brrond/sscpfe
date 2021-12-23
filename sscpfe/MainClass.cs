using System;

namespace sscpfe
{
    // MainClass for sscpfe
    class MainClass
    {

        [System.STAThreadAttribute]
        static void Main(string[] args)
        {
            SSCPFERunner runner = new SSCPFERunner(args);
            runner.Run();
        }
    }
}

using System;

namespace sscpfe
{
    class SSCPFEHandlerException : Exception
    {
        public SSCPFEHandlerException() : base("Expection thrown by handler in mainloop")
        {

        }
    }
}

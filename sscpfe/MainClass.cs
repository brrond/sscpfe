namespace sscpfe
{

    class MainClass
    {

        [System.STAThreadAttribute]
        static void Main(string[] args)
        {
            SSCPFEApplication app;
            if (args.Length != 0)
            {
                if(args[0].ToLower() == "/help" || args[0].ToLower() == "--help")
                {
                    SSCPFEApplication.ShowHelp();
                    return;
                }
                app = new SSCPFEApplication(args[0]);
            }
            else
                app = new SSCPFEApplication();

            app.Mainloop();
        }
    }
}

namespace sscpfe
{
    // MainClass for sscpfe
    class MainClass
    {

        [System.STAThreadAttribute]
        static void Main(string[] args)
        {
            SSCPFEApplication app; // create app
            if (args.Length != 0)
            {
                if(args[0].ToLower() == "/help" || args[0].ToLower() == "--help") // help command
                {
                    SSCPFEApplication.ShowHelp(); // static method for help
                    return;
                }
                app = new SSCPFEApplication(args[0]); // init app with params (f-name)
            }
            else
                app = new SSCPFEApplication(); // init app

            app.Mainloop(); // start sscpfe
        }
    }
}

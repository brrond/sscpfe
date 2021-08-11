namespace sscpfe
{
    class MainClass
    {
        static void Main(string[] args)
        {
            SSCPFEApplication app;
            if (args.Length != 0)
                app = new SSCPFEApplication(args[0]);
            else
                app = new SSCPFEApplication();

            app.Mainloop();
        }
    }
}

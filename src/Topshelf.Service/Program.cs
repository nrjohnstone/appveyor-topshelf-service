using System;
using System.Timers;
using Serilog;
using Topshelf;

namespace AppVeyor.Topshelf.Service
{
    public class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File("logfile.txt").CreateLogger();

            var rc = HostFactory.Run(x =>                                  
            {
                x.Service<TownCrier>(s =>                                 
                {
                    s.ConstructUsing(name => new TownCrier());                
                    s.WhenStarted(tc => tc.Start());                         
                    s.WhenStopped(tc => tc.Stop());                          
                });
                x.RunAsLocalSystem();                                       

                x.SetDescription("Sample Topshelf Host");                   
                x.SetDisplayName("Stuff");                                 
                x.SetServiceName("Stuff");                                 
            });                                                             

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());  
            Environment.ExitCode = exitCode;
        }
    }

    public class TownCrier
    {
        readonly Timer _timer;
        public TownCrier()
        {
            _timer = new Timer(1000) { AutoReset = true };
            _timer.Elapsed += (sender, eventArgs) =>
            {
                var message = $"It is {DateTime.Now} and all is well";
                Log.Information(message);
                Console.WriteLine(message);
            };
        }
        public void Start() { _timer.Start(); }
        public void Stop() { _timer.Stop(); }
    }

}

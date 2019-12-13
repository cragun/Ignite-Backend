using System.Diagnostics;

namespace DataReef.ServiceRunner.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            SetupTraceListeners();
        }

        /// <summary>
        /// Setup Trace to write to both Console and a file output.
        /// </summary>
        private static void SetupTraceListeners()
        {
            Trace.Listeners.Clear();
            TextWriterTraceListener twtl = new TextWriterTraceListener("Output.txt");
            twtl.Name = "TextLogger";
            twtl.TraceOutputOptions = TraceOptions.ThreadId | TraceOptions.DateTime;

            ConsoleTraceListener ctl = new ConsoleTraceListener(false);
            ctl.TraceOutputOptions = TraceOptions.DateTime;

            Trace.Listeners.Add(twtl);
            Trace.Listeners.Add(ctl);
            Trace.AutoFlush = true;
        }
    }
}

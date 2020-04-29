using System;
using ProcessManager;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Main
{
    internal static class Monitor
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("This is an application monitoring tool. It monitors necessary process  and stops it if it is launched " +
                              "more than provided amount of time (in minutes)");
            Console.WriteLine("Usage: monitor.exe <processName> <processTimeout> <processCheckInterval>");
            Console.WriteLine("Example: monitor.exe notepad 5 1");

            if (args.Length < 3)
            {
                Console.WriteLine("Incorrect number of parameters!");
                Console.WriteLine("Usage: Monitor.exe <processName> <processTimeout> <processCheckInterval>");
                return;
            }

            if (!int.TryParse(args[1], out var timeout) || timeout <= 0)
            {
                Console.WriteLine("Please provide a timeout argument as positive integer");
                return;
            }

            if (!int.TryParse(args[2], out var checkInterval) || checkInterval <= 0)
            {
                Console.WriteLine("Please provide a check interval argument as a positive integer");
                return;
            }

            var proc = new ProcessMonitor(args[0]);

            if (proc.IsProcessRunning() != true)
            {
                Console.WriteLine($"Process '{proc.Name}' is not running. Exiting...");
                return;
            }
            proc.Monitor(timeout, checkInterval);

            ProcessMonitor.FreezeOnScreen();
        }
    }
}

namespace ProcessManager
{
    /// <summary>
    ///
    /// </summary>
    internal class ProcessMonitor
    {
        //Private declarations.
        private readonly string _processName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="program"></param>
        public ProcessMonitor(string program)
        {
            _processName = program;
            Name = _processName;
        }

        public string Name { get; }

        /// <summary>
        /// Determines if the process is running or NOT
        /// </summary>
        public bool IsProcessRunning()
        {
            var proc = Process.GetProcessesByName(_processName);
            
            return proc.Length != 0;
        }

        /// <summary>
        /// Monitors the running program
        /// </summary>
        public void Monitor(int timeout, int checkInterval)
        {
            while (IsProcessRunning())
            {
                int executionTime = GetProcessExecutionTime(_processName);
                if (executionTime >= timeout)
                {
                    Console.WriteLine(
                        $"Killing {_processName} at {DateTime.Now.ToString(CultureInfo.InvariantCulture)} due to " +
                        $"timeout {timeout} minutes.");
                    KillProcess(_processName); // Kills the running process
                    break;
                }
                // Sleep till the next loop
                Console.WriteLine($"Process '{_processName}' is running for {executionTime} minute(s), waiting {checkInterval} more minute(s)");
                Thread.Sleep(checkInterval * 60000 + 60000);
            }
        }

        /// <summary>
        /// Check for process execution time (in minutes)
        /// </summary>
        /// <returns>process execution time</returns>
        private static int GetProcessExecutionTime(string processName)
        {
            var p = Process.GetProcessesByName(processName);
            if (p.Length <= 0) throw new Exception("Process not found!");
            var span = DateTime.Now - p[0].StartTime;
            return (int)span.TotalMinutes;
        }

        /// <summary>
        /// Kills the running process, selects the process from the function input parameter
        /// </summary>
        /// <param name="program">The running process name</param>
        private static void KillProcess(string program)
        {
            var taskKiller = String.Format("taskkill /f /im {0}", program.EndsWith(".exe") ? program : program + ".exe");
            try
            {
                var info = new ProcessStartInfo("cmd.exe", "/c " + taskKiller)
                {
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var process = new Process {StartInfo = info};
                process.Start();
                process.StandardOutput.ReadToEnd();
            }
            catch (Exception exception)
            {
                Console.Write(exception.Message);
            }
        }

        /// <summary>
        /// Waiting for last user interaction before exiting
        /// </summary>
        public static void FreezeOnScreen()
        {
            Console.WriteLine("Process is not running anymore, press any key to exit...");
            Console.Read();
        }
    }
}
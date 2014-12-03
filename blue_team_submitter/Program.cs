/* file: Program.cs
 * author: David Weinman
 * date: 12/2/14
 * description: Main class which
 * creates Submission Daemon
 * service.
 */

using System;
using System.ServiceProcess;

namespace SubmissionDaemon
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (Environment.UserInteractive) {
                var submissionHandle = new SubmissionDaemon();
                submissionHandle.Start();
                Console.WriteLine("... press <ENTER> to quit");
                Console.ReadLine();
                submissionHandle.Stop();
            }
            else {
                System.ServiceProcess.ServiceBase.Run(new SubmissionDaemon());
            }
        }
    }

}

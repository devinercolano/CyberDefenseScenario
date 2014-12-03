/* file: SubmissionDaemon.cs
 * author: David Weinman
 * date: 12/2/14
 * description: Submission Daemon
 * class file. Creates file event
 * notification handlers which send
 * submissions to the scoring server.
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Net.Sockets;

namespace SubmissionDaemon
{
    public class SubmissionDaemon : ServiceBase
    {
        // scoring server information
        const Int32 port = 1212;
        const String scoringServer = "10.0.27.128";

        // maximum submission size in bytes (10K).
        const int MAXSUBMISSIONLENGTH = 10000;

        // challenge submission filename constants
        const String chall1_filename = "ftp_server.txt";
        const String chall2_filename = "forgot_my_pass.txt";
        const String chall3_filename = "compress_stan.txt";
        const String chall4_filename = "stan.txt";
        const String chall5_filename = "pro_tools.txt";
        const String chall6_filename = "plugin.txt";
        const String chall1_foldername = "ftp_service\\";
        const String chall2_foldername = "forgot_my_pass\\";
        const String chall3_foldername = "compress_stan\\";
        const String chall4_foldername = "stan\\";
        const String chall5_foldername = "pro_tools\\";
        const String chall6_foldername = "plugin\\";

        const String submissions_path = "C:\\Users\\blue_team\\Documents\\submissions\\";

        // challenge file watcher objects
        FileSystemWatcher chall1_filewatcher;
        FileSystemWatcher chall2_filewatcher;
        FileSystemWatcher chall3_filewatcher;
        FileSystemWatcher chall4_filewatcher;
        FileSystemWatcher chall5_filewatcher;
        FileSystemWatcher chall6_filewatcher;

        // submission daemon constructor
        public SubmissionDaemon()
        {
            this.ServiceName = "SubmissionDaemon";
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = true;
        }

        // this is the function that logs some
        // message while the submission daemon is
        // running.
        private static void LogEvent(String message)
        {
            string eventSource = "Submission File Monitor Service";
            DateTime dt = new DateTime();
            dt = System.DateTime.UtcNow;
            message = dt.ToLocalTime() + ": " + message;

            EventLog.WriteEntry(eventSource, message);
        }

        // this is the function that gets called when
        // a submission file gets created
        private static void OnCreate(object source, FileSystemEventArgs evntargs)
        {
            String currentContent;
            // log the change
            LogEvent(String.Format("File {0} was: {1}", evntargs.FullPath, evntargs.ChangeType.ToString()));
            // then try to read in the file data and send to submission listener
            try
            {
                // first, read the contents of the submitted file
                //LogEvent(String.Format("About to read all text from {0}.", evntargs.FullPath));
                try
                {
                    currentContent = File.ReadAllText(evntargs.FullPath);
                }
                catch (FileNotFoundException e)
                {
                    LogEvent(String.Format("FileNotFoundException: {0}", e));
                    return;
                }
                // and check that it is within our size limitations
                //LogEvent(String.Format("About to check size of {0}.", evntargs.FullPath));
                if (currentContent.Length > MAXSUBMISSIONLENGTH)
                {
                    LogEvent(String.Format("Submission was {0} bytes too long.\nSubmission ignored.",
                        currentContent.Length - MAXSUBMISSIONLENGTH));
                    return;
                }
                // initialize the tcp client to send the submission to
                TcpClient client = new TcpClient(scoringServer, port);
                // make a byte array our of the submission
                Byte[] submission = System.Text.Encoding.ASCII.GetBytes(currentContent);
                // prepare client stream to send submission to 
                NetworkStream stream = client.GetStream();
                // Log submission event
                LogEvent(String.Format(
                "Submission Content:\n--------------------------\n{0}\n--------------------------\nSending message with file content now.",
                currentContent));
                // send submission via client stream and log event
                stream.Write(submission, 0, submission.Length);
                LogEvent(String.Format("Submission sent.", currentContent));
                // delete file and log deletion event
                File.Delete(evntargs.FullPath);
                LogEvent(String.Format("Deleted {0}.", evntargs.FullPath));
            }
            catch (SocketException e)
            {
                LogEvent(String.Format("SocketException: {0}", e));
            }
        }

        //  this method is called when the challenge file watcher detects an error. 
        private static void OnError(object source, ErrorEventArgs e)
        {
            //  Show that an error has been detected.
            LogEvent("The Challenge File Watcher has detected an error");
            //  Give more information if the error is due to an internal buffer overflow. 
            if (e.GetException().GetType() == typeof(InternalBufferOverflowException))
            {
                //  This can happen if Windows is reporting many file system events quickly  
                //  and internal buffer of the  FileSystemWatcher is not large enough to handle this 
                //  rate of events. The InternalBufferOverflowException error informs the application 
                //  that some of the file system events are being lost.
                LogEvent(("The challenge file watcher experienced an internal buffer overflow: " + e.GetException().Message));
            }
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            // initialize file watcher objects
            chall1_filewatcher = new FileSystemWatcher(submissions_path + chall1_foldername, chall1_filename);
            chall2_filewatcher = new FileSystemWatcher(submissions_path + chall2_foldername, chall2_filename);
            chall3_filewatcher = new FileSystemWatcher(submissions_path + chall3_foldername, chall3_filename);
            chall4_filewatcher = new FileSystemWatcher(submissions_path + chall4_foldername, chall4_filename);
            chall5_filewatcher = new FileSystemWatcher(submissions_path + chall5_foldername, chall5_filename);
            chall6_filewatcher = new FileSystemWatcher(submissions_path + chall6_foldername, chall6_filename);
            chall1_filewatcher.EnableRaisingEvents = true;
            chall2_filewatcher.EnableRaisingEvents = true;
            chall3_filewatcher.EnableRaisingEvents = true;
            chall4_filewatcher.EnableRaisingEvents = true;
            chall5_filewatcher.EnableRaisingEvents = true;
            chall6_filewatcher.EnableRaisingEvents = true;

            // set notification events
            chall1_filewatcher.NotifyFilter = NotifyFilters.FileName;
            chall2_filewatcher.NotifyFilter = NotifyFilters.FileName;
            chall3_filewatcher.NotifyFilter = NotifyFilters.FileName;
            chall4_filewatcher.NotifyFilter = NotifyFilters.FileName;
            chall5_filewatcher.NotifyFilter = NotifyFilters.FileName;
            chall6_filewatcher.NotifyFilter = NotifyFilters.FileName;

            // register event handler that
            // gets called when the file is created
            chall1_filewatcher.Created += new FileSystemEventHandler(OnCreate);
            chall2_filewatcher.Created += new FileSystemEventHandler(OnCreate);
            chall3_filewatcher.Created += new FileSystemEventHandler(OnCreate);
            chall4_filewatcher.Created += new FileSystemEventHandler(OnCreate);
            chall5_filewatcher.Created += new FileSystemEventHandler(OnCreate);
            chall6_filewatcher.Created += new FileSystemEventHandler(OnCreate);

            //  Register a handler that gets called if the  
            //  challenge file watcher needs to report an error.
            chall1_filewatcher.Error += new ErrorEventHandler(OnError);
            chall2_filewatcher.Error += new ErrorEventHandler(OnError);
            chall3_filewatcher.Error += new ErrorEventHandler(OnError);
            chall4_filewatcher.Error += new ErrorEventHandler(OnError);
            chall5_filewatcher.Error += new ErrorEventHandler(OnError);
            chall6_filewatcher.Error += new ErrorEventHandler(OnError);

        }

        protected override void OnStop()
        {
            base.OnStop();
            // do nothing
        }

        public void Start()
        {
            OnStart(new string[0]);
            LogEvent("The FileSystemWatcher has been started on these files: "
    + submissions_path + chall1_foldername + chall1_filename + ",\n"
    + submissions_path + chall2_foldername + chall2_filename + ",\n"
    + submissions_path + chall3_foldername + chall3_filename + ",\n"
    + submissions_path + chall4_foldername + chall4_filename + ",\n"
    + submissions_path + chall5_foldername + chall5_filename + ", and:\n"
    + submissions_path + chall6_foldername + chall6_filename + "."
            );
        }

        public void Stop()
        {
            OnStop();
        }
    }
}

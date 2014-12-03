/* file: SubmissionInstaller.cs
 * author: David Weinman
 * date: 12/2/14
 * description: Registers the
 * submission daemon with the
 * windows services
 */

using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;


namespace SubmissionDaemon
{
    [RunInstaller(true)]
    public class SubmissionDaemonInstaller : Installer
    {
        private ServiceProcessInstaller submissionProcessInstaller;
        private ServiceInstaller submissionDaemonInstaller;

        public SubmissionDaemonInstaller()
        {
            submissionProcessInstaller = new ServiceProcessInstaller();
            submissionDaemonInstaller = new ServiceInstaller();
            // Here you can set properties on submissionProcessInstaller
            //or register event handlers
            submissionProcessInstaller.Account = ServiceAccount.LocalService;

            submissionDaemonInstaller.ServiceName = new SubmissionDaemon().ServiceName;
            this.Installers.AddRange(new Installer[] {
            submissionProcessInstaller, submissionDaemonInstaller });
        }
    }
}

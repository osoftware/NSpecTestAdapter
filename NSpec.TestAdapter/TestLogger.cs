using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NSpec.TestAdapter
{
    public class TestLogger
    {
        public TestLogger(IMessageLogger messageLogger)
        {
            this.messageLogger = messageLogger;

            adapterVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public void SendMessage(TestMessageLevel logLevel, string message)
        {
            if (CanLog)
            {
                messageLogger.SendMessage(logLevel, message);
            }
        }

        public void SendInformationalMessage(string message)
        {
            SendMessage(TestMessageLevel.Informational, message);
        }

        public void SendMainMessage(string message)
        {
            SendInformationalMessage(String.Format("{0} {1}: {2}", adapterName, adapterVersion, message));
        }

        public void SendDebugMessage(string message)
        {
#if DEBUG
            SendMessage(TestMessageLevel.Informational, String.Format("[DBG] {0}", message));
#endif
        }

        public void SendWarningMessage(string message)
        {
            SendMessage(TestMessageLevel.Warning, message);
        }

        public void SendWarningMessage(Exception ex, string message)
        {
            SendWarningMessage(message);

            SendWarningMessage(ex.ToString());
        }

        public void SendErrorMessage(string message)
        {
            SendMessage(TestMessageLevel.Error, message);
        }

        public void SendErrorMessage(Exception ex, string message)
        {
            SendErrorMessage(message);

            SendErrorMessage(ex.ToString());
        }

        bool CanLog { get { return messageLogger != null; } }

        readonly IMessageLogger messageLogger;
        readonly string adapterVersion;

        const string adapterName = "NSpec Test Adapter";
    }
}

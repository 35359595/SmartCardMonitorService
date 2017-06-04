using PCSC;
using System;
using System.Diagnostics;

namespace SCBlocker
{
    class SCManager
    {
        private string[] _readers;
        private SCardMonitor _monitor = new SCardMonitor(ContextFactory.Instance, SCardScope.System);

        public SCManager()
        {
            using (var context = new SCardContext())
            {
                context.Establish(SCardScope.System);
                _readers = context.GetReaders();
            }
        }

        private static void _writeLog(string Message)
        {
            string sSource = "SCManager";
            string sLog = "Security";

            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, sLog);

            EventLog.WriteEntry(sSource, Message, EventLogEntryType.Information, 4800);
        }

        private void _sCardRemoved(object sender, CardStatusEventArgs e)
        {
            _writeLog(String.Format("Locking machine. SmartCard was removed from {0}.", e.ReaderName));
            LockWorkStation();
        }

        private static void LockWorkStation() { //not working from the service :(
            Process.Start("rundll32.exe", "user32.dll, LockWorkStation");
        }

        public bool StartMonitoring()
        {
            try
            {
                _monitor.CardRemoved += new CardRemovedEvent(_sCardRemoved);
                foreach (string reader in _readers)
                    _monitor.Start(reader);
                return true;
            } catch (Exception e)
            {
                _writeLog(e.Message);
                return false;
            }
        }

        public bool StopMonitoring()
        {
            try
            {
                _monitor.Dispose();
                return true;
            } catch (Exception e) {
                _writeLog(e.Message);
                return false;
            }
        }
    }
}

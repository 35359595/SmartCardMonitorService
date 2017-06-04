using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCSC;

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

        private void _sCardRemoved(object sender, CardStatusEventArgs e)
        {
            Console.WriteLine("Locking machine. SmartCard was removed.");
            // LockWorkStation();
        }

        public void StartMonitoring()
        {
            _monitor.CardRemoved += new CardRemovedEvent(_sCardRemoved);
            foreach (string reader in _readers)
                _monitor.Start(reader);
        }

        public void StopMonitoring()
        {
            _monitor = new SCardMonitor(ContextFactory.Instance, SCardScope.System);
        }
    }
}

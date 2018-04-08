using System;
using System.Diagnostics;

namespace WorldEdit
{
    public class Disposable : IDisposable
    {
        private Process _process;

        public Disposable(Process process)
        {
            _process = process;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            var pid = _process?.Id;
            if (pid != null)
            {
                Process.Start("taskkill.exe", "/PID " + pid);
            }

            _process?.CloseMainWindow();
            _process?.Dispose();
            _process = null;
        }
    }
}
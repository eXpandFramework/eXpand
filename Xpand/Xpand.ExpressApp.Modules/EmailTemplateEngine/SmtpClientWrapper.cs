namespace Xpand.EmailTemplateEngine {
    using System;
    using System.Diagnostics;
    using System.Net.Mail;

    public class SmtpClientWrapper : ISmtpClient {
        private readonly SmtpClient _realClient;
        private bool _disposed;

        public SmtpClientWrapper(SmtpClient realClient) {
            Invariant.IsNotNull(realClient, "realClient");

            _realClient = realClient;
        }

        [DebuggerStepThrough]
        ~SmtpClientWrapper() {
            Dispose(false);
        }

        [DebuggerStepThrough]
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Send(MailMessage message) {
            Invariant.IsNotNull(message, "message");

            _realClient.Send(message);
        }

        [DebuggerStepThrough]
        protected virtual void DisposeCore() {
        }

        [DebuggerStepThrough]
        private void Dispose(bool disposing) {
            if (!_disposed && disposing) {
                DisposeCore();
            }

            _disposed = true;
        }
    }
}
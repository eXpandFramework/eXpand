namespace Xpand.EmailTemplateEngine {
    using System;
    using System.Diagnostics;
    using System.Net.Mail;

    public class SmtpClientWrapper : ISmtpClient {
        private readonly SmtpClient realClient;
        private bool disposed;

        public SmtpClientWrapper(SmtpClient realClient) {
            Invariant.IsNotNull(realClient, "realClient");

            this.realClient = realClient;
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

            realClient.Send(message);
        }

        [DebuggerStepThrough]
        protected virtual void DisposeCore() {
        }

        [DebuggerStepThrough]
        private void Dispose(bool disposing) {
            if (!disposed && disposing) {
                DisposeCore();
            }

            disposed = true;
        }
    }
}
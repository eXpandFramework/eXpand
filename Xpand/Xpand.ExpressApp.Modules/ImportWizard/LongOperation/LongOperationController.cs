using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.ImportWizard.LongOperation {
    public abstract class LongOperationController : ViewController {
        private IProgressControl _ProgressControl;
        private AsyncOperation _WaitLongOperationCompleted;
        public List<string> ChangedProps = new List<string>();

        private void DoWork(DevExpress.ExpressApp.Demos.LongOperation longOperation) {
            try {
                DoWorkCore(longOperation);
            } catch (Exception) {
                longOperation.TerminateAsync();
                _ProgressControl.Dispose();
                throw;

            }
        }
        private void WorkCompleted(object state) {
            OnOperationCompleted();
        }
        private void LongOperation_CancellingTimeoutExpired(object sender, EventArgs e) {
            ((DevExpress.ExpressApp.Demos.LongOperation)sender).TerminateAsync();
        }
        protected void LongOperation_Completed(object sender, LongOperationCompletedEventArgs e) {
            _ProgressControl.Dispose();
            _ProgressControl = null;
            ((DevExpress.ExpressApp.Demos.LongOperation)sender).CancellingTimeoutExpired -= LongOperation_CancellingTimeoutExpired;
            ((DevExpress.ExpressApp.Demos.LongOperation)sender).Completed -= LongOperation_Completed;
            ((DevExpress.ExpressApp.Demos.LongOperation)sender).Dispose();

            _WaitLongOperationCompleted.PostOperationCompleted(WorkCompleted, null);
            _WaitLongOperationCompleted = null;
        }

        protected abstract void DoWorkCore(DevExpress.ExpressApp.Demos.LongOperation longOperation);

        protected abstract IProgressControl CreateProgressControl();

        protected virtual void OnOperationStarted() {
            if (OperationStarted != null) {
                OperationStarted(this, EventArgs.Empty);
            }
        }
        protected virtual void OnOperationCompleted() {
            View.ObjectSpace.Refresh();
            if (OperationCompleted != null) {
                OperationCompleted(this, EventArgs.Empty);
            }
        }
        protected void StartLongOperation(List<string> strings) {

            _WaitLongOperationCompleted = AsyncOperationManager.CreateOperation(null);
            var longOperation = new DevExpress.ExpressApp.Demos.LongOperation(DoWork) { CancellingTimeoutMilliSeconds = 2000 };
            longOperation.CancellingTimeoutExpired += LongOperation_CancellingTimeoutExpired;
            longOperation.Completed += LongOperation_Completed;

            ChangedProps = strings;
            _ProgressControl = CreateProgressControl();
            _ProgressControl.ShowProgress(longOperation);
            longOperation.StartAsync();
            OnOperationStarted();
        }
        public event EventHandler OperationStarted;
        public event EventHandler OperationCompleted;

        #region Session struff

        public UnitOfWork CreateUpdatingSession() {
            var session = new UnitOfWork(((XPObjectSpace)ObjectSpace).Session.DataLayer);
            OnUpdatingSessionCreated(session);
            return session;
        }

        public void CommitUpdatingSession(UnitOfWork session) {
            session.CommitChanges();
            OnUpdatingSessionCommitted(session);
        }

        protected virtual void OnUpdatingSessionCommitted(UnitOfWork session) {
            if (UpdatingSessionCommitted != null) {
                UpdatingSessionCommitted(this, new SessionEventArgs(session));
            }
        }

        protected virtual void OnUpdatingSessionCreated(UnitOfWork session) {
            if (UpdatingSessionCreated != null) {
                UpdatingSessionCreated(this, new SessionEventArgs(session));
            }
        }

        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;

        #endregion

    }

    public class LongOperationTerminateException : Exception { }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class BatchCreationOptionsAttribute : Attribute {
        public BatchCreationOptionsAttribute(int objectsCount) {
            ObjectsCount = objectsCount;
        }
        public BatchCreationOptionsAttribute(int objectsCount, int commitInterval)
            : this(objectsCount) {
            CommitInterval = commitInterval;
        }

        public int? ObjectsCount { get; private set; }

        public int? CommitInterval { get; private set; }
    }

    public interface IObjectPropertiesInitializer {
        void InitializeObject(int index);
    }

    public interface IProgressControl : IDisposable {
        void ShowProgress(DevExpress.ExpressApp.Demos.LongOperation longOperation);
    }
}
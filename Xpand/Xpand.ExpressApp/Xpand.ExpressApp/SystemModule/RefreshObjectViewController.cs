using System;
using System.ComponentModel;
using System.Timers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelObjectViewRefresh:IModelObjectView{
        [Category(AttributeCategoryNameProvider.Xpand)]
        TimeSpan AutoRefreshInterval{ get; set; }
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool RefreshObjectSpaceOnCommit { get; set; }
    }

    public class RefreshObjectViewController:ViewController<ObjectView>,IModelExtender {
        private Timer _timer;
        protected override void OnDeactivated() {
            base.OnDeactivated();
            ObjectSpace.Committed-=ObjectSpaceOnCommitted;
            if (_timer!=null){
                _timer.Elapsed -= timer_Elapsed;
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }

        private void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs) {
            View.RefreshDataSource();
        }

        protected override void OnActivated(){
            base.OnActivated();
            var modelObjectViewRefresh = ((IModelObjectViewRefresh) View.Model);
            if (modelObjectViewRefresh.RefreshObjectSpaceOnCommit){
                ObjectSpace.Committed += ObjectSpaceOnCommitted;
            }
            var interval = modelObjectViewRefresh.AutoRefreshInterval;
            if (interval > TimeSpan.Zero){
                _timer = new Timer(interval.TotalMilliseconds){SynchronizingObject = (ISynchronizeInvoke)Application.MainWindow.Template};
                _timer.Elapsed += timer_Elapsed;
                _timer.Start();
            }
        }

        protected virtual void Refresh(){
            try{
                View.RefreshDataSource();
            }
            catch (Exception e){
                Tracing.Tracer.LogError(e);
            }
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e){
            if (ObjectSpace != null && !ObjectSpace.IsModified)
                Refresh();
        }

        public virtual void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelListView,IModelObjectViewRefresh>();
            extenders.Add<IModelDetailView,IModelObjectViewRefresh>();
        }
    }
}

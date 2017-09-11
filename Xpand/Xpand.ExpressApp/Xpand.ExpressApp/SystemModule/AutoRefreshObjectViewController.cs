using System;
using System.ComponentModel;
using System.Timers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.SystemModule {
    
    public interface IModelObjectViewAutoRefresh:IModelObjectView{
        [Category(AttributeCategoryNameProvider.Xpand)]
        TimeSpan AutoRefreshInterval{ get; set; }
    }

    public class AutoRefreshObjectViewController:ViewController<ObjectView>,IModelExtender {
        private Timer _timer;
        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (_timer!=null){
                _timer.Elapsed -= timer_Elapsed;
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }
        protected override void OnActivated(){
            base.OnActivated();
            var interval = ((IModelObjectViewAutoRefresh) View.Model).AutoRefreshInterval;
            if (interval > TimeSpan.Zero){
                // ReSharper disable once SuspiciousTypeConversion.Global
                _timer = new Timer(interval.TotalMilliseconds){SynchronizingObject = (ISynchronizeInvoke)Application.MainWindow.Template};
                _timer.Elapsed += timer_Elapsed;
                _timer.Start();
            }
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e){
            ObjectSpace.Refresh();
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelListView,IModelObjectViewAutoRefresh>();
            extenders.Add<IModelDetailView,IModelObjectViewAutoRefresh>();
        }
    }
}

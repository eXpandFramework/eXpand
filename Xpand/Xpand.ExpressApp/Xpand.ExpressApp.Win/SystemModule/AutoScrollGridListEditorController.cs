using System;
using System.ComponentModel;
using System.Timers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.General.Model.Options;

namespace Xpand.ExpressApp.Win.SystemModule {
    
    public interface IModelListViewAutoScrollGridListEditor{
        [Category(AttributeCategoryNameProvider.Xpand)]
        [ModelBrowsable(typeof(GridListEditorVisibilityCalculator))]
        TimeSpan AutoScrollToTopInterval{ get; set; }
        [Category(AttributeCategoryNameProvider.Xpand)]
        [ModelBrowsable(typeof(GridListEditorVisibilityCalculator))]
        bool AutoScrollToTopWhenNotFocused{ get; set; }
    }
    public class AutoScrollGridListEditorController:ViewController<ListView>,IModelExtender{
        private Timer _topTimer;
        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (_topTimer != null) {
                _topTimer.Elapsed -= timer_Elapsed;
                _topTimer.Stop();
                _topTimer.Dispose();
                _topTimer = null;
            }
        }
        protected override void OnActivated() {
            base.OnActivated();
            var interval = ((IModelListViewAutoScrollGridListEditor)View.Model).AutoScrollToTopInterval;
            if (interval > TimeSpan.Zero) {
                // ReSharper disable once SuspiciousTypeConversion.Global
                _topTimer = new Timer(interval.TotalMilliseconds) { SynchronizingObject = (ISynchronizeInvoke) Application.MainWindow.Template };
                _topTimer.Elapsed += timer_Elapsed;
                _topTimer.Start();
            }
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e){
            var gridListEditor = ((GridListEditor)View.Editor);
            if (!gridListEditor.Grid.IsFocused|| !((IModelListViewAutoScrollGridListEditor)View.Model).AutoScrollToTopWhenNotFocused)
                gridListEditor.GridView.FocusedRowHandle = 0;
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelListView,IModelListViewAutoScrollGridListEditor>();
        }
    }
}

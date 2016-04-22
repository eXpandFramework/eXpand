using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DevExpress.XtraBars;
using DevExpress.XtraRichEdit;
using Xpand.Persistent.Base.General.Win;

namespace Xpand.ExpressApp.Win.PropertyEditors.RichEdit {
    public partial class RichEditContainer : RichEditContainerBase {
        private readonly List<string> _mergedBarNames=new List<string>();

        public RichEditContainer(){
            InitializeComponent();
            foreach (var bar in barManager1.Bars.Cast<Bar>()){
                bar.Visible = false;
            }
        }

        public override void DestroyToolBar(){
            this.Execute(DestroyBars);
        }

        public override void CreateToolBars(){
            this.Execute(CreateBarsCore);
        }

        private void DestroyBars(BarManager manager){
            foreach (var barName in _mergedBarNames) {
                DestroyBar(manager, barName);
            }
            _mergedBarNames.Clear();
        }

        private void DestroyBar(BarManager manager, string barName){
            for (int i = 0; i < manager.Bars.Count; i++){
                var destBar = manager.Bars[i];
                if (destBar.BarName == barName){
                    manager.Bars.RemoveAt(i);
                    break;
                }
            }
        }

        private void CreateBarsCore(BarManager parentBarManager){
            if (ToolBarsAreHidden)
                return;
            var bars = barManager1.Bars.Cast<Bar>().OrderBy(bar => bar.DockRow).ThenBy(bar => bar.DockCol);
            foreach (var editorBar in bars){
                var parentBar = parentBarManager.Bars[editorBar.BarName];
                if (parentBar == null){
                    if (editorBar.ItemLinks.Any(link => link.Visible)){
                        parentBar = CopyBar(editorBar, parentBarManager);
                        parentBar.Merge(editorBar);
                        parentBar.Visible = true;
                        _mergedBarNames.Add(editorBar.BarName);
                    }
                }
            }
            foreach (var bar in bars){
                var parentBar = parentBarManager.Bars[bar.BarName];
                if (parentBar != null){
                    parentBar.DockRow++;
                    parentBar.ApplyDockRowCol();
                }
            }
        }

        private Bar CopyBar(Bar bar, BarManager barManager){
            Bar res = new Bar(barManager);
            var barMappingExpression = Mapper.CreateMap<Bar, Bar>();
            barMappingExpression.IgnoreAllPropertiesWithAnInaccessibleSetter();
            Mapper.Map(bar, res);
            var barOptionsMappingExpression = Mapper.CreateMap<BarOptions, BarOptions>();
            barOptionsMappingExpression.IgnoreAllPropertiesWithAnInaccessibleSetter();
            Mapper.Map(bar.OptionsBar, res.OptionsBar);
            return res;
        }

        public override RichEditControl RichEditControl{
            get { return richEditControl1; }
        }

        public override void HideToolBars(){
            base.HideToolBars();
            foreach (Bar bar in barManager1.Bars){
                bar.Visible = false;
            }    
        }
    }
}

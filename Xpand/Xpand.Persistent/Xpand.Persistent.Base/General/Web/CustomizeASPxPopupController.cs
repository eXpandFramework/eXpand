using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Controls;
using DevExpress.ExpressApp.Web.Templates;
using Xpand.XAF.Modules.ModelMapper;
using Xpand.XAF.Modules.ModelMapper.Services;

namespace Xpand.Persistent.Base.General.Web{
//    public interface IModelViewPopup {
//
//        [Description("Allows you to customize the view popupcontrol")]
//        IModelWebPopupControl PopupControl { get; }
//    }

    public interface IModelWebPopupControl:IModelNode{
        [Category("eXpand")]
        PopupTemplateType? PopupTemplateType { get; set; }
        [Category("eXpand")]
        ShowPopupMode? ShowPopupMode { get; set; }
    }

    public class CustomizeASPxPopupController : ViewController {
        public static string PopupControlMapName = "PopupControl";

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            var popupWindowControl = ((BaseXafPage)WebWindow.CurrentRequestPage).XafPopupWindowControl;
            popupWindowControl.CustomizePopupWindowSize += XafPopupWindowControl_CustomizePopupWindowSize;
            popupWindowControl.CustomizePopupControl += PopupWindowControlOnCustomizePopupControl;
            Frame.Disposing+=FrameOnDisposing;        }

        private void FrameOnDisposing(object sender, EventArgs eventArgs){
            ((Frame) sender).Disposing-=FrameOnDisposing;
        }

        private void PopupWindowControlOnCustomizePopupControl(object sender, CustomizePopupControlEventArgs e){
            var popupWindowControl = ((XafPopupWindowControl)sender);
            popupWindowControl.CustomizePopupControl -= PopupWindowControlOnCustomizePopupControl;
            ((IModelModelMap) View?.Model.GetNode(PopupControlMapName))?.BindTo(e.PopupControl);
//                new ObjectModelSynchronizer(e.PopupControl, ((IModelViewPopup) View.Model).PopupControl).ApplyModel();
        }

        private void XafPopupWindowControl_CustomizePopupWindowSize(object sender, CustomizePopupWindowSizeEventArgs e) {
            var popupWindowControl = ((XafPopupWindowControl) sender);
            popupWindowControl.CustomizePopupWindowSize -= XafPopupWindowControl_CustomizePopupWindowSize;
            if (View != null){
                var popupControl = ((IModelWebPopupControl) View.Model.GetNode(PopupControlMapName));
                var height = popupControl.GetValue<Unit>("Height");
                var width = popupControl.GetValue<Unit>("Width");
                if (!height.IsEmpty && !width.IsEmpty){
                    e.Size = new Size((int) height.Value, (int) width.Value);
                    e.Handled = true;
                }
                if (popupControl.ShowPopupMode.HasValue)
                    e.ShowPopupMode=popupControl.ShowPopupMode.Value;
                if (popupControl.PopupTemplateType.HasValue)
                    e.PopupTemplateType=popupControl.PopupTemplateType.Value;
                if (!e.Handled && popupControl.ShowPopupMode.HasValue || popupControl.ShowPopupMode.HasValue) {
                    e.Handled = true;
                }
            }
        }

//        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
//            extenders.Add<IModelView,IModelViewPopup>();
//            var builder = new InterfaceBuilder(extenders);
//            var assembly = builder.Build(BuilderDatas(), GetPath(typeof(ASPxPopupControl).Name));
//            builder.ExtendInteface<IModelWebPopupControl, ASPxPopupControl>(assembly);
//        }

//        private static IEnumerable<InterfaceBuilderData> BuilderDatas(){
//            yield return new InterfaceBuilderData(typeof(ASPxPopupControl)) {
//                Act = info => (info.DXFilter(new[] { typeof(PropertiesBase) }, typeof(object)))
//            };
//
//        }
    }
}
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Controls;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Web;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.Persistent.Base.General.Controllers{
    public interface IModelViewPopup {

        [Description("Allows you to customize the view popupcontrol")]
        IModelWebPopupControl PopupControl { get; }
    }

    public interface IModelWebPopupControl:IModelNode{
    }


    public class CustomizeASPxPopupController : ModelAdapterController,IModelExtender {
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var popupWindowControl = ((BaseXafPage)WebWindow.CurrentRequestPage).XafPopupWindowControl;
            popupWindowControl.CustomizePopupWindowSize +=XafPopupWindowControl_CustomizePopupWindowSize;
            popupWindowControl.CustomizePopupControl+=PopupWindowControlOnCustomizePopupControl;
        }

        private void PopupWindowControlOnCustomizePopupControl(object sender, CustomizePopupControlEventArgs e){
            new ObjectModelSynchronizer(e.PopupControl, ((IModelViewPopup) View.Model).PopupControl).ApplyModel();
        }

        private void XafPopupWindowControl_CustomizePopupWindowSize(object sender, CustomizePopupWindowSizeEventArgs e) {
            var popupControl = ((IModelViewPopup)View.Model).PopupControl;
            var height = popupControl.GetValue<Unit>("Height");
            var width = popupControl.GetValue<Unit>("Width");
            if (!height.IsEmpty && !width.IsEmpty){
                e.Size = new Size((int) height.Value, (int) width.Value);
                e.Handled = true;
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelView,IModelViewPopup>();
            var builder = new InterfaceBuilder(extenders);
            var assembly = builder.Build(BuilderDatas(), GetPath(typeof(ASPxPopupControl).Name));
            builder.ExtendInteface<IModelWebPopupControl, ASPxPopupControl>(assembly);
        }

        private static IEnumerable<InterfaceBuilderData> BuilderDatas(){
            yield return new InterfaceBuilderData(typeof(ASPxPopupControl)) {
                Act = info => (info.DXFilter(new[] { typeof(PropertiesBase) }, typeof(object)))
            };

        }
    }
}
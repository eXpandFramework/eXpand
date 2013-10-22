using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Updating;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.MapView
{
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class MapViewModule : XpandModuleBase, IModelXmlConverter
    {

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelClass, IModelClassMapView>();
            extenders.Add<IModelListView, IModelListViewMapView>();
        }


        void IModelXmlConverter.ConvertXml(ConvertXmlParameters parameters)
        {
            if (typeof(IModelMapView).IsAssignableFrom(parameters.NodeType))
            {
                const string oldPropertyName = "InfoWindowText";
                if (parameters.ContainsKey(oldPropertyName))
                {
                    parameters.Values["InfoWindowTextMember"] = parameters.Values[oldPropertyName];
                    parameters.Values.Remove(oldPropertyName);
                }
            }
        }
    }
}


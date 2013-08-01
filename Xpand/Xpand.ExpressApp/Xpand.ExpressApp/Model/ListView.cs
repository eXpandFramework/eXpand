using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Model {
    public interface IModelListViewLinq : IModelListView {
        [Category("eXpand")]
        string XPQueryMethod { get; set; }
    }
    [ModelAbstractClass]
    public interface IModelStaticTextEx : IModelStaticText {
        [Category("eXpand")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        new string Text { get; set; }
    }
    [DomainLogic(typeof(IModelColumnDetailViews))]
    public class IModelColumnDetailViewsDomainLogic {
        public static IModelList<IModelDetailView> Get_DetailViews(IModelColumnDetailViews detailViews) {
            var modelDetailViews =((ModelNode) detailViews).Application.Views.OfType<IModelDetailView>()
                                         .Where(view =>detailViews.ModelMember.MemberInfo.MemberTypeInfo ==view.ModelClass.TypeInfo);
            return new CalculatedModelNodeList<IModelDetailView>(modelDetailViews);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using System.Linq;

namespace Xpand.ExpressApp.Web.FriendlyUrl {
    public interface IFriendlyUrl {
        [Category("eXpand.FriendlyUrl")]
        [ModelValueCalculator("Id")]
        [Required]
        [Description("The same url for listview/detailview is permitted. However there will be conflicts if a 2nd listview with the same url exists")]
        string FriendlyUrl { get; set; }
    }

    public interface IModelOptionsFriendlyUrl {
        [Category("eXpand")]
        bool EnableFriendlyUrl { get; set; }
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class FriendlyUrlAttribute : Attribute, IFriendlyUrl {
        public FriendlyUrlAttribute(string friendlyUrl) {
            FriendlyUrl = friendlyUrl;
        }

        public string FriendlyUrl { get; set; }
    }
    [ModelAbstractClass]
    public interface IModelViewFriendlyUrl : IModelView, IFriendlyUrl {
    }

    public interface IModelFriendlyUrl : IModelNode {
        [ModelValueCalculator(typeof(FriendlyUrlMemberValueCalculator))]
        [Required]
        [Category("eXpand.FriendlyUrl")]
        [DataSourceProperty("AllMembers")]
        string ValueMemberName { get; set; }
        [DefaultValue(null)]
        [Browsable(false)]
        string EditMode { get; set; }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        IEnumerable<string> AllMembers { get; }
    }
    [ModelAbstractClass]
    public interface IModelDetailViewFriendlyUrl : IModelDetailView, IModelViewFriendlyUrl {
        IModelFriendlyUrl Url { get; }
    }
    [DomainLogic(typeof(IModelFriendlyUrl))]
    public class ModelDetailViewFriendlyUrlDomainLogic {
        public static IEnumerable<string> Get_AllMembers(IModelFriendlyUrl friendlyUrl) {
            return ((IModelObjectView)friendlyUrl.Parent).ModelClass.AllMembers.Select(member => member.Name);
        }
    }
    public class FriendlyUrlMemberValueCalculator : IModelValueCalculator {
        #region Implementation of IModelValueCalculator
        public object Calculate(ModelNode node, string propertyName) {
            var modelClass = ((IModelObjectView)node.Parent).ModelClass;
            var friendlyKeyProperty = modelClass.FriendlyKeyProperty;
            return friendlyKeyProperty != null
                       ? modelClass.FindMember(friendlyKeyProperty).Name
                       : (modelClass.KeyProperty != null ? modelClass.FindMember(modelClass.KeyProperty).Name : null);
        }
        #endregion
    }

    public class FriendlyUrlModelExtenderController : Controller, IModelExtender {
        #region Implementation of IModelExtender
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelView, IModelViewFriendlyUrl>();
            extenders.Add<IModelDetailView, IModelDetailViewFriendlyUrl>();
            extenders.Add<IModelOptions, IModelOptionsFriendlyUrl>();
        }
        #endregion
    }

}

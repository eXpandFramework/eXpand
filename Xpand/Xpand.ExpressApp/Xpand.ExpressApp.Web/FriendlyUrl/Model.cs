using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Web.FriendlyUrl {
    public interface IFriendlyUrl {
        [Category("eXpand.FriendlyKey")]
        string FriendlyUrl { get; set; }
    }

    public interface IKeylessDetailKey {
        [Category("eXpand.FriendlyKey")]
        string KeylessDetailKey { get; set; }
    }

    public interface IKeylessListKey {
        [Category("eXpand.FriendlyKey")]
        string KeylessListKey { get; set; }
    }

    public interface IModelOptionsFriendlyUrl {
        [Category("eXpand")]
        bool EnableFriendlyUrl { get; set; }
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class FriendlyUrlAttribute : Attribute, IFriendlyUrl, IKeylessDetailKey, IKeylessListKey {
        public const string DefaultKeylessDetailKey = "detail";
        public const string DefaultKeylessListKey = "list";
        public const string DefaultKeylessDashboardKey = "dashboard";

        public FriendlyUrlAttribute(string friendlyUrl) {
            FriendlyUrl = friendlyUrl;
        }
        #region Implementation of IFriendlyUrl
        public string FriendlyUrl { get; set; }
        #endregion
        #region Implementation of IKeylessDetailKey
        public string KeylessDetailKey { get; set; }
        #endregion
        #region Implementation of IKeylessListKey
        public string KeylessListKey { get; set; }
        #endregion
    }

    public interface IModelClassFriendlyUrl : IModelClass, IFriendlyUrl, IKeylessDetailKey, IKeylessListKey {

    }

    public interface IModelViewFriendlyUrl : IModelView, IFriendlyUrl {
        [Category("eXpand.FriendlyKey")]
        string KeylessKey { get; set; }
    }

    public interface IModelDetailViewFriendlyUrl : IModelDetailView, IModelViewFriendlyUrl {

    }

    public interface IModelListViewFriendlyUrl : IModelListView, IModelViewFriendlyUrl {

    }

    public interface IModelDashboardViewFriendlyUrl : IModelDashboardView, IModelViewFriendlyUrl {

    }
    public interface IModelBOModelFriendlyUrls : IModelBOModel {
        IModelClassFriendlyUrl GetClassByShortUrl();
        [DefaultValue(true)]
        [Category("eXpand.FriendlyKey")]
        bool RemoveIFromInterfaceFriendlyUrl { get; set; }
    }
    [DomainLogic(typeof(IModelListViewFriendlyUrl))]
    public class ModelListViewFriendlyUrlLogic {
        public static string Get_FriendlyUrl(IModelListViewFriendlyUrl instance) {
            if (Equals(instance, instance.ModelClass.DefaultListView)) {
                var ModelClassFrienlyUrl = instance.ModelClass as IModelClassFriendlyUrl;
                if (ModelClassFrienlyUrl != null) {
                    return ModelClassFrienlyUrl.FriendlyUrl;
                }
            }
            return instance.Id;
        }

        public static string Get_KeylessKey(IModelListViewFriendlyUrl instance) {
            if (Equals(instance, instance.ModelClass.DefaultListView)) {
                var ModelClassFrienlyUrl = instance.ModelClass as IModelClassFriendlyUrl;
                if (ModelClassFrienlyUrl != null) {
                    return ModelClassFrienlyUrl.KeylessListKey;
                }
            }
            return FriendlyUrlAttribute.DefaultKeylessListKey;
        }
    }

    [DomainLogic(typeof(IModelBOModelFriendlyUrls))]
    public class ModelBOModelFriendlyUrlsLogic {
        public static IModelClassFriendlyUrl GetClassByShortUrl(IModelBOModelFriendlyUrls modelBoModelFriendlyUrls, string friendlyUrl) {
            return modelBoModelFriendlyUrls.OfType<IModelClassFriendlyUrl>().SingleOrDefault(@class => @class.FriendlyUrl == friendlyUrl);
        }
    }
    [DomainLogic(typeof(IModelClassFriendlyUrl))]
    public class ModelClassFriendlyUrlLogic {
        public static string Get_FriendlyUrl(IModelClassFriendlyUrl instance) {
            string friendlyUrl = "";
            ITypeInfo typeInfo = instance.TypeInfo;
            if (typeInfo != null) {
                var friendlyUrlAttribute = typeInfo.FindAttribute<FriendlyUrlAttribute>();
                if (friendlyUrlAttribute != null) {
                    friendlyUrl = friendlyUrlAttribute.FriendlyUrl;
                }
                if (string.IsNullOrEmpty(friendlyUrl)) {
                    var modelBoModelFriendlyUrls = instance.Parent as IModelBOModelFriendlyUrls;
                    if (typeInfo.IsInterface && modelBoModelFriendlyUrls != null && (((IModelBOModelFriendlyUrls)instance.Parent)).RemoveIFromInterfaceFriendlyUrl && typeInfo.Name.StartsWith("I")) {
                        friendlyUrl = typeInfo.Name.Substring(1);
                    } else {
                        friendlyUrl = typeInfo.Name;
                    }
                }
            }
            return friendlyUrl;
        }
        public static string Get_KeylessDetailKey(IModelClassFriendlyUrl instance) {
            if (instance.TypeInfo != null) {
                var friendlyUrlAttribute = instance.TypeInfo.FindAttribute<FriendlyUrlAttribute>();
                return friendlyUrlAttribute != null ? friendlyUrlAttribute.KeylessDetailKey : FriendlyUrlAttribute.DefaultKeylessDetailKey;
            }
            return FriendlyUrlAttribute.DefaultKeylessDetailKey;
        }
        public static string Get_KeylessListKey(IModelClassFriendlyUrl instance) {
            ITypeInfo typeInfo = instance.TypeInfo;
            if (typeInfo != null) {
                var friendlyUrlAttribute = typeInfo.FindAttribute<FriendlyUrlAttribute>();
                return friendlyUrlAttribute != null
                           ? friendlyUrlAttribute.KeylessListKey
                           : FriendlyUrlAttribute.DefaultKeylessListKey;
            }
            return FriendlyUrlAttribute.DefaultKeylessListKey;
        }
    }
    [DomainLogic(typeof(IModelDetailViewFriendlyUrl))]
    public class ModelDetailViewFriendlyUrlLogic {
        public static string Get_FriendlyUrl(IModelDetailViewFriendlyUrl instance) {
            if (Equals(instance, instance.ModelClass.DefaultDetailView)) {
                var ModelClassFrienlyUrl = instance.ModelClass as IModelClassFriendlyUrl;
                return ModelClassFrienlyUrl != null ? ModelClassFrienlyUrl.FriendlyUrl : instance.Id;
            }
            return instance.Id;
        }

        public static string Get_KeylessKey(IModelDetailViewFriendlyUrl instance) {
            if (Equals(instance, instance.ModelClass.DefaultDetailView)) {
                var ModelClassFrienlyUrl = instance.ModelClass as IModelClassFriendlyUrl;
                return ModelClassFrienlyUrl != null
                           ? ModelClassFrienlyUrl.KeylessDetailKey
                           : FriendlyUrlAttribute.DefaultKeylessDetailKey;
            }
            return FriendlyUrlAttribute.DefaultKeylessDetailKey;
        }
    }

    [DomainLogic(typeof(IModelDashboardViewFriendlyUrl))]
    public class ModelDashboardViewFriendlyUrlLogic {
        public static string Get_FriendlyUrl(IModelDashboardViewFriendlyUrl instance) {
            return instance.Id;
        }

        public static string Get_KeylessKey(IModelDashboardViewFriendlyUrl instance) {
            return FriendlyUrlAttribute.DefaultKeylessDashboardKey;
        }
    }
    public class FriendlyUrlModelExtenderController : Controller, IModelExtender {
        #region Implementation of IModelExtender
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelBOModel, IModelBOModelFriendlyUrls>();
            extenders.Add<IModelClass, IModelClassFriendlyUrl>();
            extenders.Add<IModelView, IModelViewFriendlyUrl>();
            extenders.Add<IModelDetailView, IModelDetailViewFriendlyUrl>();
            extenders.Add<IModelListView, IModelListViewFriendlyUrl>();
            extenders.Add<IModelDashboardView, IModelDashboardViewFriendlyUrl>();
            extenders.Add<IModelOptions, IModelOptionsFriendlyUrl>();
        }
        #endregion
    }

}

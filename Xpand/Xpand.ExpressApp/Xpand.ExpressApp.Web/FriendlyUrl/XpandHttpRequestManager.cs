using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;

namespace Xpand.ExpressApp.Web.FriendlyUrl {
    static class FriendlyUrlModelExtensions {
        public static IEnumerable<string> GetKeylessKeysFromFriendlyUrl(this IModelViews modelViews, string friendlyUrl, params string[] ignoredViewIds) {
            return modelViews.GetViewsFromFriendlyUrl(friendlyUrl).Select(view => view.GetKeylessKey());
        }

        public static IEnumerable<IModelView> GetViewsFromFriendlyUrl(this IModelViews modelViews, string friendlyUrl, params string[] ignoredViewIds) {
            return ignoredViewIds == null ? Enumerable.Empty<IModelView>() : modelViews.Where(view => ignoredViewIds.Contains(view.Id) && view.GetFriendlyUrl() == friendlyUrl);
        }

        public static string GetKeylessKey(this IModelView ModelView) {
            return ((ModelView is IModelViewFriendlyUrl) ? ((IModelViewFriendlyUrl)ModelView).KeylessKey : GetDefaultKeylessKey(GetViewType(ModelView)));
        }

        public static bool IsFriendlyUrl(this IModelViews ModelViews, string FriendlyUrl) {
            return (GetViewsFromFriendlyUrl(ModelViews, FriendlyUrl).Any());
        }

        public static string GetDefaultKeylessKey(this ViewType ViewType) {
            switch (ViewType) {
                case ViewType.DetailView:
                    return FriendlyUrlAttribute.DefaultKeylessDetailKey;
                case ViewType.ListView:
                    return FriendlyUrlAttribute.DefaultKeylessListKey;
                case ViewType.DashboardView:
                    return FriendlyUrlAttribute.DefaultKeylessDashboardKey;
                default:
                    return null;
            }
        }


        public static bool NeedForceDetailViewMode(this IModelViews ModelViews, string FriendlyUrl, string objectKey, params string[] IgnoredViewIds) {
            return GetKeylessKeysFromFriendlyUrl(ModelViews, FriendlyUrl, IgnoredViewIds).Contains(objectKey);
        }

        public static ViewType GetViewType(this IModelView ModelView) {
            if (ModelView is IModelDetailView) {
                return ViewType.DetailView;
            }
            if (ModelView is IModelListView) {
                return ViewType.ListView;
            }
            if (ModelView is IModelDashboardView) {
                return ViewType.DashboardView;
            }
            return ViewType.Any;
        }

        public static string GetFriendlyUrl(this IModelView modelView) {
            return ((IModelViewFriendlyUrl)modelView).FriendlyUrl;
        }

    }

    class FriendlyUrlHelper {
        public static string GetViewIdFromFriendlyUrl(IModelViews modelViews, string friendlyUrl, ref string objectKey, string viewMode) {
            string s = string.IsNullOrEmpty(viewMode) ? objectKey : viewMode;
            var views = modelViews.Where(view => view.GetFriendlyUrl() == friendlyUrl && view.GetKeylessKey() == s);
            bool UseObjectKey = false;
            if (views.Any()) {
                views = modelViews.Where(view => view.GetFriendlyUrl() == friendlyUrl && view.GetViewType() == ViewType.DetailView);
                if (!views.Any()) {
                    return null;
                }
                UseObjectKey = true;
            }
            if (!UseObjectKey) {
                objectKey = null;
            }
            return views.First().Id;
        }

        public static string GetFriendlyUrlFromViewId(IModelViews modelViews, string ViewId, ref string ObjectKey, ref string ViewMode) {
            IModelView modelView = modelViews[ViewId];
            if (modelView != null) {
                string FriendlyUrl = modelView.GetFriendlyUrl();
                if (string.IsNullOrEmpty(ObjectKey)) {
                    ObjectKey = modelView.GetKeylessKey();
                    ViewMode = null;
                } else if (modelView.GetViewType() != ViewType.DetailView || modelViews.NeedForceDetailViewMode(FriendlyUrl, ObjectKey, ViewId)) {
                    ViewMode = modelView.GetKeylessKey();
                }
                return FriendlyUrl;
            }
            return null;
        }
    }
    public sealed class XpandHttpRequestManager : DefaultHttpRequestManager {
        public const string NewObjectParamName = "NewObject";
        public const string ScrollParamName = "Scroll";
        List<string> ShortcutFriendlyUrls;
        public const string ViewParamName = "View";

        public XpandHttpRequestManager() {
            InitShortcutFriendlyUrls();
        }

        string GetFriendlyObjectKey(ITypeInfo typeInfo, string objectKey) {
            if (typeInfo == null) {
                return objectKey;
            }
            string friendlyKeyPropertyName = WebApplication.Instance.Model.BOModel.GetClass(typeInfo.Type).FriendlyKeyProperty;
            if (string.IsNullOrEmpty(friendlyKeyPropertyName)) {
                return objectKey;
            }
            IMemberInfo friendlyKeyPropertyMember = typeInfo.FindMember(friendlyKeyPropertyName);
            if ((friendlyKeyPropertyMember == null) || friendlyKeyPropertyMember.IsKey) {
                return objectKey;
            }
            IObjectSpace objectSpace = WebApplication.Instance.CreateObjectSpace();
            object objectKeyValue = RuntimeHelpers.GetObjectValue(objectSpace.GetObjectKey(typeInfo.Type, objectKey));
            object objectValue = RuntimeHelpers.GetObjectValue(objectSpace.GetObjectByKey(typeInfo.Type, RuntimeHelpers.GetObjectValue(objectKeyValue)));
            if (objectValue == null) {
                return objectKey;
            }
            object friendlyKeyValue = RuntimeHelpers.GetObjectValue(friendlyKeyPropertyMember.GetValue(RuntimeHelpers.GetObjectValue(objectValue)));
            return friendlyKeyValue == null ? null : friendlyKeyValue.ToString();
        }

        public override ViewShortcut GetViewShortcutFromQueryString(NameValueCollection queryString) {
            var queryStringCopy = new NameValueCollection(queryString);
            var result = new ViewShortcut();
            if (!string.IsNullOrEmpty(queryStringCopy[NewObjectParamName])) {
                result[ViewShortcut.IsNewObject] = "True";
                result[ViewShortcut.TemporaryObjectKeyParamName] = queryStringCopy[NewObjectParamName];
                queryStringCopy.Remove(NewObjectParamName);
            }
            if (!string.IsNullOrEmpty(queryStringCopy[ScrollParamName])) {
                result[ViewShortcut.ScrollPositionParamName] = queryStringCopy[ScrollParamName];
                queryStringCopy.Remove(ScrollParamName);
            }
            string ViewMode = null;
            if (!string.IsNullOrEmpty(queryStringCopy[ViewParamName])) {
                ViewMode = queryStringCopy[ViewParamName];
                queryStringCopy.Remove(ViewParamName);
            }
            foreach (string key in queryStringCopy.Keys) {
                if (key == null) {
                    continue;
                }
                if (key.StartsWith(ShortcutUrlParamPrefix)) {
                    string shortcutKey = key.Substring(ShortcutUrlParamPrefix.Length);
                    result[shortcutKey] = queryStringCopy[key];
                } else if (result.ViewId == null) {
                    string objectKey = queryStringCopy[key];
                    string ViewId = FriendlyUrlHelper.GetViewIdFromFriendlyUrl(WebApplication.Instance.Model.Views, key,
                                                                               ref objectKey, ViewMode);
                    if (ViewId != null) {
                        result.ViewId = ViewId;
                        if (objectKey != null) {
                            result.ObjectKey = objectKey;
                        }
                    }
                }
            }

            return result;
        }

        void InitShortcutFriendlyUrls() {
            ShortcutFriendlyUrls = new List<string>(new[] { NewObjectParamName, ScrollParamName, ViewParamName });
        }

        public override void WriteShortcutTo(ViewShortcut currentShortcut, NameValueCollection queryString) {
            var shortcutCopy = new ViewShortcut();
            currentShortcut.CopyTo(shortcutCopy);
            for (int i = queryString.Count - 1; i >= 0; i += -1) {
                string key = queryString.GetKey(i);
                if (key != null && key.StartsWith(ShortcutUrlParamPrefix) || ShortcutFriendlyUrls.Contains(key) ||
                    WebApplication.Instance.Model.Views.IsFriendlyUrl(key)) {
                    if (key != null) queryString.Remove(key);
                }
            }
            ITypeInfo TypeInfo = null;
            if (!string.IsNullOrEmpty(shortcutCopy.ObjectClassName)) {
                TypeInfo = XafTypesInfo.Instance.FindTypeInfo(shortcutCopy.ObjectClassName);
                shortcutCopy.Remove(ViewShortcut.ObjectClassNameParamName);
            }
            string objectKey = null;
            if (!string.IsNullOrEmpty(shortcutCopy.ObjectKey)) {
                objectKey = GetFriendlyObjectKey(TypeInfo, shortcutCopy.ObjectKey);
                shortcutCopy.Remove(ViewShortcut.ObjectKeyParamName);
            }
            string friendlyUrl = null;
            string viewMode = null;
            if (!string.IsNullOrEmpty(shortcutCopy.ViewId)) {
                friendlyUrl = FriendlyUrlHelper.GetFriendlyUrlFromViewId(WebApplication.Instance.Model.Views, shortcutCopy.ViewId, ref objectKey, ref viewMode);
                shortcutCopy.Remove(ViewShortcut.ViewIdParamName);
            }
            if (friendlyUrl != null) {
                queryString[friendlyUrl] = objectKey;
            }
            if (viewMode != null) {
                queryString[ViewParamName] = viewMode;
            }
            string isNewObject = null;
            if (!string.IsNullOrEmpty(shortcutCopy[ViewShortcut.IsNewObject])) {
                isNewObject = shortcutCopy[ViewShortcut.IsNewObject];
                if (System.String.Compare(isNewObject, "True", System.StringComparison.OrdinalIgnoreCase) != 0) {
                    isNewObject = null;
                } else if (!string.IsNullOrEmpty(shortcutCopy[ViewShortcut.TemporaryObjectKeyParamName])) {
                    objectKey = shortcutCopy[ViewShortcut.TemporaryObjectKeyParamName];
                    shortcutCopy.Remove(ViewShortcut.TemporaryObjectKeyParamName);
                }
                shortcutCopy.Remove(ViewShortcut.IsNewObject);
            }
            if (isNewObject != null) {
                queryString[NewObjectParamName] = objectKey;
            }
            if (!string.IsNullOrEmpty(shortcutCopy[ViewShortcut.ScrollPositionParamName])) {
                queryString[ScrollParamName] = shortcutCopy[ViewShortcut.ScrollPositionParamName];
                shortcutCopy.Remove(ViewShortcut.ScrollPositionParamName);
            }
            for (int i = 0; i <= shortcutCopy.Count - 1; i++) {
                string key = ShortcutUrlParamPrefix + shortcutCopy.GetKey(i);
                if (!string.IsNullOrEmpty(shortcutCopy[key])) {
                    queryString[key] = shortcutCopy[key];
                } else if (queryString[key] != null) {
                    queryString.Remove(key);
                }
            }
        }
    }



}

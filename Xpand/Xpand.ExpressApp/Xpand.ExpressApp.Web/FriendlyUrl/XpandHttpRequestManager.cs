using System.Collections.Specialized;
using System.Globalization;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using System.Linq;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.Web.FriendlyUrl {

    public sealed class XpandHttpRequestManager : DefaultHttpRequestManager, IHttpRequestManager {
        #region Implementation of IHttpRequestManager
        ViewShortcut IHttpRequestManager.GetViewShortcut(string shortcutString) {
            if (shortcutString.Contains("=")) {
                var strings = shortcutString.Split('=');
                var viewId = GetViewId(strings);
                var friendlyObjectKey = GetFriendlyObjectKey(strings, viewId);
                return GetViewShortcut(strings, viewId, friendlyObjectKey);
            }
            return GetViewShortcut(shortcutString);
        }

        string GetFriendlyObjectKey(string[] strings, string viewId) {
            var modelView = WebApplication.Instance.Model.Views[viewId] as IModelDetailViewFriendlyUrl;
            return modelView != null ? GetObjectKey(modelView, strings) : null;
        }

        ViewShortcut GetViewShortcut(string[] strings, string viewId, string friendlyObjectKey) {
            var viewShortcut = WebApplication.Instance.Model.Views[viewId] is IModelListView
                                            ? new ViewShortcut(WebApplication.Instance.Model.Views[viewId].AsObjectView.ModelClass.TypeInfo.Type, null, viewId)
                                            : new ViewShortcut(viewId, friendlyObjectKey);
            if (strings[0].Contains("-")) {
                viewShortcut.Keys.Add("mode");
                viewShortcut.Values.Add("Edit");
            }
            return viewShortcut;
        }

        string GetObjectKey(IModelDetailViewFriendlyUrl modelView, string[] strings) {
            var objectSpace = WebApplication.Instance.CreateObjectSpace();
            var modelMember = modelView.ModelClass.FindMember(modelView.Url.ValueMemberName);
            var findObject = objectSpace.FindObject(modelView.ModelClass.TypeInfo.Type, CriteriaOperator.Parse(modelMember.Name + "=?", strings[1]));
            return modelView.ModelClass.TypeInfo.KeyMember.GetValue(findObject).ToString();
        }

        string GetViewId(string[] strings) {
            var viewId = strings[0];
            var split = viewId.Split('-');
            var id = split.Length > 1 ? split[1] : viewId;
            return GetViewIdFromFrindlyUrl(strings, id);
        }

        string GetViewIdFromFrindlyUrl(string[] strings, string id) {
            var modelViewFriendlyUrls = WebApplication.Instance.Model.Views.OfType<IModelViewFriendlyUrl>().Where(url => url.FriendlyUrl == id);
            return strings.Length == 2 ? modelViewFriendlyUrls.OfType<IModelDetailView>().Single().Id : id;
        }
        #endregion

        public override void WriteShortcutTo(ViewShortcut currentShortcut, NameValueCollection queryString) {
            base.WriteShortcutTo(currentShortcut, queryString);
            if (!IsNewObjectView(currentShortcut)) {
                queryString.Clear();
                var modelView = (IModelViewFriendlyUrl)WebApplication.Instance.Model.Views[currentShortcut.ViewId];
                var objectKey = ObjectKey(currentShortcut, modelView);
                var friendlyUrl = EditModeFriendlyUrl(currentShortcut, modelView.FriendlyUrl, modelView as IModelDetailViewFriendlyUrl);
                queryString.Add(friendlyUrl, objectKey);
            }
        }

        bool IsNewObjectView(ViewShortcut currentShortcut) {
            string shortcutNewObject;
            return currentShortcut.TryGetValue("NewObject", out shortcutNewObject) && shortcutNewObject == true.ToString(CultureInfo.InvariantCulture).ToLower();
        }

        string ObjectKey(ViewShortcut currentShortcut, IModelViewFriendlyUrl modelView) {
            if (!string.IsNullOrEmpty(currentShortcut.ObjectKey)) {
                var objectByKey = GetObjectByKey(currentShortcut.ObjectKey, modelView.AsObjectView);
                return GetFriendlyObjectKey(modelView, objectByKey);
            }
            return null;
        }

        object GetObjectByKey(string objectKey, IModelObjectView modelObjectView) {
            if (modelObjectView != null && modelObjectView.ModelClass.TypeInfo.IsPersistent) {
                var objectSpace = WebApplication.Instance.CreateObjectSpace();
                var typeInfo = modelObjectView.ModelClass.TypeInfo;
                var convert = ReflectionHelper.Convert(objectKey, typeInfo.KeyMember.MemberType);
                return objectSpace.GetObjectByKey(typeInfo.Type, convert);
            }
            return null;
        }

        string EditModeFriendlyUrl(ViewShortcut currentShortcut, string friendlyUrl,
                                      IModelDetailViewFriendlyUrl modelDetailViewFriendlyUrl) {
            return modelDetailViewFriendlyUrl != null && !string.IsNullOrEmpty(modelDetailViewFriendlyUrl.Url.EditMode)
                       ? currentShortcut.Keys.ToList().Where(
                           (key, i) => key == "mode" && currentShortcut.Values.ToList()[i] == "Edit").Aggregate(
                               friendlyUrl, (current, key) => (modelDetailViewFriendlyUrl).Url.EditMode + "-" + current)
                       : friendlyUrl;
        }

        string GetFriendlyObjectKey(IModelViewFriendlyUrl modelView, object objectByKey) {
            if (modelView.AsObjectView != null && modelView.AsObjectView.ModelClass.TypeInfo.IsPersistent) {
                var friendlyKeyMember = ((IModelDetailViewFriendlyUrl)modelView).Url.ValueMemberName;
                var memberInfo = modelView.AsObjectView.ModelClass.FindMember(friendlyKeyMember).MemberInfo;
                if (objectByKey != null) {
                    var value = memberInfo.GetValue(objectByKey);
                    return value != null ? value.ToString() : null;
                }
            }
            return null;
        }
    }
}

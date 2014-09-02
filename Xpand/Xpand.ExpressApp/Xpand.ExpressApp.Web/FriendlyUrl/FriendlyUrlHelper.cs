﻿using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.Web.FriendlyUrl {
    class FriendlyUrlHelper {
        readonly XpandHttpRequestManager _xpandHttpRequestManager;

        public FriendlyUrlHelper(XpandHttpRequestManager xpandHttpRequestManager) {
            _xpandHttpRequestManager = xpandHttpRequestManager;
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
            var objectSpace = WebApplication.Instance.CreateObjectSpace(modelView.ModelClass.TypeInfo.Type);
            var modelMember = modelView.ModelClass.FindMember(modelView.Url.ValueMemberName);
            var findObject = objectSpace.FindObject(modelView.ModelClass.TypeInfo.Type, CriteriaOperator.Parse(modelMember.Name + "=?", strings[1]));
            return modelView.ModelClass.TypeInfo.KeyMember.GetValue(findObject).ToString();
        }

        string GetViewId(string[] strings) {
            var viewId = strings[0];
            var split = viewId.Split('-');
            var id = split.Length > 1 ? split[1] : viewId;
            return WebApplication.Instance.Model.Views.OfType<IModelViewFriendlyUrl>().First(url => url.FriendlyUrl == id).Id;
        }

        public ViewShortcut GetViewShortCut(string shortcutString) {
            if (WebApplication.Instance.SupportsFriendlyUrl() && shortcutString.Contains("=")) {
                var strings = shortcutString.Split('=');
                var viewId = GetViewId(strings);
                var friendlyObjectKey = GetFriendlyObjectKey(strings, viewId);
                return GetViewShortcut(strings, viewId, friendlyObjectKey);
            }
            return _xpandHttpRequestManager.GetViewShortcut(shortcutString);
        }

        public void WriteShortcutTo(ViewShortcut currentShortcut, NameValueCollection queryString) {
            if (WebApplication.Instance.SupportsFriendlyUrl()) {
                if (!IsNewObjectView(currentShortcut)){
                    var windowId = queryString["WindowId"];
                    queryString.Clear();
                    var modelView = (IModelViewFriendlyUrl)WebApplication.Instance.Model.Views[currentShortcut.ViewId];
                    var objectKey = ObjectKey(currentShortcut, modelView);
                    var friendlyUrl = EditModeFriendlyUrl(currentShortcut, modelView.FriendlyUrl, modelView as IModelDetailViewFriendlyUrl);
                    queryString.Add(friendlyUrl, objectKey);
                    if (!string.IsNullOrEmpty(windowId))
                        queryString.Add("WindowId",windowId);
                }
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
                var typeInfo = modelObjectView.ModelClass.TypeInfo;
                var objectSpace = WebApplication.Instance.CreateObjectSpace(typeInfo.Type);
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
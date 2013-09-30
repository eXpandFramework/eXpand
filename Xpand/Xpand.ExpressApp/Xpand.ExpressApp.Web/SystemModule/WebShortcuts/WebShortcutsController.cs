using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Web.SystemModule.WebShortcuts {
    public interface IModelOptionsWebShortcut {
        IModelWebShortcut WebShortcut { get; }
    }

    public interface IModelWebShortcut:IModelNode {
        [DefaultValue(true)]
        bool Enabled { get; set; }
        [DefaultValue("Control+Alt+Shift+N")]
        string CtrlNReplacement { get; set; }
        [DefaultValue("Control+Alt+Shift+T")]
        string CtrlTReplacement { get; set; }
    }

    public class WebShortcutsController : WindowController, IXafCallbackHandler,IModelExtender {
        private const string KeybShortCutsScriptName = "KeybShortCuts";
        protected override void OnActivated() {
            base.OnActivated();
            if (((IModelOptionsWebShortcut) Application.Model.Options).WebShortcut.Enabled) {
                var webWindow = ((WebWindow)Frame);
                webWindow.PagePreRender += WebWindowOnPagePreRender;
                if (Frame.Template != null) {
                    Frame.TemplateChanging += Frame_TemplateChanging;
                    ((Page)Frame.Template).Load += OnLoad;
                } else {
                    Frame.TemplateChanged += Frame_TemplateChanged;
                }
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (((IModelOptionsWebShortcut)Application.Model.Options).WebShortcut.Enabled) {
                var webWindow = ((WebWindow) Frame);
                webWindow.PagePreRender -= WebWindowOnPagePreRender;
                Frame.TemplateChanging -= Frame_TemplateChanging;
                Frame.TemplateChanged -= Frame_TemplateChanged;
            }
        }

        void WebWindowOnPagePreRender(object sender, EventArgs e) {
            var script = GetScript();
            if (!string.IsNullOrEmpty(script))
                ((WebWindow)Frame).RegisterStartupScript("ActionKeybShortCuts", script, true);
        }

        void OnLoad(object sender, EventArgs eventArgs) {
            ((ICallbackManagerHolder)Frame.Template).CallbackManager.RegisterHandler(KeybShortCutsScriptName, this);
            var webWindow = ((WebWindow)Frame);
            var jwerty = GetType().GetResourceString("jwerty.js");
            webWindow.RegisterClientScript("jwerty", jwerty, true);
        }

        private void Frame_TemplateChanging(object sender, EventArgs e) {
            if (Frame.Template != null) {
                ((Page)Frame.Template).Load -= OnLoad;
            }
        }
        private void Frame_TemplateChanged(object sender, EventArgs e) {
            if (Frame.Template != null) {
                Frame.TemplateChanging += Frame_TemplateChanging;
                ((Page)Frame.Template).Load += OnLoad;
            }
        }

        string GetScript() {
            var actions = Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions).Where(@base => @base.Enabled & @base.Active);
            var script = actions.Where(@base => !string.IsNullOrEmpty(@base.Shortcut)).Select(ReplaceUnsupportedShortcuts).Select(GetScriptCore);
            return string.Join(Environment.NewLine, script);
        }

        Keys ShortcutToKeys(string str) {
            if (!string.IsNullOrEmpty(str)) {
                object value;
                if (typeof (Shortcut).EnumTryParse(str, out value))
                    return (Keys) value;
                if (typeof (Keys).EnumTryParse(str, out value))
                    return (Keys) value;
                if (str.Contains("+")) {
                    return str.Split('+')
                              .Aggregate(Keys.None, (current, item) => current | (Keys) Enum.Parse(typeof (Keys), item));
                }
            }
            return Keys.None;
        }

        KeyValuePair<ActionBase,string> ReplaceUnsupportedShortcuts(ActionBase actionBase) {
            var shortcutToKeys = ShortcutToKeys(actionBase.Shortcut);
            if (((shortcutToKeys & Keys.Control) == Keys.Control)) {
                var modelWebShortcut = ((IModelOptionsWebShortcut)Application.Model.Options).WebShortcut;
                if (((shortcutToKeys & Keys.N) == Keys.N)) {
                    shortcutToKeys =ShortcutToKeys(modelWebShortcut.CtrlNReplacement);
                }
                else if (((shortcutToKeys & Keys.T) == Keys.T)) {
                    shortcutToKeys =ShortcutToKeys(modelWebShortcut.CtrlTReplacement);
                }
            }
            return new KeyValuePair<ActionBase, string>(actionBase, KeyShortcut.GetKeyDisplayText(shortcutToKeys).Replace(KeyShortcut.ControlKeyName, "ctrl"));
        }

        string GetScriptCore(KeyValuePair<ActionBase, string> keyValuePair) {
            var xafCallbackManager = ((ICallbackManagerHolder) Frame.Template).CallbackManager;
            return @"jwerty.key(""" + keyValuePair.Value + @""", function(e) {" +
                  xafCallbackManager.GetScript(KeybShortCutsScriptName, "'" + keyValuePair.Key.Id+"'") + @"return false;});";
        }

        public void ProcessAction(string parameter) {
            var actionBase = Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions).First(@base => @base.Id == parameter);
            actionBase.DoExecute();
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions,IModelOptionsWebShortcut>();
        }
    }
}

using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.SystemModule {
    public delegate XafApplication GetApplicationInstanceCallback();

    public class SessionStateController : WindowController {
        public static GetApplicationInstanceCallback GetApplicationInstance;
        readonly List<CachedViewShortcut> cachedShortcuts;


        public SessionStateController() {
            cachedShortcuts = new List<CachedViewShortcut>();
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
        }

        protected override void OnAfterConstruction() {
            base.OnAfterConstruction();
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            Application.CustomProcessShortcut += Application_CustomProcessShortcut;
        }


        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            Application.CustomProcessShortcut -= Application_CustomProcessShortcut;
        }

        public void Application_CustomProcessShortcut(object sender, CustomProcessShortcutEventArgs e) {
            int i = 0;

            while ((i < cachedShortcuts.Count) && (cachedShortcuts[i].Shortcut != e.Shortcut))
                i++;

            if (i < cachedShortcuts.Count) {
                e.Shortcut.ObjectKey = cachedShortcuts[i].Parameter.CurrentValue.ToString();
            }
            else if (e.Shortcut.ObjectKey.StartsWith("@")) {
                var cachedShortcut = new CachedViewShortcut(e.Shortcut,ParametersFactory.CreateParameter(e.Shortcut.ObjectKey.Substring(1)));
                cachedShortcuts.Add(cachedShortcut);
                e.Shortcut.ObjectKey = cachedShortcut.Parameter.CurrentValue.ToString();
            }
        }
        #region Nested type: CachedViewShortcut
        class CachedViewShortcut {
            public CachedViewShortcut(ViewShortcut shortcut, IParameter parameter) {
                Shortcut = shortcut;

                Parameter = parameter;
            }

            public ViewShortcut Shortcut { get; private set; }

            public IParameter Parameter { get; private set; }
        }
        #endregion
    }
}
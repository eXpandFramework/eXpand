using System;
using DevExpress.ExpressApp.Localization;

namespace DevExpress.ExpressApp.ConditionalEditorState {
    [System.ComponentModel.DisplayName("ConditionalEditorState Module")]
    public class EditorStateLocalizer : XafResourceLocalizer {
        private static EditorStateLocalizer activeLocalizer;
        static EditorStateLocalizer() {
            activeLocalizer = new EditorStateLocalizer();
        }
        protected override IXafResourceManagerParameters GetXafResourceManagerParameters() {
            return new XafResourceManagerParameters(
                ConditionalEditorStateNodeWrapper.NodeName,
                "DevExpress.ExpressApp.ConditionalEditorState.LocalizationResources",
                String.Empty,
                GetType().Assembly
                );
        }
        public static EditorStateLocalizer Active {
            get { return activeLocalizer; }
            set { activeLocalizer = value; }
        }
        public override void Activate() {
            Active = this;
        }
    }
}

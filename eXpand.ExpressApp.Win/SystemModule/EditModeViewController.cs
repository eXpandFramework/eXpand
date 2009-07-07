using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class EditModeViewController : ViewController {
        private const string useViewEditModeAttribute = @"UseViewEditMode";

        public EditModeViewController() {
            InitializeComponent();
            RegisterActions(components);
        }

        public override Schema GetSchema() {
            const string s = @"<Element Name=""Application"">;
                            <Element Name=""Views"">
                                <Element Name=""DetailView"">;
                                    <Attribute Name=""" + useViewEditModeAttribute + @""" Choice=""False,True""/>
                                </Element>
                            </Element>
                    </Element>";

            return new Schema(new DictionaryXmlReader().ReadFromString(s));
        }
        public bool HasViewEditMode { get; private set; }

        public void SetViewEditState(ViewEditMode value) {
            toggleEditMode.Enabled["ViewEditMode"] = value == ViewEditMode.View ? true : false ;
            View.ReadOnly["ViewEditMode"] = value == ViewEditMode.View ? true : false;
            Frame.GetController<DetailViewController>().CancelAction.Enabled["Is modified"] = true;
            Frame.GetController<DetailViewController>().SaveAndNewAction.Active["ViewEditMode"] = false;
        }

        protected override void OnActivated() {
            base.OnActivated();

            HasViewEditMode = View.Info.GetAttributeBoolValue(useViewEditModeAttribute);
            toggleEditMode.Active["ViewEditMode"] = HasViewEditMode;
            
            if (!HasViewEditMode) return;
            
            Frame.GetController<DetailViewController>().CancelAction.Executed += (sender, e) => { SetViewEditState(ViewEditMode.View); };
            Frame.GetController<DetailViewController>().SaveAction.Executed += (sender, e) => { SetViewEditState(ViewEditMode.View); };
            toggleEditMode.Execute += (sender, e) => { SetViewEditState(ViewEditMode.Edit); };
            SetViewEditState(ViewEditMode.View);
        }
    }
}
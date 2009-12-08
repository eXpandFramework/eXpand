using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.SystemModule
{
    public partial class CustomSelectionListViewViewController : BaseViewController
    {
        public const string CustomSelection = "CustomSelection";
        public CustomSelectionListViewViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType=ViewType.ListView;
            

        }
        private bool hasCustomSelection;
        public bool HasCustomSelection
        {
            get { return hasCustomSelection; }
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            hasCustomSelection = Application.FindClassInfo(View.ObjectTypeInfo.Type).GetAttributeBoolValue(CustomSelection);
            if (hasCustomSelection) {
                if (View.ObjectTypeInfo.FindMember(CustomSelection) == null) {
                    IMemberInfo member = View.ObjectTypeInfo.CreateMember(CustomSelection, typeof (bool));
                    member.AddAttribute(new NonPersistentAttribute());
                    XafTypesInfo.Instance.RefreshInfo(View.ObjectTypeInfo);
                }
            }
        }



        public override Schema GetSchema()
        {
            const string s = @"<Element Name=""Application"">;
                            <Element Name=""BOModel"">
                                <Element Name=""Class"">;
                                    <Attribute Name=""" + CustomSelection + @""" Choice=""False,True""/>
                                </Element>
                            </Element>
                    </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(s));
        }
    }
}
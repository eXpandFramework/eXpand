using DevExpress.ExpressApp;
using eXpand.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class SupressConfirmationController : BaseViewController
    {
        public const string SupressConfirmationAttribute = "SupressConfirmation";
        private WinDetailViewController winDetailViewController;
        public SupressConfirmationController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            winDetailViewController = Frame.GetController<WinDetailViewController>();
            winDetailViewController.SuppressConfirmation = View.Info.GetAttributeBoolValue(SupressConfirmationAttribute);

            if (View is DetailView && ObjectSpace.IsNewObject(View.CurrentObject))
            {
                ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
                winDetailViewController.SuppressConfirmation = true;
            }
        }

        protected override void OnDeactivating()
        {
            ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
            base.OnDeactivating();
        }

        private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            winDetailViewController.SuppressConfirmation = false;
        }

        public override Schema GetSchema()
        {
            const string CommonTypeInfos = @"<Element Name=""Application"">
                        <Element Name=""Views"" >
                            <Element Name=""DetailView"">
                                 <Attribute Name=""" + SupressConfirmationAttribute + @""" Choice=""False,True""/>
                            </Element>
                            <Element Name=""ListView"">
                                 <Attribute Name=""" + SupressConfirmationAttribute + @""" Choice=""False,True""/>
                            </Element>
                        </Element>
                    </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }

    }
}
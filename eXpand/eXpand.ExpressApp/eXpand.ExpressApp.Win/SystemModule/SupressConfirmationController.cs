using DevExpress.ExpressApp;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class SupressConfirmationController : BaseViewController
    {
        public const string SupressConfirmationAttribute = "SupressConfirmation";
        public SupressConfirmationController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            var winDetailViewController = Frame.GetController<DevExpress.ExpressApp.Win.SystemModule.WinDetailViewController>();
            if (winDetailViewController!= null)
                winDetailViewController.SuppressConfirmation = View.Info.GetAttributeBoolValue(SupressConfirmationAttribute);
        }
        public override Schema GetSchema()
        {
            const string CommonTypeInfos = @"<Element Name=""Application"">
                        <Element Name=""Views"" >
                            <Element Name=""DetailView"">
                                 <Attribute Name=""" +SupressConfirmationAttribute +@""" Choice=""False,True""/>
                            </Element>
                            <Element Name=""ListView"">
                                 <Attribute Name=""" +SupressConfirmationAttribute +@""" Choice=""False,True""/>
                            </Element>
                        </Element>
                    </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }

    }
}
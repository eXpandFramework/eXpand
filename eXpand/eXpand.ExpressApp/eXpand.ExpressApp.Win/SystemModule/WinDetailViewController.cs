using DevExpress.ExpressApp;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class WinDetailViewController : BaseViewController
    {
        public const string SupressConfirmationAttribute = "SupressConfirmation";
        public WinDetailViewController()
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
        ///<summary>
        ///
        ///<para>
        ///Returns the Schema extension which is combined with the entire Schema when loading the Application Model.
        ///
        ///</para>
        ///
        ///</summary>
        ///
        ///<returns>
        ///The <b>Schema</b> object that represents the Schema extension to be added to the application's entire Schema.
        ///
        ///</returns>
        ///
        public override Schema GetSchema()
        {
            //            if (typeof (WinModule) == GetType())
            //            {
            const string CommonTypeInfos = @"<Element Name=""Application"">
                        <Element Name=""Views"" >
                            <Element Name=""DetailView"">
                                 <Attribute Name=""" +
                                           SupressConfirmationAttribute +
                                           @""" Choice=""False,True""/>
                            </Element>
                        </Element>
                    </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }

    }
}
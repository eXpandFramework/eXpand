using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.Win.ListEditors;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class HideGridPopUpMenuViewController : BaseViewController
    {
        public const string HidePopupMenu = "HidePopupMenu";

        public HideGridPopUpMenuViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType=ViewType.ListView;
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
            const string CommonTypeInfos = @"<Element Name=""Application"">
                        <Element Name=""Views"" >
                            <Element Name=""ListView"" >
                                <Attribute Name=""" +
                                           HidePopupMenu +
                                           @""" Choice=""False,True""/>
                            </Element>
                        </Element>
                    </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            ListEditor listEditor = ((ListView) View).Editor;
            if (listEditor is IPopupMenuHider)
                ((IPopupMenuHider) listEditor).HidePopupMenu = View.Info.GetAttributeBoolValue(HidePopupMenu);
        }
    }
}
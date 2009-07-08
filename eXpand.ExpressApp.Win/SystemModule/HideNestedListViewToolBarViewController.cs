using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.XtraBars;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class HideNestedListViewToolBarViewController : BaseViewController
    {
        public const string HideToolBar = "HideToolBar";
        public HideNestedListViewToolBarViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewNesting=Nesting.Nested;
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
                                           HideToolBar +
                                           @""" Choice=""False,True""/>
                            </Element>
                        </Element>
                    </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            View.ControlsCreated+=View_OnControlsCreated;
        }

        private void View_OnControlsCreated(object sender, EventArgs e)
        {
            if (Frame.Template is NestedFrameTemplate)
            {
                Bar bar = ((NestedFrameTemplate) Frame.Template).BarManager.Bars[0];
                bar.Visible = !View.Info.GetAttributeBoolValue(HideToolBar);
            }
        }
    }
}
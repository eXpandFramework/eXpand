using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class TabStopViewController : BaseViewController
    {
        public const string TabStopForReadOnly = "TabStopForReadOnly";

        public TabStopViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType=ViewType.DetailView;

        }

        protected override void OnActivated()
        {
            base.OnActivated();
            View.ControlsCreated+=View_OnControlsCreated;
        }


        public override Schema GetSchema()
        {
            const string CommonTypeInfos = @"<Element Name=""Application"">
                    <Element Name=""Options"" >
                            <Attribute Name=""" + TabStopForReadOnly + @""" Choice=""False,True""/>
                    </Element>
                </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));

        }

        private void View_OnControlsCreated(object sender, EventArgs e)
        {
            if (!Application.Info.GetChildNodeByPath("Options").GetAttributeBoolValue(TabStopForReadOnly))
            {
                var detailView = (DetailView) View;
                LayoutControl layout = ((WinLayoutManager) (detailView.LayoutManager)).Container;
                layout.OptionsFocus.EnableAutoTabOrder = false;
                foreach (DetailViewItem item in ((DetailView) View).GetItems<DetailViewItem>())
                {
                    if (item is PropertyEditor)
                    {
                        if (!(((PropertyEditor)item).AllowEdit))
                        {
                            if (item.Control is TextEdit)
                                ((TextEdit) item.Control).TabStop = false;
                            continue;
                        }
                    }
                }
            }
        }



    }
}
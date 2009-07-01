using System;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class AddRuntimeFieldsFromModelToXPDictionary :
        BaseWindowController
    {
        private XPDictionary xpDictionary;
        
        [DebuggerNonUserCode]
        public override Schema GetSchema()
        {
            return new Schema(new DictionaryXmlReader().ReadFromString(
                                  @"<?xml version=""1.0""?>" +
                                  @"<Element Name=""Application"">" +
                                  @"	<Element Name=""BOModel"">" +
                                  @"		<Element Name=""Class"">" +
                                  @"			<Element Name=""Member"">" +
                                  @"			    <Attribute	IsNewNode=""True"" Name=""" + DictionaryHelper.IsRuntimeMember +
                                  @"""" +
                                  @"						Choice=""True,False""" + @"/>" +
                                  @"			</Element>" +
                                  @"		</Element>" +
                                  @"	</Element>" +
                                  @"</Element>"));
        }

        public AddRuntimeFieldsFromModelToXPDictionary()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            if (typeof(WinComponent).IsAssignableFrom(Application.GetType()))
            {
                ((WinComponent) Application).ModelEditFormShowning +=
                    (sender, e) =>
                    e.ModelEditorForm.Controller.Saving += ModelEditorController_OnSaving;
            }
        }


        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            XPDictionary dictionary = XafTypesInfo.XpoTypeInfoSource.XPDictionary;
            xpDictionary = dictionary;
        }


        public XPDictionary XpDictionary
        {
            get { return xpDictionary; }
        }

        private void ModelEditorController_OnSaving(object sender, EventArgs e)
        {
            DictionaryHelper.AddFields(((ModelEditorController)sender).RootNode, XpDictionary);
        }

    }
}
using System;
using System.Diagnostics;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.SystemModule
{
    public partial class AddRuntimeFieldsFromModelToXPDictionary :ViewController
    {
        public AddRuntimeFieldsFromModelToXPDictionary()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (IXpoDictionaryStore);
        }

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


        protected override void OnActivated()
        {
            base.OnActivated();
            View.ObjectSpace.Committed+=ObjectSpaceOnCommitted;
            
        }
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            View.ObjectSpace.Committed -= ObjectSpaceOnCommitted;
        }
        private void ObjectSpaceOnCommitted(object sender, EventArgs args)
        {
            DictionaryHelper.AddFields(Application.Info, XafTypesInfo.XpoTypeInfoSource.XPDictionary);
        }
    }
}
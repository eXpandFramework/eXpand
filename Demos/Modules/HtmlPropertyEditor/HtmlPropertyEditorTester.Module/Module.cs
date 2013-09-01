using System;
using System.Text;
using System.Linq;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;

namespace HtmlPropertyEditorTester.Module {
    // You can override various virtual methods and handle corresponding events to manage various aspects of your XAF application UI and behavior.
    public sealed partial class HtmlPropertyEditorTesterModule : ModuleBase { // http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppModuleBasetopic
        public HtmlPropertyEditorTesterModule() {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }

        // Override the CustomizeTypesInfo method, to customize information on a particular class or property, before it is loaded to the Application Model. 
        //public override void CustomizeTypesInfo(ITypesInfo typesInfo) { // http://documentation.devexpress.com/Xaf/CustomDocument3224.aspx
        //    base.CustomizeTypesInfo(typesInfo);
        //    ITypeInfo theTypeInfo = typesInfo.FindTypeInfo(typeof(DomainObject1));
        //    if(theTypeInfo != null) {
        //        string memberName = "MyCustomMember";
        //        IMemberInfo theMemberInfo = theTypeInfo.FindMember(memberName);
        //        if(theMemberInfo == null) {
        //            theMemberInfo = theTypeInfo.CreateMember(memberName, typeof(string));
        //        }
        //        theTypeInfo.AddAttribute(new VisibleInDetailViewAttribute(false));
        //    }
        //}

        // You can define a fully custom Application Model element or extend an existing one (http://documentation.devexpress.com/#Xaf/CustomDocument3169)
        //public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
        //    base.ExtendModelInterfaces(extenders);
        //    extenders.Add<IModelApplication, IModelMyModelExtension>();
        //}
    }

    // Declaration of a custom Application Model element extension to keep your custom settings (http://documentation.devexpress.com/#Xaf/CustomDocument2579).
    //public interface IModelMyModelExtension : IModelNode {
    //    string MyCustomProperty { get; set; }
    //}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.Xpo.DB;
using System.Linq;

namespace eXpand.ExpressApp.SystemModule
{
    [ToolboxItem(true)]
    [Description("Includes Controllers that represent basic features for XAF applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof (XafApplication), "Resources.SystemModule.ico")]
    public sealed partial class eXpandSystemModule : ModuleBase
    {
        private string _connectionString;
        private const string ProxyDataStoreSelectDataAttributeName = "ProxyDataStoreSelectData";
        private const string MergingClassAttributeName = "MergingClass";
        private const string ObjectTypeAttributeName = "ObjectType";

        public override void UpdateModel(Dictionary model)
        {
            base.UpdateModel(model);
            var wrapper = new ApplicationNodeWrapper(model);
            if (!DesignMode) {
                using (var session = new Session {ConnectionString = _connectionString}) {
                    foreach (var classInfoNodeWrapper in wrapper.BOModel.Classes.Where(nodeWrapper => nodeWrapper.ClassTypeInfo.IsPersistent)) {
                        var objectType = session.GetObjectType(XafTypesInfo.XpoTypeInfoSource.GetEntityClassInfo(classInfoNodeWrapper.ClassTypeInfo.Type));
                        classInfoNodeWrapper.Node.SetAttribute(ObjectTypeAttributeName, objectType.Oid);
                    }
                }
            }
            wrapper.Node.GetChildNode("Options").SetAttribute("UseServerMode", "True");
            
        }

        public override void ValidateModel(Dictionary model)
        {
            base.ValidateModel(model);
            DictionaryHelper.AddFields(model.RootNode, XafTypesInfo.XpoTypeInfoSource.XPDictionary);
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.CreateCustomObjectSpaceProvider += (sender, args) => _connectionString = args.ConnectionString;
            application.SetupComplete += (sender, args) => {
                                             DictionaryHelper.AddFields(application.Info,
                                                                        application.ObjectSpaceProvider.XPDictionary);
                                             if (Application.Info.GetChildNode("Options").GetAttributeBoolValue(ProxyDataStoreSelectDataAttributeName))
                                                proxyDataStore((XafApplication) sender);
                                         };
            application.LoggedOn +=(sender, args) =>DictionaryHelper.AddFields(application.Info, application.ObjectSpaceProvider.XPDictionary);
        }
        private void proxyDataStore(XafApplication application)
        {
            var objectSpaceProvider = (application.ObjectSpaceProvider);
            if (!(objectSpaceProvider is IObjectSpaceProvider))            {
                throw new NotImplementedException("ObjectSpaceProvider does not implement " + typeof(IObjectSpaceProvider).FullName);
            }
            XpoDataStoreProxy proxy = ((IObjectSpaceProvider)objectSpaceProvider).DataStoreProvider.Proxy;
                proxy.DataStoreSelectData += Proxy_DataStoreSelectData;
        }
        public string FindClassNameInDictionary(string tableName)
        {
            return (from cl in Application.XPDictionary.Classes.Cast<XPClassInfo>()
                    where cl.TableName == tableName && cl.ClassType != null
                    select cl.ClassType.FullName).FirstOrDefault();
        }

        private void Proxy_DataStoreSelectData(object sender, DataStoreSelectDataEventArgs e) {
            var wrapper = new ApplicationNodeWrapper(Application.Info);
            var classInfoNamesForStatements = getClassInfoNamesForStatements(e, wrapper);
            var classInfoNodeWrappers = wrapper.BOModel.Classes.Where(nodeWrapper => nodeWrapper.Node!= null&& classInfoNamesForStatements.Contains(nodeWrapper.Node.GetAttributeValue(MergingClassAttributeName)));
            var objectMerger = new ObjectMerger(e.SelectStatements);
            foreach (var classInfoNodeWrapper in classInfoNodeWrappers) {
                var mergingClassName = classInfoNodeWrapper.Node.GetAttributeValue(MergingClassAttributeName);
                var objectType = wrapper.BOModel.FindClassByName(mergingClassName).Node.GetAttributeIntValue(ObjectTypeAttributeName);
                var objectTypeToMerge = classInfoNodeWrapper.Node.GetAttributeIntValue(ObjectTypeAttributeName);
                objectMerger.Merge(objectTypeToMerge, objectType);
            }
        }

        private IEnumerable<string> getClassInfoNamesForStatements(DataStoreSelectDataEventArgs e, ApplicationNodeWrapper wrapper) {
            var statements = e.SelectStatements.Select(statement => wrapper.BOModel.FindClassByName(FindClassNameInDictionary(statement.TableName))).Where(nodeWrapper => nodeWrapper!=
                                                                                                                                                                          null).Select(nodeWrapper => nodeWrapper.Name).ToList();
            return statements;
        }

        public override Schema GetSchema()
        {
            const string s = @"<Element Name=""Application"">;
                            <Element Name=""BOModel"">
                                <Element Name=""Class"">
                                    <Attribute  Name=""" + MergingClassAttributeName + @""" Choice=""True,False"" RefNodeName=""/Application/BOModel/Class""/>
                                    <Attribute  Name=""" + ObjectTypeAttributeName + @"""  />
                                </Element>
                            </Element>
                            <Element Name=""Options"">
                                <Attribute  Name=""" + ProxyDataStoreSelectDataAttributeName+ @""" Choice=""True,False"" />    
                            </Element>
                    </Element>";
            
            return new Schema(new DictionaryXmlReader().ReadFromString(s));
        }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.FilterDataStore.Core;
using eXpand.Xpo;
using eXpand.Xpo.DB;


namespace eXpand.ExpressApp.FilterDataStore
{
    public sealed partial class FilterDataStoreModule : ModuleBase
    {
        public const string FilterDataStoreModuleAttributeName = "FilterDataStoreModule";
        public const string DisabledDataStoreFiltersAttributeName = "DisabledDataStoreFilters";
//        public const string FilterIsSharedAttributeName = "FilterIsShared";
        public override Schema GetSchema()
        {
            const string s = @"<Element Name=""Application"">;
                            
                            <Element Name=""BOModel"">
                                <Element Name=""Class"">
                                    <Element Name=""" + DisabledDataStoreFiltersAttributeName + @""">
                                        <Element Name=""Item"" KeyAttribute=""Name"" DisplayAttribute=""Name"" Multiple=""True"">
                                            <Attribute  Name=""Name"" />
                                        </Element>
                                    </Element>
                                    
                                </Element>
                            </Element>
                            
                            <Element Name=""" + FilterDataStoreModuleAttributeName + @""">
                                    <Attribute  Name=""Enabled"" Choice=""False,True""/>
                                    <Element Name=""SystemTables"">
                                        <Element Name=""Item"" KeyAttribute=""Name"" DisplayAttribute=""Name"" Multiple=""True"">
                                            <Attribute  Name=""Name"" />
                                        </Element>
                                    </Element>
                            </Element>
                    </Element>";

            return new Schema(new DictionaryXmlReader().ReadFromString(s));
        }
        public override void UpdateModel(Dictionary model)
        {
            base.UpdateModel(model);
            var applicatioNodeWrapper = new ApplicationNodeWrapper(model);
            foreach (ClassInfoNodeWrapper clw in applicatioNodeWrapper.BOModel.Classes)
                clw.Node.AddChildNode(DisabledDataStoreFiltersAttributeName);
        }

        public FilterDataStoreModule()
        {
            InitializeComponent();
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);
            ITypeInfo info = typesInfo.FindTypeInfo(typeof(PersistentBase));
            if (FilterProviderManager.Providers != null)
                foreach (FilterProviderBase provider in FilterProviderManager.Providers)
                {
                    if (info.FindMember(provider.FilterMemberName) == null)
                        CreateMember(info, provider);
                }
        }

        public static void CreateMember(ITypeInfo typeInfo, FilterProviderBase provider)
        {
            var attributes = new List<Attribute>
                                 {
                                     new BrowsableAttribute(false),
                                     new MemberDesignTimeVisibilityAttribute(
                                         false)
                                 };

            IMemberInfo member = typeInfo.CreateMember(provider.FilterMemberName, provider.FilterMemberType);
            if (provider.FilterMemberIndexed)
                attributes.Add(new IndexedAttribute());
            if (provider.FilterMemberSize != SizeAttribute.DefaultStringMappingFieldSize)
                attributes.Add(new SizeAttribute(provider.FilterMemberSize));
            foreach (Attribute attribute in attributes)
                member.AddAttribute(attribute);
        }

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.SetupComplete += application_SetupComplete;
        }
        void application_SetupComplete(object sender, EventArgs e)
        {
            var objectSpaceProvider = (((XafApplication)(sender)).ObjectSpaceProvider);
            if (objectSpaceProvider is IObjectSpaceProvider){
                throw new NotImplementedException("ObjectSpaceProvider does not implement " + typeof(IObjectSpaceProvider).FullName);
            }
            XpoDataStoreProxy proxy = ((IObjectSpaceProvider)objectSpaceProvider).DataStoreProvider.Proxy;
            if (Application.Info.GetChildNode(FilterDataStoreModuleAttributeName).GetAttributeBoolValue("Enabled"))
            {
                proxy.DataStoreModifyData += (o,args) => ModifyData(args.ModificationStatements);
                proxy.DataStoreSelectData += Proxy_DataStoreSelectData;
            }
        }

        private void Proxy_DataStoreSelectData(object sender, DataStoreSelectDataEventArgs e)
        {
            filterData(e.SelectStatements);
        }

        public void ModifyData(ModificationStatement[] statements)
        {
            InsertData(statements.OfType<InsertStatement>());
            UpdateData(statements.OfType<UpdateStatement>());
        }

        public void UpdateData(IEnumerable<UpdateStatement> statements)
        {
            foreach (UpdateStatement statement in statements)
            {
                traceStatement(statement,"UpdateData");
                if (!IsSystemTable(statement.TableName))
                {
                    List<QueryOperand> operands = statement.Operands.OfType<QueryOperand>().ToList();
                    for (int i = 0; i < operands.Count(); i++)
                    {
                        int index = i;
                        FilterProviderBase providerBase =FilterProviderManager.GetFilterProvider(operands[index].ColumnName);
                        if (providerBase != null && !FilterIsShared(statement.TableName,providerBase.Name))
                            statement.Parameters[i].Value = providerBase.FilterValue;
                    }
                }
                
            }
        }


        public void InsertData(IEnumerable<InsertStatement> statements)
        {
            foreach (InsertStatement statement in statements)
            {
                traceStatement(statement, "InsertData");
                if (!IsSystemTable(statement.TableName))
                {
                    List<QueryOperand> operands = statement.Operands.OfType<QueryOperand>().ToList();
                    for (int i = 0; i < operands.Count(); i++)
                    {
                        FilterProviderBase providerBase =
                            FilterProviderManager.GetFilterProvider(operands[i].ColumnName);
                        if (providerBase != null && !FilterIsShared(statement.TableName,providerBase.Name)) 
                            statement.Parameters[i].Value = providerBase.FilterValue;
                    }
                }
            }

        }


        public BaseStatement[] filterData(SelectStatement[] statements)
        {
            var baseStatements = new List<SelectStatement>();
            foreach (SelectStatement statement in statements)
                baseStatements.Add(ApplyCondition(statement));
            return baseStatements.ToArray();
        }

        public SelectStatement ApplyCondition(SelectStatement statement)
        {
            var extractor = new CriteriaOperatorExtractor();
            extractor.Extract(statement.Condition);
            traceStatement(statement, "ApplyCondition");

            foreach (FilterProviderBase provider in FilterProviderManager.Providers)
            {
                FilterProviderBase providerBase = FilterProviderManager.GetFilterProvider(provider.FilterMemberName);
                if (providerBase!= null)
                {
                    Tracing.Tracer.LogVerboseValue("providerName", providerBase.Name);
                    IEnumerable<BinaryOperator> binaryOperators =
                        extractor.BinaryOperators.Where(
                            @operator =>
                            @operator.RightOperand is OperandValue &&
                            ReferenceEquals(((OperandValue) @operator.RightOperand).Value, providerBase.FilterMemberName));
                    if (!FilterIsShared(statement.TableName,providerBase.Name) && binaryOperators.Count() == 0&&!IsSystemTable(statement.TableName))
                    {
                        string s = providerBase.FilterValue== null? null:providerBase.FilterValue.ToString();


                        string nodeAlias = statement.Operands.OfType<QueryOperand>().Where(
                                               operand => operand.ColumnName == providerBase.FilterMemberName).Select(
                                               operand => operand.NodeAlias).FirstOrDefault() ?? statement.Alias;
                        statement.Condition &= new QueryOperand(providerBase.FilterMemberName, nodeAlias) ==s;
                        Tracing.Tracer.LogVerboseValue("new_statement", statement);
                    }
                }
            }
            return statement;
        }

        private void traceStatement(JoinNode statement, string methodName)
        {
            Tracing.Tracer.LogVerboseSubSeparator("Filter DataStore -- "+ methodName);
            Tracing.Tracer.LogVerboseValue("statement.TableName", statement.TableName);
            Tracing.Tracer.LogVerboseValue("statement", statement);
            Tracing.Tracer.LogVerboseValue("FilterProviderManager.Providers.Count", FilterProviderManager.Providers.Count);
        }


        private bool IsSystemTable(string name)
        {
            bool ret = false;
            DictionaryNode dictionaryNode = Application.Info.GetChildNode(FilterDataStoreModuleAttributeName).GetChildNode("SystemTables");
            foreach (DictionaryNode childNode in dictionaryNode.ChildNodes){
                if (childNode.GetAttributeValue("Name")==name)
                    ret= true;
            }
            Tracing.Tracer.LogVerboseValue("IsSystemTable", ret);
            return ret;
        }

        public string FindClassNameInDictionary(string tableName)
        {
            return (from cl in Application.XPDictionary.Classes.Cast<XPClassInfo>()
                    where cl.TableName == tableName &&cl.ClassType!= null
                    select cl.ClassType.FullName).FirstOrDefault();
        }

        public bool FilterIsShared(string tableName,string providerName)
        {
            bool ret = false;
            string classNameInDictionary = FindClassNameInDictionary(tableName);
            if (classNameInDictionary != null)
            {
                var classInfoNodeWrapper = new ApplicationNodeWrapper(Application.Info).BOModel.FindClassByName(classNameInDictionary);
                if (classInfoNodeWrapper != null)
                    foreach (
                        DictionaryNode childNode in
                            classInfoNodeWrapper.Node.GetChildNode(DisabledDataStoreFiltersAttributeName).ChildNodes)
                        if (childNode.GetAttributeValue("Name") == providerName)
                            ret =true;
            }
            Tracing.Tracer.LogVerboseValue("FilterIsShared", ret);
            return ret;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.FilterDataStore.Core;
using Xpand.ExpressApp.FilterDataStore.Model;
using Xpand.Xpo.DB;
using Xpand.Xpo.Filtering;

namespace Xpand.ExpressApp.FilterDataStore {
    public abstract class FilterDataStoreModuleBase:XpandModuleBase {
        static FilterDataStoreModuleBase() {
            _tablesDictionary = new Dictionary<string, Type>();
        }

        static readonly Dictionary<string, Type> _tablesDictionary ;
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (ProxyEventsSubscribed.HasValue&&ProxyEventsSubscribed.Value) {
                SubscribeToDataStoreProxyEvents();
            }
        }
        void SubscribeToDataStoreProxyEvents() {
            if (Application != null && Application.ObjectSpaceProvider != null) {
                var objectSpaceProvider = (Application.ObjectSpaceProvider);
                if (!(objectSpaceProvider is IXpandObjectSpaceProvider)) {
                    throw new NotImplementedException("ObjectSpaceProvider does not implement " + typeof(IXpandObjectSpaceProvider).FullName);
                }
                SqlDataStoreProxy proxy = ((IXpandObjectSpaceProvider)objectSpaceProvider).DataStoreProvider.Proxy;
                proxy.DataStoreModifyData += (o, args) => ModifyData(args.ModificationStatements);
                proxy.DataStoreSelectData += Proxy_DataStoreSelectData;
                ProxyEventsSubscribed = true;
            }
        }

        protected abstract bool? ProxyEventsSubscribed { get; set; }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (FilterProviderManager.IsRegistered&& FilterProviderManager.Providers != null) {
                SubscribeToDataStoreProxyEvents();
                CreateMembers(typesInfo);
                foreach (var persistentType in typesInfo.PersistentTypes.Where(info => info.IsPersistent)) {
                    var xpClassInfo = XafTypesInfo.XpoTypeInfoSource.GetEntityClassInfo(persistentType.Type);
                    if (xpClassInfo.TableName != null && xpClassInfo.ClassType != null) {
                        if (!IsMappedToParent(xpClassInfo))
                            _tablesDictionary.Add(xpClassInfo.TableName, xpClassInfo.ClassType);
                    }
                }
            }
        }

        bool IsMappedToParent(XPClassInfo xpClassInfo) {
            var attributeInfo = xpClassInfo.FindAttributeInfo(typeof(MapInheritanceAttribute));
            return attributeInfo != null &&
                   ((MapInheritanceAttribute) attributeInfo).MapType == MapInheritanceType.ParentTable;
        }

        void CreateMembers(ITypesInfo typesInfo) {
            foreach (FilterProviderBase provider in FilterProviderManager.Providers) {
                FilterProviderBase provider1 = provider;
                foreach (ITypeInfo typeInfo in typesInfo.PersistentTypes.Where(
                    typeInfo => !(typeInfo.IsAbstract) && typeInfo.FindMember(provider1.FilterMemberName) == null && typeInfo.IsPersistent)) {
                    CreateMember(typeInfo, provider);
                }
            }
        }

        public static void CreateMember(ITypeInfo typeInfo, FilterProviderBase provider) {
            var attributes = new List<Attribute>
                                 {
                                     new BrowsableAttribute(false),
                                     new MemberDesignTimeVisibilityAttribute(false)
                                 };

            IMemberInfo member = typeInfo.CreateMember(provider.FilterMemberName, provider.FilterMemberType);
            if (provider.FilterMemberIndexed)
                attributes.Add(new IndexedAttribute());
            if (provider.FilterMemberSize != SizeAttribute.DefaultStringMappingFieldSize)
                attributes.Add(new SizeAttribute(provider.FilterMemberSize));
            foreach (Attribute attribute in attributes)
                member.AddAttribute(attribute);
        }

        private void Proxy_DataStoreSelectData(object sender, DataStoreSelectDataEventArgs e) {
            FilterData(e.SelectStatements);
        }

        public void ModifyData(ModificationStatement[] statements) {
            InsertData(statements.OfType<InsertStatement>());
            UpdateData(statements.OfType<UpdateStatement>());
        }

        public void UpdateData(IEnumerable<UpdateStatement> statements) {
            foreach (UpdateStatement statement in statements) {
                if (!IsSystemTable(statement.TableName)) {
                    List<QueryOperand> operands = statement.Operands.OfType<QueryOperand>().ToList();
                    for (int i = 0; i < operands.Count(); i++) {
                        int index = i;
                        FilterProviderBase providerBase = FilterProviderManager.GetFilterProvider(operands[index].ColumnName, StatementContext.Update);
                        if (providerBase != null && !FilterIsShared(statement.TableName, providerBase.Name))
                            statement.Parameters[i].Value = GetModifyFilterValue(providerBase);
                    }
                }

            }
        }

        object GetModifyFilterValue(FilterProviderBase providerBase) {
            return providerBase.FilterValue is IList
                       ? ((IList)providerBase.FilterValue).OfType<object>().FirstOrDefault()
                       : providerBase.FilterValue;
        }


        public void InsertData(IEnumerable<InsertStatement> statements) {
            foreach (InsertStatement statement in statements) {
                if (!IsSystemTable(statement.TableName)) {
                    List<QueryOperand> operands = statement.Operands.OfType<QueryOperand>().ToList();
                    for (int i = 0; i < operands.Count(); i++) {
                        FilterProviderBase providerBase =
                            FilterProviderManager.GetFilterProvider(operands[i].ColumnName, StatementContext.Insert);
                        if (providerBase != null && !FilterIsShared(statements, providerBase))
                            statement.Parameters[i].Value = GetModifyFilterValue(providerBase);
                    }
                }
            }

        }

        bool FilterIsShared(IEnumerable<InsertStatement> statements, FilterProviderBase providerBase) {
            return statements.Aggregate(false, (current, insertStatement) => current & FilterIsShared(insertStatement.TableName, providerBase.Name));
        }


        public BaseStatement[] FilterData(SelectStatement[] statements) {
            return statements.Where(statement => !IsSystemTable(statement.TableName)).Select(ApplyCondition).ToArray();
        }

        public SelectStatement ApplyCondition(SelectStatement statement) {
            var extractor = new CriteriaOperatorExtractor();
            extractor.Extract(statement.Condition);

            foreach (FilterProviderBase provider in FilterProviderManager.Providers) {
                FilterProviderBase providerBase = FilterProviderManager.GetFilterProvider(provider.FilterMemberName, StatementContext.Select);
                if (providerBase != null) {
                    IEnumerable<BinaryOperator> binaryOperators = GetBinaryOperators(extractor, providerBase);
                    if (!FilterIsShared(statement.TableName, providerBase.Name) && binaryOperators.Count() == 0) {
                        string nodeAlias = GetNodeAlias(statement, providerBase);
                        ApplyCondition(statement, providerBase, nodeAlias);
                    }
                }
            }
            return statement;
        }

        void ApplyCondition(SelectStatement statement, FilterProviderBase providerBase, string nodeAlias) {
            if (providerBase.FilterValue is IList) {
                CriteriaOperator criteriaOperator = ((IEnumerable)providerBase.FilterValue).Cast<object>().Aggregate<object, CriteriaOperator>(null, (current, value)
                    => current | new QueryOperand(providerBase.FilterMemberName, nodeAlias) == value.ToString());
                criteriaOperator = new GroupOperator(criteriaOperator);
                statement.Condition &= criteriaOperator;
            } else
                statement.Condition &= new QueryOperand(providerBase.FilterMemberName, nodeAlias) == (providerBase.FilterValue == null ? null : providerBase.FilterValue.ToString());
        }

        IEnumerable<BinaryOperator> GetBinaryOperators(CriteriaOperatorExtractor extractor, FilterProviderBase providerBase) {
            return extractor.BinaryOperators.Where(
                                                      @operator =>
                                                      @operator.RightOperand is OperandValue &&
                                                      ReferenceEquals(((OperandValue)@operator.RightOperand).Value, providerBase.FilterMemberName));
        }

        string GetNodeAlias(SelectStatement statement, FilterProviderBase providerBase) {
            return statement.Operands.OfType<QueryOperand>().Where(operand
                => operand.ColumnName == providerBase.FilterMemberName).Select(operand
                    => operand.NodeAlias).FirstOrDefault() ?? GetNodeAlias(statement, providerBase.FilterMemberName);
        }

        string GetNodeAlias(SelectStatement statement, string filterMemberName) {
            if (GetModelClass(statement.TableName).OwnMembers.Where(member => member.Name==filterMemberName).FirstOrDefault()==null) {
                return statement.SubNodes[0].Alias;
            }
            return statement.Alias;
        }


        private bool IsSystemTable(string name) {
            bool ret = false;

            foreach (IModelFilterDataStoreSystemTable systemTable in ((IModelApplicationFilterDataStore)Application.Model).FilterDataStoreSystemTables) {
                if (systemTable.Name == name)
                    ret = true;
            }
            return ret;
        }

        public bool FilterIsShared(string tableName, string providerName) {
            bool ret = false;

            if (_tablesDictionary.ContainsKey(tableName)) {
                IModelClass modelClass = GetModelClass(tableName);
                if (modelClass != null &&((IModelClassDisabledDataStoreFilters)modelClass).DisabledDataStoreFilters.Where(
                        childNode => childNode.Name == providerName).FirstOrDefault() != null) ret = true;
            }
            return ret;
        }

        IModelClass GetModelClass(string tableName) {
            return Application.Model.BOModel[_tablesDictionary[tableName].FullName];
        }
    }
}

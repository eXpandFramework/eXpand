using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using Microsoft.CSharp;
using Xpand.Utils.Linq;

namespace Xpand.Persistent.Base.General {
    public class ObjectToViewMapper {
        readonly TypeHelper _typeHelper = new TypeHelper();

        public Dictionary<ITypeInfo, string> GenerateSql(ISqlGeneratorFormatter sqlGeneratorFormatter, IList<ITypeInfo> typeInfos) {
            var dictionary = new Dictionary<ITypeInfo, string>();
            var dataSetDataStore = new DataSetDataStore();
            var unitOfWork = new UnitOfWork(new SimpleDataLayer(_typeHelper.XPDictionary, dataSetDataStore));
            foreach (var typeInfo in typeInfos) {
                SetupDataStore(dataSetDataStore, unitOfWork, typeInfo);
                var sql = new SelectSqlGenerator(sqlGeneratorFormatter).GenerateSql(dataSetDataStore.SelectStatement).Sql;
                sql = ParseSql(sql);
                dictionary.Add(typeInfo, sql);
            }
            return dictionary;
        }

        string ParseSql(string sql) {
            var indexOf = sql.IndexOf("from", StringComparison.Ordinal);
            var ret = sql.Substring("select ".Length - 1, indexOf);
            var replace = Regex.Replace(ret, @"""([^""\s]*)([^""]*)""", "\"$1\"$2");
            return "select " + replace + sql.Substring(indexOf);
        }

        void SetupDataStore(DataSetDataStore dataSetDataStore, UnitOfWork unitOfWork, ITypeInfo typeInfo) {
            var classType = FindMappedTypeInfo(typeInfo.Name).Type;
            dataSetDataStore.ObjectClassInfo = unitOfWork.Dictionary.GetClassInfo(classType);
            dataSetDataStore.DynamicTypeInfo = typeInfo;
            Activator.CreateInstance(classType, unitOfWork);
            unitOfWork.CommitChanges();
            unitOfWork.GetObjects(dataSetDataStore.ObjectClassInfo, null, new SortingCollection(), 0, 0, false, true);
        }

        ITypeInfo FindMappedTypeInfo(string name) {
            return XafTypesInfo.Instance.PersistentTypes.First(info => {
                var objectToViewAttribute = info.FindAttribute<ObjectToViewAttribute>();
                return objectToViewAttribute != null && objectToViewAttribute.ViewName == name;
            });
        }

        void RegisterReferences(IList<string> references, params  Type[] types) {
            foreach (var type in types) {
                references.Add(type.Assembly.Location);
            }
        }

        public IList<ITypeInfo> BuildTypeInfos() {
            var codeGenerator = new CodeGenerator();
            var persistentTypes = XafTypesInfo.Instance.PersistentTypes;
            var codeInfos = persistentTypes.Select(codeGenerator.GetCode).Where(info => info != null).ToList();
            var references = codeInfos.SelectMany(info => info.References).ToList();
            RegisterReferences(references, typeof(ICommandChannel), typeof(CSharpCodeProvider));
            var source = String.Join(Environment.NewLine, codeInfos.Select(info => info.Source).ToArray());
            TypeGenerator.CreateDll(source, references.ToArray(), null);
            return codeInfos.Select(info => info.ViewTypeInfo).ToList();
        }
    }

    class CodeInfo {
        readonly ITypeInfo _typeInfo;
        readonly string _source;
        readonly List<string> _references;
        readonly string _viewName;

        public CodeInfo(ITypeInfo typeInfo, string source, List<string> references, string viewName) {
            _typeInfo = typeInfo;
            _source = source;
            _references = references;
            _viewName = viewName;
        }

        public ITypeInfo TypeInfo {
            get { return _typeInfo; }
        }

        public string Source {
            get { return _source; }
        }

        public List<string> References {
            get { return _references; }
        }

        public ITypeInfo ViewTypeInfo {
            get { return XafTypesInfo.Instance.FindTypeInfo(ReflectionHelper.GetType(_viewName)); }
        }
    }

    class CodeGenerator {
        readonly TypeHelper _typeHelper = new TypeHelper();
        readonly ReferencesCollector _referencesCollector = new ReferencesCollector();
        readonly List<Type> _usingTypes = new List<Type>();
        string _viewName;
        ITypeInfo _typeInfo;

        public CodeInfo GetCode(ITypeInfo typeInfo) {
            _typeInfo = typeInfo;
            var objectToViewAttribute = _typeInfo.FindAttribute<ObjectToViewAttribute>();
            if (objectToViewAttribute != null) {
                _viewName = objectToViewAttribute.ViewName;
                string classDeclaration = CreateClassDeclaration();
                var classConstructor = CreateClassConstructor();
                string properties = CreateProperties();
                var source = string.Join(Environment.NewLine, new[] { classDeclaration, classConstructor, properties, "}" });
                _referencesCollector.Add(_usingTypes);
                string[] references = _referencesCollector.References.ToArray();
                return new CodeInfo(typeInfo, source, references.ToList(), _viewName);
            }
            return null;
        }

        protected virtual string CreateClassConstructor() {
            return string.Format("public {0}({1} session):base(session){{}}", _viewName, TypeToString(typeof(Session)));
        }

        string CreateProperties() {
            var memberInfos = PersistentNonSystemMemberInfos();
            return string.Join(Environment.NewLine, memberInfos.Select(CreateProperty).ToArray());
        }

        string CreateProperty(IMemberInfo info) {
            return CreatePropertyCore(KeyAttributeCode(info), TableNameAttributeCode(info), GetPropertyType(info), _typeHelper.GetColumnNameCore(info));
        }

        protected virtual string CreatePropertyCore(string keyAttributeCode, string tableNameAttributeCode, string propertyType, string columnName) {
            return string.Format("{3}{2}public {0} {1}{{get;set;}}", propertyType, columnName, keyAttributeCode, tableNameAttributeCode);
        }

        string TableNameAttributeCode(IMemberInfo memberInfo) {
            return memberInfo.MemberTypeInfo.IsDomainComponent && memberInfo.MemberTypeInfo != _typeInfo
                       ? string.Format("[{0}(\"{1}\",\"{3}\")]{2}", TypeToString(typeof(ReferenceInfoAttribute)),
                                       _typeHelper.XPDictionary.GetClassInfo(memberInfo.MemberTypeInfo.Type).TableName,
                                       Environment.NewLine, memberInfo.MemberTypeInfo.Type.FullName)
                       : null;
        }

        string KeyAttributeCode(IMemberInfo info) {
            return info.IsKey ? "[" + TypeToString(typeof(KeyAttribute)) + "(true)]" + Environment.NewLine : null;
        }

        IEnumerable<IMemberInfo> PersistentNonSystemMemberInfos() {
            var systemMembers = new[] { GCRecordField.StaticName, _typeHelper.XPDictionary.GetClassInfo(_typeInfo.Type).OptimisticLockFieldName };
            return _typeInfo.Members.Where(info => info.IsPersistent && !systemMembers.Contains(info.Name));
        }

        string GetPropertyType(IMemberInfo info) {
            return TypeToString(info.MemberTypeInfo.IsPersistent ? _typeHelper.DefaultMember(info.MemberTypeInfo).MemberType : info.MemberTypeInfo.Type);
        }

        protected string TypeToString(Type type) {
            return HelperTypeGenerator.TypeToString(type, _usingTypes, true);
        }

        protected virtual string CreateClassDeclaration() {
            return string.Format("public class {0}:{1}{{", _viewName, TypeToString(typeof(XPLiteObject)));
        }
    }

    class TypeHelper {
        public string GetColumnNameCore(IMemberInfo info) {
            var persistentAttribute = info.FindAttribute<PersistentAttribute>();
            return persistentAttribute != null ? persistentAttribute.MapTo : info.Name;
        }

        public IMemberInfo DefaultMember(ITypeInfo typeInfo) {
            return typeInfo.DefaultMember ?? typeInfo.KeyMember;
        }

        public XPDictionary XPDictionary {
            get { return XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary; }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ObjectToViewAttribute : Attribute {
        readonly string _viewName;

        public ObjectToViewAttribute(string viewName) {
            _viewName = viewName;
        }

        public string ViewName {
            get { return _viewName; }
        }
    }

    class DataSetDataStore : DevExpress.Xpo.DB.DataSetDataStore {
        readonly TypeHelper _typeHelper = new TypeHelper();
        SelectStatement _selectStatement;

        public XPClassInfo ObjectClassInfo { get; set; }

        public ITypeInfo DynamicTypeInfo { get; set; }

        public SelectStatement SelectStatement {
            get { return _selectStatement; }
        }

        public DataSetDataStore()
            : base(new DataSet(), AutoCreateOption.DatabaseAndSchema) {
        }

        public override SelectedData SelectData(params SelectStatement[] selects) {
            var selectStatement = selects.FirstOrDefault(statement => statement.TableName == ObjectClassInfo.TableName);
            if (selectStatement != null) {
                var systemMembers = new[] { GCRecordField.StaticName, ObjectClassInfo.OptimisticLockFieldName, ObjectClassInfo.OptimisticLockFieldInDataLayerName };
                var memberInfos = DynamicTypeInfo.OwnMembers.Where(info => info.IsPublic && !systemMembers.Contains(info.Name));
                var operands = memberInfos.Select(info => CreateOperand(info, selectStatement)).ToList();
                selectStatement.Operands.Clear();
                selectStatement.Operands.AddRange(operands);
                _selectStatement = selectStatement;
            }
            return base.SelectData(selects);
        }

        string GetColumnName(IMemberInfo info, ReferenceInfoAttribute referenceInfoAttribute) {
            var defaultMember = _typeHelper.DefaultMember(referenceInfoAttribute == null ? info.MemberTypeInfo : referenceInfoAttribute.TypeInfo);
            string alias = null;
            if (referenceInfoAttribute != null)
                alias = " AS " + _typeHelper.GetColumnNameCore(info);
            return defaultMember != null ? defaultMember.Name + alias : _typeHelper.GetColumnNameCore(info);
        }

        CriteriaOperator CreateOperand(IMemberInfo memberInfo, SelectStatement selectStatement) {
            var referenceInfoAttribute = memberInfo.FindAttribute<ReferenceInfoAttribute>();
            return new QueryOperand(GetColumnName(memberInfo, referenceInfoAttribute), GetAlias(referenceInfoAttribute, selectStatement));
        }

        string GetAlias(ReferenceInfoAttribute referenceInfoAttribute, SelectStatement selectStatement) {
            if (referenceInfoAttribute != null) {
                var joinNode =
                    selectStatement.SubNodes.GetItems<JoinNode>(node => node.SubNodes).FirstOrDefault(
                        node => node.TableName == referenceInfoAttribute.TableName);
                if (joinNode != null) {
                    return joinNode.Alias;
                }
            }
            return "N0";
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ReferenceInfoAttribute : Attribute {
        readonly string _tableName;
        readonly ITypeInfo _typeInfo;

        public ReferenceInfoAttribute(string tableName, string typeName) {
            _tableName = tableName;
            _typeInfo = XafTypesInfo.Instance.FindTypeInfo(ReflectionHelper.GetType(typeName));
        }

        public ITypeInfo TypeInfo {
            get { return _typeInfo; }
        }

        public string TableName {
            get { return _tableName; }
        }
    }
}

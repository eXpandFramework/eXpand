using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Xpo.Metadata;
using Fasterflect;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.RuntimeMembers.Model;
using Xpand.Utils.Fastreflect;
using Xpand.Xpo.MetaData;

namespace Xpand.Persistent.Base.Xpo.MetaData{
    public interface IXpandModelMemberInfo  {
        void SynchronizeModel(object theObject,IModelNode modelNode);
        void ApplyModel(object theObject,IModelNode modelNode);
        bool IsReadOnly { get;  }
    }

    public interface IModelMemberModelMemberCommon:IModelNode{
        [Category("eXpand.ModelMembers")]
        [TypeConverter(typeof(StringToModelTypeConverter))]
        [Required]
        Type DeclaredType { get; set; }
        [Required]
        [Category("eXpand.ModelMembers")]
        string ModelNodePath { get; set; }
    }

    public class StringToModelTypeConverter : StringToTypeConverterBase {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context){
            var typeInfos = XafTypesInfo.Instance.FindTypeInfo(typeof(IModelNode)).Descendants.Where(info => info.IsInterface).OrderBy(info => info.FullName);
            return new StandardValuesCollection(typeInfos.Select(info => info.Type).ToArray());
        }
    }

    [KeyProperty("Name")]
    [ModelDisplayName("AdditionalMember")]
    public interface IModelMemberModelMemberAdditionalMember:IModelMemberModelMemberCommon{
        [Required(typeof(NonRequiredCalculator))]
        [ReadOnly(true)]
        Type Type { get; set; }
        [Required]
        string Name { get; set; }
    }

    [DomainLogic(typeof(IModelMemberModelMemberAdditionalMember))]
    public class ModelMemberModelMemberAdditionalMemberDomainLogic{
        public static string Get_ModelNodePath(IModelMemberModelMemberAdditionalMember modelMember){
            return modelMember.Name;
        }

        public static Type Get_Type(IModelMemberModelMemberAdditionalMember modelMember) {
            if (modelMember.DeclaredType != null && !string.IsNullOrEmpty(modelMember.ModelNodePath)) {
                var propertyInfo = modelMember.DeclaredType.Property(modelMember.ModelNodePath.Split('.').Last());
                if (propertyInfo != null) return propertyInfo.PropertyType;
            }
            return null;
        }

    }
    
    [ModelDisplayName("ModelMember")]
    public interface IModelMemberModelMember : IModelMemberEx, IModelMemberModelMemberCommon {
        [Required(typeof(NonRequiredCalculator))]
        [Category("eXpand.ModelMembers")]
        [ReadOnly(true)]
        new Type Type { get; set; }
        [Category("eXpand.ModelMembers")]
        IModelMemberModelMembers AdditionalMembers { get; }
        [Category("eXpand.ModelMembers")]
        [ModelReadOnly(typeof(ReadonlyOnTrueCalculator))]
        bool CreateMember { get; set; }
    }

    public class ReadonlyOnTrueCalculator:IModelIsReadOnly{
        public bool IsReadOnly(IModelNode node, string propertyName){
            return node.GetValue<bool>(propertyName);
        }

        public bool IsReadOnly(IModelNode node, IModelNode childNode){
            throw new NotImplementedException();
        }
    }

    public class NonRequiredCalculator:IModelIsRequired{
        public bool IsRequired(IModelNode node, string propertyName){
            return false;
        }
    }

    [ModelNodesGenerator(typeof(ModelMemberModelMembersNodesGenerator))]
    public interface IModelMemberModelMembers : IModelList<IModelMemberModelMemberAdditionalMember>,IModelNode {
         
    }

    public class ModelMemberModelMembersNodesGenerator:ModelNodesGeneratorBase{
        protected override void GenerateNodesCore(ModelNode node){
            
        }
    }

    [DomainLogic(typeof(IModelMemberModelMember))]
    public class ModelMemberModelMemberDomainLogic : ModelMemberExDomainLogicBase<IModelMemberModelMember> {
        public static Type Get_Type(IModelMemberModelMember modelMember){
            if (modelMember.DeclaredType != null && !string.IsNullOrEmpty(modelMember.ModelNodePath)){
                var propertyInfo = modelMember.DeclaredType.Property(modelMember.ModelNodePath.Split('.').Last());
                if (propertyInfo != null) return propertyInfo.PropertyType;
            }
            return null;
        }

        public static string Get_ModelNodePath(IModelMemberModelMember modelMember) {
            return modelMember.Name;
        }

        public static IMemberInfo Get_MemberInfo(IModelMemberModelMember modelMemberCalculated) {
            return GetMemberInfo(modelMemberCalculated,
                (calculated, info) => GetXpandModelMemberInfo(calculated, info),
                modelMember => !string.IsNullOrEmpty(modelMember.ModelNodePath)&&modelMember.DeclaredType!=null&&modelMember.CreateMember);
        }

        private static XpandModelMemberInfo GetXpandModelMemberInfo(IModelMemberModelMember memberModelMember, XPClassInfo info){
            var memberInfoData = new ModelMemberInfoData(info, memberModelMember.Name, memberModelMember.Type, memberModelMember.ModelNodePath, memberModelMember.DeclaredType);
            var additionalDatas = memberModelMember.AdditionalMembers.Select(member => new ModelMemberInfoAdditionalData(member.Name,member.Type,member.DeclaredType)).ToArray();
            return new XpandModelMemberInfo(memberInfoData,false,additionalDatas);
        }
    }

    public class ModelMemberInfoAdditionalData{
        private readonly Type _memberType;
        private readonly Type _modelOwner;

        public ModelMemberInfoAdditionalData(string name, Type memberType,Type modelOwner) {
            _memberType = memberType;
            _modelOwner = modelOwner;
            Name = name;
        }

        public string Name { get; set; }
        public Type MemberType {
            get { return _memberType; }
        }
        public Type ModelOwner {
            get { return _modelOwner; }
        }

    }
    public class ModelMemberInfoData:ModelMemberInfoAdditionalData{
        private readonly XPClassInfo _owner;
        private readonly string _modelNodePath;
        

        public ModelMemberInfoData(XPClassInfo owner,string name, Type memberType, string modelNodePath,Type modelOwner)
            : base(name, memberType,modelOwner){
            _owner = owner;
            _modelNodePath = modelNodePath;
        }

        public ModelMemberInfoData(XPClassInfo owner,Type memberType, string modelNodePath,Type modelOwner)
            : this(owner,modelNodePath.Contains(".") ? modelNodePath.Split('.').Last() : modelNodePath, memberType, modelNodePath,modelOwner) {
            _modelNodePath = modelNodePath;
        }


        public XPClassInfo Owner{
            get { return _owner; }
        }

        public string ModelNodePath{
            get { return _modelNodePath; }
        }
    }
    public sealed class XpandModelMemberInfo : XpandCustomMemberInfo, IXpandModelMemberInfo {
        
        public void SynchronizeModel(object theObject,IModelNode modelNode){
            var theValue = GetValue(theObject);
            if (theValue != null){
                var valueType = ((ModelNode) modelNode).GetValueType(_modelNodePath);
                modelNode.SetValue(_modelNodePath, valueType,theValue);
            }
        }

        public void ApplyModel(object theObject,IModelNode modelNode){
            var value = modelNode.GetValue(_modelNodePath);
            var typeInfo = value as ITypeInfo;
            if (typeInfo!=null)
                value = typeInfo.Type;
            SetValue(theObject, value);
        }

        public void SetModelNodeValue(object theObject, IModelNode modelNode){
            _modelNodeIdMemberInfo.SetValue(theObject, modelNode);
        }
        private readonly string _modelNodePath;
        private readonly XpandCustomMemberInfo _modelNodeIdMemberInfo;

        public XpandModelMemberInfo(ModelMemberInfoData memberInfoData,bool isReadOnly,  params ModelMemberInfoAdditionalData[] additionalMembers)
            : base(memberInfoData.Owner, memberInfoData.Name, memberInfoData.MemberType, null, true, false,isReadOnly){
            _modelNodePath = memberInfoData.ModelNodePath;
            foreach (var attribute in memberInfoData.ModelOwner.PropertyX(memberInfoData.ModelNodePath.Split('.').Last()).Attributes()) {
                AddAttribute(attribute);
            }

            foreach (var additionalMember in additionalMembers){
                var memberType = typeof(ITypeInfo).IsAssignableFrom(additionalMember.MemberType)?typeof(Type):additionalMember.MemberType;
                var modelMemberInfoData = new ModelMemberInfoData(Owner, additionalMember.Name, memberType, additionalMember.Name, additionalMember.ModelOwner);
                new XpandModelMemberInfo(modelMemberInfoData,true);
            }
            _modelNodeIdMemberInfo = Owner.FindMember("ModelNode") as XpandCustomMemberInfo ?? new XpandCustomMemberInfo(Owner, "ModelNode", typeof(IModelNode), null, true, true);
        }

        public string ModelNodePath{
            get { return _modelNodePath; }
        }
    }

    public class XpandModelMemberInfoController:ViewController<DetailView>{
        private List<IXpandModelMemberInfo> _memberInfos;

        protected override void OnActivated(){
            base.OnActivated();
            _memberInfos = View.Model.ModelClass.GetXPClassInfo().Members.OfType<IXpandModelMemberInfo>().ToList();
            if (_memberInfos.Any()){
                foreach (var memberInfo in _memberInfos) {
                    memberInfo.ApplyModel(View.CurrentObject, ((Link)View.Tag).ListView.Model);
                }
            }
        }


        public void SynchronizeModel(IModelNode modelNode, object theObject){
            var memberInfo = _memberInfos.FirstOrDefault();
            if (memberInfo != null){
                foreach (var modelMemberInfo in _memberInfos.Where(info => !info.IsReadOnly)){
                    modelMemberInfo.SynchronizeModel(theObject, modelNode);
                }
            }
        }
    }
}
using System;
using System.ComponentModel;
using System.Globalization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Model.RuntimeMembers.Collections;
using Xpand.ExpressApp.Model.VisibilityCalculators;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Xpo.MetaData;

namespace Xpand.ExpressApp.Model.RuntimeMembers {
    [ModelAbstractClass]
    public interface IModelMemberEx : IModelMember {
        [Category(ModelMemberExDomainLogic.AttributesCategory)]
        [Description("Specifies the current property type.")]
        [TypeConverter(typeof(XpandStringToTypeConverterExtended))]
        [ModelBrowsable(typeof(ModelMemberExTypeVisibilityCalculator))]
        [Required]
        [ModelReadOnly(typeof(AlwaysEditableVisibilityCalculator))]
        new Type Type { get; set; }
        [ModelBrowsable(typeof(DesignerOnlyCalculatorForExMembers))]
        new bool IsCustom { get; set; }
        [Browsable(false)]
        object Tag { get; set; }
        [Browsable(false)]
        bool CreatedAtDesignTime { get; set; }
    }
    [ModelDisplayName("NonPersistent")]
    [ModelPersistentName("RuntimeNonPersistentMember")]
    public interface IModelMemberNonPersistent : IModelMemberEx {
    }

    public class XpandStringToTypeConverterExtended : DevExpress.ExpressApp.Model.Core.StringToTypeConverterExtended {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value != null) {
                ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(Type.GetType(value.ToString()));
                return typeInfo != null ? typeInfo.Type : base.ConvertFrom(context,culture, value);
            }
            return null;
        }
    }

    public class DesignerOnlyCalculatorForExMembers : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return !(node is IModelMemberEx);
        }
    }

    public class ModelMemberExTypeVisibilityCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return !(node is IModelMemberOrphanedColection) || propertyName != "Type";
        }
    }

    public abstract class ModelMemberExDomainLogicBase<TModelMember> where TModelMember:IModelMemberEx {

        protected static IMemberInfo GetMemberInfo(TModelMember modelMemberEx, Action<TModelMember, XPClassInfo> CreateXpandCustomMemberInfo, Func<TModelMember, bool> validState) {
            XPClassInfo classInfo = FindXPClassInfo(modelMemberEx);
            var customMemberInfo = classInfo.FindMember(modelMemberEx.Name) as XPCustomMemberInfo;
            if (ValidState(modelMemberEx, customMemberInfo,validState)) {
                var xpClassInfo = FindXPClassInfo(modelMemberEx);
                var xpandCustomMemberInfo = (XpandCustomMemberInfo)xpClassInfo.FindMember(modelMemberEx.Name);
                if (xpandCustomMemberInfo == null) {
                    modelMemberEx.CreatedAtDesignTime = !InterfaceBuilder.RuntimeMode;
                    CreateXpandCustomMemberInfo.Invoke(modelMemberEx, xpClassInfo);
                    var typesInfo = ((BaseInfo)modelMemberEx.ModelClass.TypeInfo).Store;
                    typesInfo.RefreshInfo(xpClassInfo.ClassType);
                }
            }
            return modelMemberEx.ModelClass.TypeInfo.FindMember(modelMemberEx.Name);
        }

        protected static bool CheckTag(TModelMember modelMemberEx) {
            if (Equals(true, modelMemberEx.Tag)) {
                modelMemberEx.Tag = null;
                return true;
            }
            return false;
        }


        protected static bool ValidState(TModelMember modelMemberEx, XPCustomMemberInfo memberInfo,Func<TModelMember,bool> validState) {
            if (CheckTag(modelMemberEx)) return false;
            if (memberInfo == null && !String.IsNullOrEmpty(modelMemberEx.Name)) {
                modelMemberEx.Tag = true;
                if (modelMemberEx.Type != null && (validState.Invoke(modelMemberEx))) {
                    modelMemberEx.Tag = null;
                    return true;
                }
            }
            return false;
        }

        protected static XPClassInfo FindDCXPClassInfo(TypeInfo typeInfo) {
            var xpoTypeInfoSource = XpandModuleBase.XpoTypeInfoSource;
            var generatedEntityType = xpoTypeInfoSource.GetGeneratedEntityType(typeInfo.Type);
            return generatedEntityType == null ? null : xpoTypeInfoSource.XPDictionary.GetClassInfo(generatedEntityType);
        }

        protected static XPClassInfo FindXPClassInfo(TModelMember modelMemberEx) {
            var typeInfo = (TypeInfo)modelMemberEx.ModelClass.TypeInfo;
            return typeInfo.IsInterface ? FindDCXPClassInfo(typeInfo) : XpandModuleBase.Dictiorary.GetClassInfo(typeInfo.Type);
        }
    }

    [DomainLogic(typeof(IModelMemberEx))]
    public class ModelMemberExDomainLogic {
        public const string AttributesCategory = "eXpand.RuntimeMembers";
        public static string Get_Caption(IModelMemberEx modelRuntimeMember) {
            return modelRuntimeMember.MemberInfo != null ? GetMemberCaption(modelRuntimeMember.MemberInfo) : string.Empty;
        }

        static string GetMemberCaption(IMemberInfo memberInfo) {
            string memberCaption = null;

            var displayNameAttr =
                memberInfo.FindAttribute<DisplayNameAttribute>();
            if (displayNameAttr != null) {
                memberCaption = displayNameAttr.DisplayName;
            }

            if (string.IsNullOrEmpty(memberCaption)) {
                var attribute = memberInfo.FindAttribute<XafDisplayNameAttribute>();
                if (attribute != null) {
                    memberCaption = attribute.DisplayName;
                }
            }
            if (string.IsNullOrEmpty(memberCaption)) {
                memberCaption = memberInfo.DisplayName;
            }
            if (string.IsNullOrEmpty(memberCaption)) {
                memberCaption = CaptionHelper.ConvertCompoundName(memberInfo.Name);
            }
            return memberCaption;
        }

    }

}

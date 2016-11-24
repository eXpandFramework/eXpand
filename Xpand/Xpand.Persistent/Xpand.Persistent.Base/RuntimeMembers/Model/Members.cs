using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model.RequiredCalculators;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.RuntimeMembers.Model.Collections;
using Xpand.Xpo.MetaData;

namespace Xpand.Persistent.Base.RuntimeMembers.Model {
    [ModelAbstractClass]
    public interface IModelClassEx:IModelClass {
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new ITypeInfo TypeInfo { get; }
    }
    public class ModelClassExDomainLogic {
        public static ITypeInfo  Get_TypeInfo(IModelClass modelClass) {
            var typesInfo = XafTypesInfo.Instance;
            if (typesInfo != null) return typesInfo.FindTypeInfo(modelClass.Name);
            return XafTypesInfo.Instance.FindTypeInfo(modelClass.Name);
        }
    }
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
        bool? CreatedAtDesignTime { get; set; }

        [ModelBrowsable(typeof(NotVisibileCalculator))]
        [Required(typeof(NotRequiredCalculator))]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ExpressionModelEditorControl, DevExpress.ExpressApp.Win.v13.2, Version=13.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a", typeof(UITypeEditor))]
        new string Expression { get; set; }
        
        [DefaultValue(false)]
        new bool IsCalculated { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class ModelMemberExMemberInfoAttribute : Attribute {
    }

    [ModelDisplayName("NonPersistent")]
    [ModelPersistentName("RuntimeNonPersistentMember")]
    public interface IModelMemberNonPersistent : IModelMemberEx {
    }

    public class XpandStringToTypeConverterExtended : StringToTypeConverterExtended {
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
            return !(node is IModelMemberColection) || propertyName != "Type";
        }
    }

    public abstract class ModelMemberExDomainLogicBase<TModelMember> where TModelMember:IModelMemberEx {

        protected static IMemberInfo GetMemberInfo(TModelMember modelMemberEx, Action<TModelMember, XPClassInfo> createXpandCustomMemberInfo, Func<TModelMember, bool> validState) {
            var customMemberInfo = FindXPClassInfo(modelMemberEx).FindMember(modelMemberEx.Name) as XPCustomMemberInfo;
            if (ValidState(modelMemberEx, customMemberInfo,validState)) {
                var xpClassInfo = FindXPClassInfo(modelMemberEx);
                var xpandCustomMemberInfo = (XpandCustomMemberInfo)xpClassInfo.FindMember(modelMemberEx.Name);
                if (xpandCustomMemberInfo == null) {
                    if (!modelMemberEx.CreatedAtDesignTime.HasValue)
                        modelMemberEx.CreatedAtDesignTime = CreatedAtDesignTime(modelMemberEx);
                    createXpandCustomMemberInfo(modelMemberEx, xpClassInfo);
                    xpClassInfo.FindMember(modelMemberEx.Name).AddAttribute(new ModelMemberExMemberInfoAttribute());
                    var typesInfo = ((BaseInfo)modelMemberEx.ModelClass.TypeInfo).Store;
                    typesInfo.RefreshInfo(modelMemberEx.ModelClass.TypeInfo);
                }
            }
            return modelMemberEx.ModelClass.TypeInfo.FindMember(modelMemberEx.Name);
        }

        private static bool CreatedAtDesignTime(TModelMember modelMemberEx){
            var createdAtDesignTime = !InterfaceBuilder.RuntimeMode;
            return createdAtDesignTime || modelMemberEx.Application.Id() == ModelApplicationLayerIds.Generator;
        }

        protected static bool CheckTag(TModelMember modelMemberEx) {
            if (Equals(true, modelMemberEx.Tag)) {
                modelMemberEx.ClearValue(member => member.Tag);
                return true;
            }
            return false;
        }

        protected static bool ValidState(TModelMember modelMemberEx, XPCustomMemberInfo memberInfo,Func<TModelMember,bool> validState) {
            if (CheckTag(modelMemberEx)) return false;
            if (memberInfo == null && !String.IsNullOrEmpty(modelMemberEx.Name)) {
                modelMemberEx.Tag = true;
                if (modelMemberEx.Type != null && (validState.Invoke(modelMemberEx))) {
                    modelMemberEx.ClearValue(member => member.Tag);
                    return true;
                }
            }
            return false;
        }

        protected static XPClassInfo FindXPClassInfo(TModelMember modelMemberEx) {
            var typeInfo = (TypeInfo)modelMemberEx.ModelClass.TypeInfo;
            return typeInfo.IsInterface ? typeInfo.FindDCXPClassInfo() : XpandModuleBase.Dictiorary.GetClassInfo(typeInfo.Type);
        }
    }

    [DomainLogic(typeof(IModelMemberEx))]
    public class ModelMemberExDomainLogic {
        public const string AttributesCategory = "eXpand.RuntimeMembers";

        public static void Set_IsCalculated(IModelMemberEx modelRuntimeMember,bool value) {
            
        }

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

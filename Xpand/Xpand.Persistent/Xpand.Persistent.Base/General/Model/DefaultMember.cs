using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General.Model{
    [ModelAbstractClass]
    public interface IModelClassDefaultCriteria : IModelClass{
        [Required]
        [Category("eXpand")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + AssemblyInfo.VSuffix, typeof(UITypeEditor))]
        [CriteriaOptions("TypeInfo")]
        string DefaultCriteria { get; set; }

    }

    [DomainLogic(typeof (IModelClassDefaultCriteria))]
    public static class DefaultCriteriaDomainLogic{
        public static string Get_DefaultCriteria(IModelClassDefaultCriteria modelClassDefaultCriteria){
            var defaultMember = GetDefaultMember(modelClassDefaultCriteria.TypeInfo);
            return new BinaryOperator(defaultMember.Name, "?").ToString();
        }

        public static IMemberInfo GetDefaultMember(this ITypeInfo typeInfo) {
            return ((typeInfo.DefaultMember ?? GetDefaultMember(typeInfo, info => info.MemberType == typeof (string))) ??
                    GetDefaultMember(typeInfo, info => info.IsPublic && info.IsPersistent)) ??
                   GetDefaultMember(typeInfo, null);
        }

        private static IMemberInfo GetDefaultMember(ITypeInfo typeInfo, Func<IMemberInfo, bool> condition){
            if (condition == null)
                condition = info => true;
            return typeInfo.OwnMembers.FirstOrDefault(condition) ?? typeInfo.Members.FirstOrDefault(condition);
        }
    }
}
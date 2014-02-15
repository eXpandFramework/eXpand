using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.Xpo.MetaData;

namespace Xpand.Persistent.Base.RuntimeMembers.Model {
    [ModelDisplayName("Calculated")]
    [ModelPersistentName("RuntimeCalculatedMember")]
    public interface IModelMemberCalculated : IModelMemberNonPersistent {
        [Required]
        [Category(ModelMemberExDomainLogic.AttributesCategory)]
        [Description("Using an expression here it will force the creation of a calculated property insted of a normal one")]
        [CriteriaOptions("ModelClass.TypeInfo")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ExpressionModelEditorControl, DevExpress.ExpressApp.Win" +XafApplication.CurrentVersion+AssemblyInfo.FullAssemblyVersionExtension , typeof(UITypeEditor))]
        string AliasExpression { get; set; }
    }

    [DomainLogic(typeof(IModelMemberCalculated))]
    public class ModelMemberCalculatedDomainLogic:ModelMemberExDomainLogicBase<IModelMemberCalculated> {
        public static IMemberInfo Get_MemberInfo(IModelMemberCalculated modelMemberCalculated) {
            return GetMemberInfo(modelMemberCalculated, 
                (calculated, info) => new XpandCalcMemberInfo(info, calculated.Name, calculated.Type, calculated.AliasExpression),
                calculated => !string.IsNullOrEmpty(calculated.AliasExpression));            
        }
    }



}

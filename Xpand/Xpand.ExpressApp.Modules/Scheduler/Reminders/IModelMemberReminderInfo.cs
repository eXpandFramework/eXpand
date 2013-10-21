using System;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Xpand.Persistent.Base.RuntimeMembers.Model;
using Xpand.Xpo;
using Xpand.Xpo.MetaData;

namespace Xpand.ExpressApp.Scheduler.Reminders {
    [ModelDisplayName("ReminderInfo")]
    public interface IModelMemberReminderInfo:IModelMemberPersistent {
        [Category(ModelMemberExDomainLogic.AttributesCategory)]
        [Description("Specifies the current property type.")]
        [TypeConverter(typeof(XpandStringToTypeConverterExtended))]
        [Required]
        [ModelReadOnly(typeof(ModelReadOnlyCalculator))]
        [ModelBrowsable(typeof(AlwaysVisibleCalculator))]
        new Type Type { get; set; }
        [CriteriaOptions("ModelClass.TypeInfo")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win"+AssemblyInfo.VSuffix, typeof(UITypeEditor))]
        [Category(XpandSchedulerModule.XpandScheduler)]
        string ReminderCriteria { get; set; }
    }

    [DomainLogic(typeof(IModelMemberReminderInfo))]
    public class ModelMemberReminderInfoDomainLogic : ModelMemberExDomainLogicBase<IModelMemberReminderInfo> {
        public static Type Get_Type(IModelMemberReminderInfo memberReminderInfo) {
            return typeof (ReminderInfo);
        }

        public static IMemberInfo Get_MemberInfo(IModelMemberReminderInfo modelRuntimeMember) {
            return GetMemberInfo(modelRuntimeMember,
                (persistent, info) => CreateCustomMember(info, persistent),
                persistent => true);
        }

        static XpandCustomMemberInfo CreateCustomMember(XPClassInfo info, IModelMemberReminderInfo persistent) {
            var customMemberInfo = info.CreateCustomMember(persistent.Name, persistent.Type, false);
            ModifyModel(persistent, customMemberInfo);
            return customMemberInfo;
        }

        public static void ModifyModel(IModelMemberReminderInfo modelMemberReminderInfo, XPMemberInfo customMemberInfo) {
            var modelClass = modelMemberReminderInfo.ModelClass;
            var modelClassCreateExpandAbleMembers = modelClass as IModelClassCreateExpandAbleMembers;
            if (modelClassCreateExpandAbleMembers != null)
                modelClassCreateExpandAbleMembers.CreateExpandAbleMembers = true;

            if (!customMemberInfo.HasAttribute(typeof(ExpandObjectMembersAttribute))) {
                customMemberInfo.AddAttribute(new ExpandObjectMembersAttribute(ExpandObjectMembers.Always));
            }
        }
    }

}

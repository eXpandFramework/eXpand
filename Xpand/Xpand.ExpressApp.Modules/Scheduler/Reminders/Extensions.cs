using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base.General;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Scheduler.Reminders {
    static class Extensions {
        public static ReminderInfo GetReminderInfoMemberValue(this IEvent @event) {
            var modelMemberReminderInfo = ModelMemberReminderInfo(@event);
            return (ReminderInfo) modelMemberReminderInfo.MemberInfo.GetValue(@event);
        }

        public static IModelMemberReminderInfo ModelMemberReminderInfo(this IEvent @event) {
            var application = ApplicationHelper.Instance.Application;
            var typeInfo = application.TypesInfo.FindTypeInfo(@event.GetType());
            var modelBoModel = application.Model.BOModel;
            return ModelMemberReminderInfo(typeInfo, modelBoModel);
        }

        public static IModelMemberReminderInfo ModelMemberReminderInfo(this ITypeInfo typeInfo, IModelBOModel modelBoModel) {
            var modelClass = modelBoModel.GetClass(typeInfo.Type);
            return modelClass.AllMembers.OfType<IModelMemberReminderInfo>().FirstOrDefault();
        }
    }
}

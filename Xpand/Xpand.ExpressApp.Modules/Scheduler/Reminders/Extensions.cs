using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base.General;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Scheduler.Reminders {
    public static class Extensions {
        public static void CreateReminderInfoMember(this IEvent @event,IObjectSpace objectSpace){
            var modelMemberReminderInfo = ModelMemberReminderInfo(@event);
            var memberInfo = modelMemberReminderInfo.MemberInfo;
            if (memberInfo.GetValue(@event)==null)
                memberInfo.SetValue(@event, objectSpace.CreateObject(memberInfo.MemberType));
        }

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

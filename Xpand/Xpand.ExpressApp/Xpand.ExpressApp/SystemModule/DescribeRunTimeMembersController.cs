using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelClassDescribeRunTimeMembers : IModelNode {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [Description("Adds a TypeDesriptionProvider to describe runtime members")]
        bool DescribeRunTimeMembers { get; set; }
    }

    public class DescribeRunTimeMembersController : WindowController, IModelExtender {
        public DescribeRunTimeMembersController() {
            TargetWindowType = WindowType.Main;
        }

        protected override void OnActivated() {
            base.OnActivated();
            IEnumerable<IModelClass> classInfoNodeWrappers = Application.Model.BOModel.Cast<IModelClassDescribeRunTimeMembers>().Where(
                    wrapper => wrapper.DescribeRunTimeMembers).Cast<IModelClass>();
            foreach (var classInfoNodeWrapper in classInfoNodeWrappers) {
                TypeDescriptionProvider typeDescriptionProvider = TypeDescriptor.GetProvider(classInfoNodeWrapper.TypeInfo.Type);
                var membersTypeDescriptionProvider = new RuntimeMembersTypeDescriptionProvider(typeDescriptionProvider);
                TypeDescriptor.AddProvider(membersTypeDescriptionProvider, classInfoNodeWrapper.TypeInfo.Type);
            }
        }


        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelClass, IModelClassDescribeRunTimeMembers>();
        }
    }
}

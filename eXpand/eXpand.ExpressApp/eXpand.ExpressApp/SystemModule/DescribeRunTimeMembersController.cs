using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.SystemModule
{
    public interface IModelClassDescribeRunTimeMembers : IModelNode
    {
        bool DescribeRunTimeMembers { get; set; }
    }

    public partial class DescribeRunTimeMembersController : WindowController
    {
        public DescribeRunTimeMembersController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetWindowType=WindowType.Main;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            IEnumerable<IModelClass> classInfoNodeWrappers = Application.Model.BOModel.Cast<IModelClassDescribeRunTimeMembers>().Where(
                    wrapper => wrapper.DescribeRunTimeMembers).Cast<IModelClass>();
            foreach (var classInfoNodeWrapper in classInfoNodeWrappers) {
                TypeDescriptionProvider typeDescriptionProvider = TypeDescriptor.GetProvider(classInfoNodeWrapper.TypeInfo.Type);
                var membersTypeDescriptionProvider = new RuntimeMembersTypeDescriptionProvider(typeDescriptionProvider);
                TypeDescriptor.AddProvider(membersTypeDescriptionProvider, classInfoNodeWrapper.TypeInfo.Type);
            }
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelClass, IModelClassDescribeRunTimeMembers>();
        }
    }
}

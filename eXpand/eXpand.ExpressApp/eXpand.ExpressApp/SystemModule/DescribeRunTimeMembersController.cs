using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using System.Linq;

namespace eXpand.ExpressApp.SystemModule
{
    public partial class DescribeRunTimeMembersController : WindowController
    {
        public const string DescribeRunTimeMembersAttributeName = "DescribeRunTimeMembers";
        public DescribeRunTimeMembersController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetWindowType=WindowType.Main;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            IEnumerable<ClassInfoNodeWrapper> classInfoNodeWrappers =
                new ApplicationNodeWrapper(Application.Model).BOModel.Classes.Where(
                    wrapper => wrapper.Node.GetAttributeBoolValue(DescribeRunTimeMembersAttributeName));
            foreach (var classInfoNodeWrapper in classInfoNodeWrappers) {
                TypeDescriptionProvider typeDescriptionProvider = TypeDescriptor.GetProvider(classInfoNodeWrapper.ClassTypeInfo.Type);
                var membersTypeDescriptionProvider = new RuntimeMembersTypeDescriptionProvider(typeDescriptionProvider);
                TypeDescriptor.AddProvider(membersTypeDescriptionProvider, classInfoNodeWrapper.ClassTypeInfo.Type);
            }
        }

        public override Schema GetSchema()
        {
            var helper = new SchemaBuilder();
            DictionaryNode dictionaryNode = helper.Inject(@"<Attribute Name=""" + DescribeRunTimeMembersAttributeName + @""" Choice=""True,False""/>", ModelElement.Class);
            return new Schema(dictionaryNode);
        }
    }
}

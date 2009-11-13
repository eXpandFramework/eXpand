using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Controllers
{
    public abstract partial class UpdateModelViewController : ViewController
    {
        public const string AdditionalViewControls = "AdditionalViewControls";

        protected UpdateModelViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        public override void UpdateModel(Dictionary dictionary)
        {
            base.UpdateModel(dictionary);

            foreach (ClassInfoNodeWrapper classInfoNodeWrapper in new ApplicationNodeWrapper(dictionary).BOModel.Classes)
            {
                DictionaryNode childNode = classInfoNodeWrapper.Node.AddChildNode(AdditionalViewControls);
                var attribute = classInfoNodeWrapper.ClassTypeInfo.FindAttribute<AdditionalViewControlsAttribute>();
                if (attribute!= null)
                {
                    childNode.SetAttribute("Message",attribute.Message);
                    childNode.SetAttribute("MessagePropertyName", attribute.MessagePropertyName);
                    childNode.SetAttribute("DecoratorType", attribute.DecoratorType!= null?attribute.DecoratorType.FullName:GetDefaultDecoratorType().AssemblyQualifiedName);
                    childNode.SetAttribute("ControlType", attribute.ControlType!= null? attribute.ControlType.FullName:GetDefaultControlType().AssemblyQualifiedName);
                    childNode.SetAttribute("ViewType", attribute.TargetViewType.ToString());
                    childNode.SetAttribute("AdditionalViewControlsProviderPosition", attribute.AdditionalViewControlsProviderPosition.ToString());
                }
                else
                {
                    childNode.SetAttribute("DecoratorType", GetDefaultDecoratorType().AssemblyQualifiedName);
                    childNode.SetAttribute("ControlType", GetDefaultControlType().AssemblyQualifiedName);
                }
            }

        }

        protected abstract Type GetDefaultControlType();

        protected abstract Type GetDefaultDecoratorType();

        public override Schema GetSchema()
        {
            return new Schema(new DictionaryXmlReader().ReadFromString(
                @"<Element Name=""Application"">
					<Element Name=""BOModel"">
						<Element Name=""Class"">
						    <Element Name=""" + AdditionalViewControls + @""" Multiple=""False"">
								    <Attribute Name=""Message"" IsLocalized=""True""/>
								    <Attribute Name=""DecoratorType""/>
								    <Attribute Name=""ControlType""/>
								    <Attribute Name=""ViewType"" Choice=""{" + typeof(ViewType).FullName + @"}""/>
								    <Attribute Name=""AdditionalViewControlsProviderPosition"" Choice=""{" + typeof(AdditionalViewControlsProviderPosition).FullName + @"}""/>
								    <Attribute Name=""MessagePropertyName"" RefNodeName=""{eXpand.ExpressApp.Core.DictionaryHelpers.RefNodeStringPropertyProvider}ClassName=..\@Name""/>
						    </Element>
                        </Element>
					</Element>
				</Element>"));
        }
    }
}

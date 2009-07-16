using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.ConditionalEditorState.Core;

namespace DevExpress.ExpressApp.ConditionalEditorState {
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxItem(true)]
    [DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
    [ToolboxBitmap(typeof(ConditionalEditorStateModuleBase), "Resources.ConditionalEditorStateModuleBase.ico")]
    [Description("Provides the capability to customize the view's editors against business rules in XAF applications.")]
    public sealed partial class ConditionalEditorStateModuleBase : ModuleBase {
        public ConditionalEditorStateModuleBase() {
            InitializeComponent();
        }
        public override Schema GetSchema() {
            return new Schema(new DictionaryXmlReader().ReadFromString(
                @"<Element Name=""Application"">
					<Element Name=""BOModel"">
						<Element Name=""Class"">
							<Element Name=""" + ConditionalEditorStateNodeWrapper.NodeName + @""">
								<Element Name=""" + EditorStateRuleNodeWrapper.NodeName + @""" KeyAttribute=""ID"" Multiple=""True"">
									<Attribute Name=""ID"" Required=""True"" />
									<Attribute Name=""Properties"" Required=""True"" />
									<Attribute Name=""EditorState"" Required=""True"" Choice=""{DevExpress.ExpressApp.ConditionalEditorState.EditorState}""/>
                                    <Attribute Name=""Criteria"" />
                                    <Attribute Name=""ViewType"" Required=""True"" Choice=""{DevExpress.ExpressApp.ViewType}""/>
                                    <Attribute Name=""Description""/>
								</Element>
							</Element>
						</Element>
					</Element>
				</Element>"));
        }
        public override void UpdateModel(Dictionary model) {
            base.UpdateModel(model);
            ApplicationNodeWrapper applicatioNodeWrapper = new ApplicationNodeWrapper(model);
            foreach (ClassInfoNodeWrapper clw in applicatioNodeWrapper.BOModel.Classes) {
                DictionaryNode conditionalEditorStateNode = clw.Node.FindChildNode(ConditionalEditorStateNodeWrapper.NodeName);
                if (conditionalEditorStateNode == null) {
                    conditionalEditorStateNode = clw.Node.AddChildNode(ConditionalEditorStateNodeWrapper.NodeName);
                    ConditionalEditorStateNodeWrapper conditionalEditorStateNodeWrapper = new ConditionalEditorStateNodeWrapper(conditionalEditorStateNode);
                    foreach (EditorStateRuleAttribute attribute in clw.ClassTypeInfo.FindAttributes<EditorStateRuleAttribute>()) {
                        conditionalEditorStateNodeWrapper.AddRule(attribute.ID, attribute.Properties, attribute.EditorState, attribute.Criteria, attribute.ViewType, attribute.Description);
                    }
                }
            }
        }
        public override ICollection<Type> GetXafResourceLocalizerTypes() {
            ICollection<Type> result = new List<Type>();
            result.Add(typeof(EditorStateLocalizer));
            return result;
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.SetupComplete += OnApplicationSetupComplete;
        }
        private void OnApplicationSetupComplete(object sender, EventArgs e) {
            EditorStateRuleManager.ClearRulesFromModel();
            EditorStateRuleManager.FillRulesFromModel(((XafApplication)sender).Model);
        }
    }
}
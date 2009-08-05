//-----------------------------------------------------------------------
// <copyright file="WizardUIWindowsFormsModule.cs" created="03.06.2009" company="VenDoc Software GmbH">
//     Copyright (c) VenDoc Software GmbH. All rights reserved.
// </copyright>
// <author>Martin Praxmarer</author>
//-----------------------------------------------------------------------

namespace eXpand.ExpressApp.WizardUI.Win
{
    using System.ComponentModel;
    using DevExpress.ExpressApp;
    using DevExpress.ExpressApp.RibbonUI.Win;
    using DevExpress.ExpressApp.Templates;
    using eXpand.ExpressApp.WizardUI.Win.Templates;

    /// <summary>
    /// Contains an RibbonDetailView Template with an Wizard Control on it
    /// </summary>
    [ToolboxItem(true)]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxItemFilter("Xaf.Platform.Win")]
    [Description("Contains an RibbonDetailView Template with an Wizard Control on it.")]
    public sealed class WizardUIWindowsFormsModule : ModuleBase
    {
        #region Methods

        /// <summary>
        /// Initializes the Module
        /// </summary>
        /// <param name="application">XafApplication Object</param>
        public override void Setup(XafApplication application)
        {
            base.Setup(application);

            application.ShowViewStrategy = new WizardShowViewStrategy(application);
            application.CreateCustomTemplate += this.Application_CreateCustomTemplate;
        }

        /// <summary>
        /// Returns the Schema extension which is combined with the entire Schema when loading the Application Model
        /// </summary>
        /// <returns>The Schema object that represents the Schema extension to be added to the application's entire Schema</returns>
        public override Schema GetSchema()
        {
            const string WizardSchema =
                @"<Element Name=""Application"">
	                <Element Name=""Views"">
		                <Element Name=""DetailView"">
			                <Element Name=""Wizard"" IsNewNode=""True"">
				                <Attribute Name=""ShowInWizard"" Choice=""True,False"" IsNewNode=""True""/>
				                <Element Name=""WizardPage"" KeyAttribute=""ID"" DisplayAttribute=""Caption"" Multiple=""True""  IsNewNode=""True"">
					                <Attribute Name=""ID"" Required=""True"" IsNewNode=""True""/>
					                <Attribute Name=""Caption"" IsLocalized=""True"" IsNewNode=""True""/>
					                <Attribute Name=""ViewID"" Required=""True"" RefNodeName=""{DevExpress.ExpressApp.Core.DictionaryHelpers.ViewIdRefNodeProvider}ClassName=..\..\@ClassName;ViewType=DetailView;IncludeBaseClasses=True"" IsNewNode=""True""/>
                                    <Attribute Name=""Index"" IsNewNode=""True""/>
                                    <Attribute Name=""Description"" IsLocalized=""True"" IsNewNode=""True""/>
				                </Element>
			                </Element>
		                </Element>
	                </Element>
                </Element>";

            return new Schema(new DictionaryXmlReader().ReadFromString(WizardSchema));
        }

        /// <summary>
        /// Creates a custom Template
        /// </summary>
        /// <param name="sender">XafApplication Object</param>
        /// <param name="e">CreateCustomTemplate EventArgs</param>
        private void Application_CreateCustomTemplate(object sender, CreateCustomTemplateEventArgs e)
        {
            if (((WizardShowViewStrategy)((XafApplication)sender).ShowViewStrategy).ShowInWizard && e.Context == TemplateContext.View)
            {
                string infoPath = string.Format("{0}/{1}[@ID='{2}']", RibbonTemplatesInfoNodeWrapper.NodeName, TemplateInfoNodeWrapper.NodeName, e.Context.Name);
                e.Template = (IFrameTemplate)new WizardRibbonDetailViewForm(new TemplateInfoNodeWrapper((DictionaryNode)e.Application.Model.RootNode.GetChildNodeByPath(infoPath)));
            }
        }

        #endregion
    }
}

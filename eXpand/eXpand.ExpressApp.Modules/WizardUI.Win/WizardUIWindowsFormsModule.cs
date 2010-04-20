//-----------------------------------------------------------------------
// <copyright file="WizardUIWindowsFormsModule.cs" created="03.06.2009" company="VenDoc Software GmbH">
//     Copyright (c) VenDoc Software GmbH. All rights reserved.
// </copyright>
// <author>Martin Praxmarer</author>
//-----------------------------------------------------------------------

using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.WizardUI.Win{

    public interface IModelDetailViewWizard : IModelNode
    {
        bool ShowInWizard { get; set; }
        bool ShowCompletionWizardPage { get; set; }
        IModelDetailViewWizardPages Pages { get; }
    }

    [DisplayProperty("Caption")]
    public interface IModelDetailViewWizardPages : IModelNode, IModelList<IModelDetailViewWizardPage>
    {
    }

    [KeyProperty("ID")]
    public interface IModelDetailViewWizardPage : IModelNode
    {
        [Required()]
        string ID { get; set; }

        [Localizable(true)]
        string Caption { get; set; }

        [Required()]
        [DataSourceProperty("Application.Views")]
        [DataSourceCriteria("ModelClass Is Not Null And ModelClass.Name = '@This.Name'")]
        IModelDetailView DetailView { get; set; }
        [Localizable(true)]
        string Description { get; set; }
    }

    /// <summary>
    /// Contains an RibbonDetailView Template with an Wizard Control on it
    /// </summary>
    [ToolboxItem(true)]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxItemFilter("Xaf.Platform.Win")]
    [Description("Contains an RibbonDetailView Template with an Wizard Control on it.")]
    public sealed class WizardUIWindowsFormsModule : ModuleBase{
        #region Methods
        /// <summary>
        /// Initializes the Module
        /// </summary>
        /// <param name="application">XafApplication Object</param>
        public override void Setup(XafApplication application){
            base.Setup(application);

            application.CreateCustomTemplate += Application_CreateCustomTemplate;
        }

        public override void ExtendModelInterfaces(DevExpress.ExpressApp.Model.ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelDetailView, IModelDetailViewWizard>();
        }

        /// <summary>
        /// Creates a custom Template
        /// </summary>
        /// <param name="sender">XafApplication Object</param>
        /// <param name="e">CreateCustomTemplate EventArgs</param>
        private void Application_CreateCustomTemplate(object sender, CreateCustomTemplateEventArgs e)
        {
            if (e.Context == "WizardDetailViewForm")
            {
                e.Template = new eXpand.ExpressApp.WizardUI.Win.Templates.WizardDetailViewForm();
            }
        }

        #endregion
    }
}
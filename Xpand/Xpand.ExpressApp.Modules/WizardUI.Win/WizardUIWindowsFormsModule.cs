//-----------------------------------------------------------------------
// <copyright file="WizardUIWindowsFormsModule.cs" created="03.06.2009" company="VenDoc Software GmbH">
//     Copyright (c) VenDoc Software GmbH. All rights reserved.
// </copyright>
// <author>Martin Praxmarer</author>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Base;
using DevExpress.Utils;

namespace Xpand.ExpressApp.WizardUI.Win {

    public interface IModelDetailViewWizard : IModelNode {
        IModelDetailViewWizardPages Wizard { get; }
    }
    [ModelNodesGenerator(typeof(DetailViewWizardPagesNodesGenerator))]
    public interface IModelDetailViewWizardPages : IModelNode, IModelList<IModelDetailViewWizardPage> {
        bool ShowInWizard { get; set; }
        bool NewObjectsOnly { get; set; }
        bool ShowCompletionWizardPage { get; set; }
    }

    public class DetailViewWizardPagesNodesGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {

        }
    }

    [DisplayProperty("Caption"), ModelDisplayName("WizardPage"), ModelPersistentName("WizardPage")]
    public interface IModelDetailViewWizardPage : IModelNode {
        [Localizable(true)]
        string Caption { get; set; }

        [Browsable(false)]
        IModelList<IModelDetailView> DetailViews { get; }

        [Required, DataSourceProperty("DetailViews"), ModelPersistentName("ViewID")]
        IModelDetailView DetailView { get; set; }

        [Localizable(true)]
        string Description { get; set; }
    }

    [DomainLogic(typeof(IModelDetailViewWizardPage))]
    public static class ModelDetailViewWizardPageLogic {
        public static CalculatedModelNodeList<IModelDetailView> Get_DetailViews(IModelDetailViewWizardPage wizardPage) {
            var views = new CalculatedModelNodeList<IModelDetailView>();
            if (wizardPage.Parent == null) {
                return views;
            }

            var parentView = wizardPage.Parent.Parent as IModelDetailView;
            if (parentView == null || parentView.ModelClass == null) {
                return views;
            }

            views.AddRange(wizardPage.Application.Views.OfType<IModelDetailView>().Where(modelView => modelView.ModelClass == parentView.ModelClass));

            return views;
        }
    }

    /// <summary>
    /// Contains an RibbonDetailView Template with an Wizard Control on it
    /// </summary>
    [ToolboxItem(true)]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxItemFilter("Xaf.Platform.Win")]
    [Description("Contains an RibbonDetailView Template with an Wizard Control on it.")]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class WizardUIWindowsFormsModule : XpandModuleBase {
        public WizardUIWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(ValidationModule));
        }
        #region Methods
        /// <summary>
        /// Initializes the Module
        /// </summary>
        /// <param name="application">XafApplication Object</param>
        public override void Setup(XafApplication application) {
            base.Setup(application);

            application.CreateCustomTemplate += Application_CreateCustomTemplate;
        }


        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelDetailView, IModelDetailViewWizard>();
        }

        /// <summary>
        /// Creates a custom Template
        /// </summary>
        /// <param name="sender">XafApplication Object</param>
        /// <param name="e">CreateCustomTemplate EventArgs</param>
        private void Application_CreateCustomTemplate(object sender, CreateCustomTemplateEventArgs e) {
            if (e.Context == "WizardDetailViewForm") {
                e.Template = new Templates.WizardDetailViewForm();
            }
        }

        #endregion
    }
}
//-----------------------------------------------------------------------
// <copyright file="WizardController.cs" created="02.06.2009" company="VenDoc Software GmbH">
//     Copyright (c) VenDoc Software GmbH. All rights reserved.
// </copyright>
// <author>Martin Praxmarer</author>
//-----------------------------------------------------------------------

namespace eXpand.ExpressApp.WizardUI.Win
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;
    using DevExpress.ExpressApp;
    using DevExpress.ExpressApp.Editors;
    using DevExpress.ExpressApp.SystemModule;
    using DevExpress.ExpressApp.Templates;
    using DevExpress.ExpressApp.Validation;
    using DevExpress.ExpressApp.Win.SystemModule;
    using DevExpress.Persistent.Validation;
    using DevExpress.XtraWizard;
    using eXpand.ExpressApp.WizardUI.Win.Templates;

    /// <summary>
    /// Controller handles Page generation and View Management
    /// </summary>
    public class WizardController : ViewController
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the WizardController class
        /// </summary>
        public WizardController()
        {
            this.TargetViewType = ViewType.DetailView;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Occours when activating the controller
        /// </summary>
        protected override void OnActivated()
        {
            base.OnActivated();

            if (this.Frame.Template != null && this.Frame.Template is WizardRibbonDetailViewForm)
            {
                DictionaryNode wizardNode = this.View.Info.FindChildNode("Wizard");
                WizardRibbonDetailViewForm wizardForm = this.Frame.Template as WizardRibbonDetailViewForm;

                wizardForm.WizardControl.CancelClick += this.WizardControl_CancelClick;
                wizardForm.WizardControl.FinishClick += this.WizardControl_FinishClick;
                wizardForm.WizardControl.NextClick += this.WizardControl_NextClick;
                wizardForm.WizardControl.SelectedPageChanged += this.WizardControl_SelectedPageChanged;
                wizardForm.WizardControl.SelectedPageChanging += this.WizardControl_SelectedPageChanging;
                wizardForm.WizardControl.BeginUpdate();

                try
                {
                    wizardForm.WizardControl.Pages.Clear();
                    
                    foreach (DictionaryNode node in wizardNode.ChildNodes.GetOrderedByIndex())
                    {
                        DetailView dv = Application.CreateDetailView(this.ObjectSpace, node.GetAttributeValue("ViewID"), false);
                        dv.CurrentObject = this.View.CurrentObject;

                        XafWizardPage page = new XafWizardPage() { View = dv };
                        page.Text = node.GetAttributeValue("Caption");
                        page.DescriptionText = node.GetAttributeValue("Description");
                        wizardForm.WizardControl.Pages.Add(page);
                    }
                }
                finally
                {
                    wizardForm.WizardControl.EndUpdate();
                }
            }
        }

        /// <summary>
        /// Occours when the selected Wizard Page has changed
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">WizardPageChanged EventArgs</param>
        private void WizardControl_SelectedPageChanged(object sender, WizardPageChangedEventArgs e)
        {
            ((WizardControl)sender).SelectedPageChanged -= this.WizardControl_SelectedPageChanged;
            this.UpdateCurrentView(e.Page);
        }

        /// <summary>
        /// Occours when the selected Wizard Page is changing
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">WizardPageChanged EventArgs</param>
        private void WizardControl_SelectedPageChanging(object sender, WizardPageChangingEventArgs e)
        {
            this.UpdateCurrentView(e.Page);
        }

        /// <summary>
        /// Sets the current view and updates the viewsitepanel
        /// </summary>
        /// <param name="page">current WizardPage</param>
        private void UpdateCurrentView(BaseWizardPage page)
        {
            ((XafWizardPage)page).View.ErrorMessages.Clear();
            ((Control)((IViewSiteTemplate)this.Frame.Template).ViewSiteControl).Parent = page;

            this.UpdateControllers(((XafWizardPage)page).View);
            this.Frame.Template.SetView(((XafWizardPage)page).View);
            if (!this.View.ErrorMessages.IsEmpty)
            {
                ((XafWizardPage)page).View.ErrorMessages.LoadMessages(this.View.ErrorMessages);
            }
        }

        /// <summary>
        /// Fires after the next button has been clicked
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">WizardCommandButtonClick EventArgs</param>
        private void WizardControl_NextClick(object sender, WizardCommandButtonClickEventArgs e)
        {
            RuleValidationResult result = null;
            RuleSetValidationResult validationResults = new RuleSetValidationResult();
            List<string> usedProperties = new List<string>();
            List<ResultsHighlightController> resultsHighlightControllers = new List<ResultsHighlightController>();
            resultsHighlightControllers.Add(this.Frame.GetController<ResultsHighlightController>());
            
            foreach (var item in ((XafWizardPage)e.Page).View.GetItems<PropertyEditor>())
            {
                if (((Control)item.Control).Visible)
                {
                    usedProperties.Add(item.PropertyName);
                    if (item is ListPropertyEditor)
                    {
                        foreach (var property in ((ListPropertyEditor)item).ListView.Editor.RequiredProperties)
                        {
                            usedProperties.Add(property.TrimEnd('!'));
                        }

                        ResultsHighlightController nestedController = ((ListPropertyEditor)item).Frame.GetController<ResultsHighlightController>();
                        if (nestedController != null)
                        {
                            resultsHighlightControllers.Add(nestedController);
                        }
                    }
                }
            }

            foreach (var obj in this.ObjectSpace.ModifiedObjects)
            {
                IList<IRule> rules = Validator.RuleSet.GetRules(obj, ContextIdentifier.Save);
                foreach (IRule rule in rules)
                {
                    bool ruleInUse = false;
                    foreach (string property in rule.UsedProperties)
                    {
                        if (usedProperties.Contains(property))
                        {
                            ruleInUse = true;
                            break;
                        }
                    }

                    if (ruleInUse)
                    {
                        result = rule.Validate(obj);
                        if (result.State == ValidationState.Invalid)
                        {
                            validationResults.AddResult(new RuleSetValidationResultItem(obj, ContextIdentifier.Save, rule, result));
                        }
                    }
                }
            }

            foreach (ResultsHighlightController resultsHighlightController in resultsHighlightControllers)
            {
                resultsHighlightController.ClearHighlighting();
                if (validationResults.State == ValidationState.Invalid)
                {
                    resultsHighlightController.HighlightResults(validationResults);
                }
            }

            if (validationResults.State == ValidationState.Invalid)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Fires afer the Finish button has been clicked
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">Cancel EventArgs</param>
        private void WizardControl_FinishClick(object sender, CancelEventArgs e)
        {
            this.UpdateControllers(this.View);
            this.Frame.GetController<DetailViewController>().SaveAndCloseAction.DoExecute();
        }

        /// <summary>
        /// Fires after the Cancel button has been clicked
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">Cancel EventArgs</param>
        private void WizardControl_CancelClick(object sender, CancelEventArgs e)
        {
            this.UpdateControllers(this.View);
            this.Frame.GetController<CloseWindowController>().CloseAction.DoExecute();
            if (((WizardControl)sender).SelectedPage != null && this.View != null)
            {
                this.UpdateCurrentView(((WizardControl)sender).SelectedPage);
            }

            e.Cancel = ((Form)this.Frame.Template).DialogResult != DialogResult.Cancel;
        }

        /// <summary>
        /// Sets the current View for all Controllers in the Frame
        /// </summary>
        /// <param name="view">current View</param>
        private void UpdateControllers(DevExpress.ExpressApp.View view)
        {
            foreach (Controller controller in this.Frame.Controllers)
            {
                if (controller is ViewController && !controller.Equals(this))
                {
                    ((ViewController)controller).SetView(null);
                    ((ViewController)controller).SetView(view);
                }
            }
        }

        #endregion
    }
}

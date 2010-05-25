//-----------------------------------------------------------------------
// <copyright file="WizardController.cs" created="02.06.2009" company="VenDoc Software GmbH">
//     Copyright (c) VenDoc Software GmbH. All rights reserved.
// </copyright>
// <author>Martin Praxmarer</author>
//-----------------------------------------------------------------------

namespace eXpand.ExpressApp.WizardUI.Win
{
    using System;
    using System.Linq;
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
        public static event EventHandler WizardPageDetailViewCreating;

        public static event EventHandler WizardPageDetailViewCreated;

        #region Members

        /// <summary>
        /// Wizard DetailForm
        /// </summary>
        private WizardDetailViewForm _WizardForm;

        #endregion

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

            if (this.Frame.Template != null && this.Frame.Template is WizardDetailViewForm)
            {
                IModelDetailViewWizard modelWizard = (IModelDetailViewWizard)(this.View as DetailView).Model;
                this._WizardForm = this.Frame.Template as WizardDetailViewForm;

                this._WizardForm.WizardControl.CancelClick += this.WizardControl_CancelClick;
                this._WizardForm.WizardControl.FinishClick += this.WizardControl_FinishClick;
                this._WizardForm.WizardControl.NextClick += this.WizardControl_NextClick;
                this._WizardForm.WizardControl.SelectedPageChanged += this.WizardControl_SelectedPageChanged;
                this._WizardForm.WizardControl.SelectedPageChanging += this.WizardControl_SelectedPageChanging;
                this._WizardForm.WizardControl.BeginUpdate();

                try
                {
                    CompletionWizardPage finishPage = this._WizardForm.WizardControl.Pages[0] as CompletionWizardPage;
                    foreach (IModelDetailViewWizardPage page in modelWizard.Wizard)
                    {
                        this.OnWizardPageDetailViewCreating();
                        var detailView = Application.CreateDetailView(this.ObjectSpace, page.DetailView, false);
                        this.OnWizardPageDetailViewCreated();
                        detailView.CurrentObject = this.View.CurrentObject;

                        var wizardPage = new XafWizardPage() { View = detailView };
                        wizardPage.Text = page.Caption;
                        wizardPage.DescriptionText = page.Description;
                        this._WizardForm.WizardControl.Pages.Insert(finishPage, wizardPage);
                    }

                    if (!modelWizard.Wizard.ShowCompletionWizardPage)
                    {
                        this._WizardForm.WizardControl.Pages.Remove(finishPage);
                    }
                }
                finally
                {
                    this._WizardForm.WizardControl.EndUpdate();
                }
            }
        }

        protected virtual void OnWizardPageDetailViewCreating()
        {
            if (WizardPageDetailViewCreating != null)
            {
                WizardPageDetailViewCreating(this, EventArgs.Empty);
            }
        }

        protected virtual void OnWizardPageDetailViewCreated()
        {
            if (WizardPageDetailViewCreated != null)
            {
                WizardPageDetailViewCreated(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this._WizardForm != null)
            {
                this._WizardForm.WizardControl.CancelClick -= this.WizardControl_CancelClick;
                this._WizardForm.WizardControl.FinishClick -= this.WizardControl_FinishClick;
                this._WizardForm.WizardControl.NextClick -= this.WizardControl_NextClick;
                this._WizardForm.WizardControl.SelectedPageChanged -= this.WizardControl_SelectedPageChanged;
                this._WizardForm.WizardControl.SelectedPageChanging -= this.WizardControl_SelectedPageChanging;

                foreach (BaseWizardPage page in this._WizardForm.WizardControl.Pages)
                {
                    if (page is XafWizardPage)
                    {
                        ((XafWizardPage)page).View.SynchronizeInfo();
                        ((XafWizardPage)page).View.Dispose();
                    }
                }

                this._WizardForm.WizardControl.Dispose();
            }

            base.Dispose(disposing);
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
            if (page is XafWizardPage)
            {
                var wizardPage = page as XafWizardPage;
                this.OnWizardPageDetailViewCreating();
                wizardPage.View = Application.CreateDetailView(this.ObjectSpace, wizardPage.View.Id, false);
                this.OnWizardPageDetailViewCreated();
                wizardPage.View.CurrentObject = this.View.CurrentObject;

                wizardPage.View.ErrorMessages.Clear();
                ((Control)((IViewSiteTemplate)this.Frame.Template).ViewSiteControl).Parent = page;

                this.UpdateControllers(wizardPage.View);

                this.Frame.Template.SetView(wizardPage.View);
                if (!this.View.ErrorMessages.IsEmpty)
                {
                    wizardPage.View.ErrorMessages.LoadMessages(this.View.ErrorMessages);
                }
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
            string reason;

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
                        if (usedProperties.Contains(property) || !string.IsNullOrEmpty(usedProperties.Where(p => p.EndsWith(String.Format(".{0}", property))).FirstOrDefault()))
                        {
                            ruleInUse = true;
                            break;
                        }
                    }

                    if (ruleInUse && RuleSet.NeedToValidateRule(rule, obj, out reason))
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
            var currentObject = this.View.CurrentObject;
            this.Frame.GetController<DetailViewController>().SaveAndCloseAction.DoExecute();

            if (this._WizardForm.ShowRecordAfterCompletion)
            {
                ObjectSpace os = this.Application.CreateObjectSpace();

                ShowViewParameters showViewParameter = new ShowViewParameters();
                showViewParameter.Context = TemplateContext.View;
                showViewParameter.CreatedView = this.Application.CreateDetailView(os, os.GetObject(currentObject));
                showViewParameter.TargetWindow = TargetWindow.NewWindow;

                this.Application.ShowViewStrategy.ShowView(showViewParameter, new ShowViewSource(null, null));
            }
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
//-----------------------------------------------------------------------
// <copyright file="WizardController.cs" created="02.06.2009" company="VenDoc Software GmbH">
//     Copyright (c) VenDoc Software GmbH. All rights reserved.
// </copyright>
// <author>Martin Praxmarer</author>
//-----------------------------------------------------------------------

namespace Xpand.ExpressApp.WizardUI.Win
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;
    using DevExpress.ExpressApp;
    using DevExpress.ExpressApp.Editors;
    using DevExpress.ExpressApp.SystemModule;
    using DevExpress.ExpressApp.Templates;
    using DevExpress.ExpressApp.Validation;
    using DevExpress.ExpressApp.Win.SystemModule;
    using DevExpress.Persistent.Validation;
    using DevExpress.XtraWizard;
    using Templates;

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
            TargetViewType = ViewType.DetailView;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Occours when activating the controller
        /// </summary>
        protected override void OnActivated()
        {
            base.OnActivated();

            if (Frame.Template != null && Frame.Template is WizardDetailViewForm)
            {
                var modelWizard = (IModelDetailViewWizard)((DetailView)View).Model;
                _WizardForm = Frame.Template as WizardDetailViewForm;

                _WizardForm.WizardControl.CancelClick += WizardControl_CancelClick;
                _WizardForm.WizardControl.FinishClick += WizardControl_FinishClick;
                _WizardForm.WizardControl.NextClick += WizardControl_NextClick;
                _WizardForm.WizardControl.SelectedPageChanged += WizardControl_SelectedPageChanged;
                _WizardForm.WizardControl.SelectedPageChanging += WizardControl_SelectedPageChanging;
                _WizardForm.WizardControl.BeginUpdate();

                try
                {
                    var finishPage = _WizardForm.WizardControl.Pages[0] as CompletionWizardPage;
                    foreach (IModelDetailViewWizardPage page in modelWizard.Wizard)
                    {
                        OnWizardPageDetailViewCreating();
                        var detailView = Application.CreateDetailView(ObjectSpace, page.DetailView, false);
                        OnWizardPageDetailViewCreated();
                        detailView.CurrentObject = View.CurrentObject;

                        var wizardPage = new XafWizardPage
                        {
                            View = detailView,
                            Text = page.Caption,
                            DescriptionText = page.Description
                        };
                        _WizardForm.WizardControl.Pages.Insert(finishPage, wizardPage);
                    }

                    if (!modelWizard.Wizard.ShowCompletionWizardPage)
                    {
                        _WizardForm.WizardControl.Pages.Remove(finishPage);
                    }
                }
                finally
                {
                    _WizardForm.WizardControl.EndUpdate();
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
            if (disposing && _WizardForm != null)
            {
                _WizardForm.WizardControl.CancelClick -= WizardControl_CancelClick;
                _WizardForm.WizardControl.FinishClick -= WizardControl_FinishClick;
                _WizardForm.WizardControl.NextClick -= WizardControl_NextClick;
                _WizardForm.WizardControl.SelectedPageChanged -= WizardControl_SelectedPageChanged;
                _WizardForm.WizardControl.SelectedPageChanging -= WizardControl_SelectedPageChanging;

                foreach (BaseWizardPage page in _WizardForm.WizardControl.Pages)
                {
                    if (page is XafWizardPage)
                    {
                        ((XafWizardPage)page).View.SynchronizeInfo();
                        ((XafWizardPage)page).View.Dispose();
                    }
                }

                _WizardForm.WizardControl.Dispose();
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
            ((WizardControl)sender).SelectedPageChanged -= WizardControl_SelectedPageChanged;
            UpdateCurrentView(e.Page);
        }

        /// <summary>
        /// Occours when the selected Wizard Page is changing
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">WizardPageChanged EventArgs</param>
        private void WizardControl_SelectedPageChanging(object sender, WizardPageChangingEventArgs e)
        {
            UpdateCurrentView(e.Page);
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
                OnWizardPageDetailViewCreating();
                wizardPage.View = Application.CreateDetailView(ObjectSpace, wizardPage.View.Id, false);
                OnWizardPageDetailViewCreated();
                wizardPage.View.CurrentObject = View.CurrentObject;

                wizardPage.View.ErrorMessages.Clear();
                ((Control)((IViewSiteTemplate)Frame.Template).ViewSiteControl).Parent = page;

                UpdateControllers(wizardPage.View);

                Frame.Template.SetView(wizardPage.View);
                if (!View.ErrorMessages.IsEmpty)
                {
                    wizardPage.View.ErrorMessages.LoadMessages(View.ErrorMessages);
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
            e.Handled = !this.Validate(e.Page as XafWizardPage);
        }

        /// <summary>
        /// Fires after the next button has been clicked
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">WizardCommandButtonClick EventArgs</param>
        private bool Validate(XafWizardPage page)
        {
            RuleValidationResult result;
            var validationResults = new RuleSetValidationResult();
            var usedProperties = new List<string>();
            var resultsHighlightControllers = new List<ResultsHighlightController> { Frame.GetController<ResultsHighlightController>() };

            foreach (var item in page.View.GetItems<PropertyEditor>())
            {
                if (item.Control != null && ((Control)item.Control).Visible)
                {
                    usedProperties.Add(item.PropertyName);
                    if (item is ListPropertyEditor)
                    {
                        usedProperties.AddRange(((ListPropertyEditor)item).ListView.Editor.RequiredProperties.Select(property => property.TrimEnd('!')));

                        var nestedController = ((ListPropertyEditor)item).Frame.GetController<ResultsHighlightController>();
                        if (nestedController != null)
                        {
                            resultsHighlightControllers.Add(nestedController);
                        }
                    }
                }
            }

            var modifiedObjects = page.View.ObjectTypeInfo.IsPersistent ? ObjectSpace.ModifiedObjects : new List<object>() { page.View.CurrentObject };
            foreach (var obj in modifiedObjects)
            {
                IList<IRule> rules = Validator.RuleSet.GetRules(obj, ContextIdentifier.Save);
                foreach (IRule rule in rules)
                {
                    bool ruleInUse = rule.UsedProperties.Any(property => usedProperties.Contains(property) || !string.IsNullOrEmpty(usedProperties.Where(p => p.EndsWith(String.Format(".{0}", property))).FirstOrDefault()));

                    string reason;
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

            return validationResults.State == ValidationState.Valid;
        }

        /// <summary>
        /// Fires afer the Finish button has been clicked
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">Cancel EventArgs</param>
        private void WizardControl_FinishClick(object sender, CancelEventArgs e)
        {
            if (!View.ObjectTypeInfo.IsPersistent && !Validate(_WizardForm.WizardControl.SelectedPage as XafWizardPage))
            {
                e.Cancel = true;
                return;
            }

            UpdateControllers(View);
            var currentObject = View.CurrentObject;
            var controller = Frame.GetController<DetailViewController>();
            if (controller.SaveAndCloseAction.Active && controller.SaveAndCloseAction.Enabled)
            {
                Frame.GetController<DetailViewController>().SaveAndCloseAction.DoExecute();
            }
            else
            {
                Frame.GetController<CloseWindowController>().CloseAction.DoExecute();
            }

            if (_WizardForm.ShowRecordAfterCompletion)
            {
                var os = Application.CreateObjectSpace();

                var showViewParameter = new ShowViewParameters
                {
                    Context = TemplateContext.View,
                    CreatedView = Application.CreateDetailView(os, os.GetObject(currentObject)),
                    TargetWindow = TargetWindow.NewWindow
                };

                Application.ShowViewStrategy.ShowView(showViewParameter, new ShowViewSource(null, null));
            }
        }

        /// <summary>
        /// Fires after the Cancel button has been clicked
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">Cancel EventArgs</param>
        private void WizardControl_CancelClick(object sender, CancelEventArgs e)
        {
            UpdateControllers(View);
            Frame.GetController<CloseWindowController>().CloseAction.DoExecute();
            if (((WizardControl)sender).SelectedPage != null && View != null)
            {
                UpdateCurrentView(((WizardControl)sender).SelectedPage);
            }

            e.Cancel = ((Form)Frame.Template).DialogResult != DialogResult.Cancel;
        }

        /// <summary>
        /// Sets the current View for all Controllers in the Frame
        /// </summary>
        /// <param name="view">current View</param>
        private void UpdateControllers(DevExpress.ExpressApp.View view)
        {
            foreach (Controller controller in Frame.Controllers)
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
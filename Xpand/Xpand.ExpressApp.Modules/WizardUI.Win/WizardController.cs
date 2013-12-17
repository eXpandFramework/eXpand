using Fasterflect;

namespace Xpand.ExpressApp.WizardUI.Win {
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
    public class WizardController : ViewController {
        public static event EventHandler WizardPageDetailViewCreating;

        public static event EventHandler WizardPageDetailViewCreated;

        #region Members

        /// <summary>
        /// Wizard DetailForm
        /// </summary>
        private WizardDetailViewForm _wizardForm;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the WizardController class
        /// </summary>
        public WizardController() {
            TargetViewType = ViewType.DetailView;
        }

        #endregion

        public WizardDetailViewForm WizardForm{
            get { return _wizardForm; }
        }

        #region Methods

        /// <summary>
        /// Occours when activating the controller
        /// </summary>
        protected override void OnActivated() {
            base.OnActivated();

            if (Frame.Template is WizardDetailViewForm) {
                var modelWizard = (IModelDetailViewWizard)((DetailView)View).Model;
                _wizardForm = Frame.Template as WizardDetailViewForm;

                _wizardForm.WizardControl.CancelClick += WizardControl_CancelClick;
                _wizardForm.WizardControl.FinishClick += WizardControl_FinishClick;
                _wizardForm.WizardControl.NextClick += WizardControl_NextClick;
                _wizardForm.WizardControl.SelectedPageChanged += WizardControl_SelectedPageChanged;
                _wizardForm.WizardControl.SelectedPageChanging += WizardControl_SelectedPageChanging;
                _wizardForm.WizardControl.BeginUpdate();

                try {
                    var finishPage = _wizardForm.WizardControl.Pages[0] as CompletionWizardPage;
                    foreach (IModelDetailViewWizardPage page in modelWizard.Wizard) {
                        OnWizardPageDetailViewCreating();
                        var detailView = Application.CreateDetailView(ObjectSpace, page.DetailView, false);
                        OnWizardPageDetailViewCreated();
                        detailView.CurrentObject = View.CurrentObject;

                        var wizardPage = new XafWizardPage {
                            View = detailView,
                            Text = page.Caption,
                            DescriptionText = page.Description
                        };
                        _wizardForm.WizardControl.Pages.Insert(finishPage, wizardPage);
                    }

                    if (!modelWizard.Wizard.ShowCompletionWizardPage) {
                        _wizardForm.WizardControl.Pages.Remove(finishPage);
                    }
                } finally {
                    _wizardForm.WizardControl.EndUpdate();
                }
            }
        }

        protected virtual void OnWizardPageDetailViewCreating() {
            if (WizardPageDetailViewCreating != null) {
                WizardPageDetailViewCreating(this, EventArgs.Empty);
            }
        }

        protected virtual void OnWizardPageDetailViewCreated() {
            if (WizardPageDetailViewCreated != null) {
                WizardPageDetailViewCreated(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && _wizardForm != null) {
                _wizardForm.WizardControl.CancelClick -= WizardControl_CancelClick;
                _wizardForm.WizardControl.FinishClick -= WizardControl_FinishClick;
                _wizardForm.WizardControl.NextClick -= WizardControl_NextClick;
                _wizardForm.WizardControl.SelectedPageChanged -= WizardControl_SelectedPageChanged;
                _wizardForm.WizardControl.SelectedPageChanging -= WizardControl_SelectedPageChanging;

                foreach (BaseWizardPage page in _wizardForm.WizardControl.Pages) {
                    var wizardPage = page as XafWizardPage;
                    if (wizardPage != null) {
                        wizardPage.View.SaveModel();
                        wizardPage.View.Dispose();
                    }
                }

                _wizardForm.WizardControl.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Occours when the selected Wizard Page has changed
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">WizardPageChanged EventArgs</param>
        private void WizardControl_SelectedPageChanged(object sender, WizardPageChangedEventArgs e) {
            if (_wizardForm.WizardControl.Pages.IndexOf(e.Page) == 0 && e.Direction == Direction.Forward)
                UpdateCurrentView(e.Page);

            FocusDefaultItem();
        }

        /// <summary>
        /// Occours when the selected Wizard Page is changing
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">WizardPageChanged EventArgs</param>
        private void WizardControl_SelectedPageChanging(object sender, WizardPageChangingEventArgs e) {
            UpdateCurrentView(e.Page);
        }

        /// <summary>
        /// Sets the current view and updates the viewsitepanel
        /// </summary>
        /// <param name="page">current WizardPage</param>
        private void UpdateCurrentView(BaseWizardPage page) {
            if (page is XafWizardPage) {
                var wizardPage = page as XafWizardPage;
                if (wizardPage.View != null) {
                    wizardPage.View.SaveModel();
                    var detailView = wizardPage.View;
                    OnWizardPageDetailViewCreating();
                    wizardPage.View = Application.CreateDetailView(ObjectSpace, wizardPage.View.Id, false);
                    OnWizardPageDetailViewCreated();
                    wizardPage.View.CurrentObject = View.CurrentObject;

                    wizardPage.View.ErrorMessages.Clear();
                    ((Control)((IViewSiteTemplate)Frame.Template).ViewSiteControl).Parent = page;

                    UpdateControllers(wizardPage.View);
                    detailView.Dispose();
                    Frame.Template.SetView(wizardPage.View);
                    if (!View.ErrorMessages.IsEmpty)
                        wizardPage.View.ErrorMessages.LoadMessages(View.ErrorMessages);
                }
            }
        }

        private void FocusDefaultItem() {
            Frame.GetController<FocusDefaultDetailViewItemController>().CallMethod("FocusDefaultItemControl");
        }

        /// <summary>
        /// Fires after the next button has been clicked
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">WizardCommandButtonClick EventArgs</param>
        private void WizardControl_NextClick(object sender, WizardCommandButtonClickEventArgs e) {
            e.Handled = !Validate(e.Page as XafWizardPage);
        }

        /// <summary>
        /// Fires after the next button has been clicked
        /// </summary>
        private bool Validate(XafWizardPage page) {
            var validationResults = new RuleSetValidationResult();
            var usedProperties = new List<string>();
            var resultsHighlightControllers = new List<ResultsHighlightController> { Frame.GetController<ResultsHighlightController>() };

            foreach (var item in page.View.GetItems<PropertyEditor>()) {
                if (item.Control != null && ((Control)item.Control).Visible) {
                    usedProperties.Add(item.PropertyName);
                    var editor = item as ListPropertyEditor;
                    if (editor != null) {
                        usedProperties.AddRange(editor.ListView.Editor.RequiredProperties.Select(property => property.TrimEnd('!')));

                        var nestedController = editor.Frame.GetController<ResultsHighlightController>();
                        if (nestedController != null) {
                            resultsHighlightControllers.Add(nestedController);
                        }
                    }
                }
            }

            var modifiedObjects = page.View.ObjectTypeInfo.IsPersistent ? ObjectSpace.ModifiedObjects : new List<object> { page.View.CurrentObject };
            foreach (var obj in modifiedObjects) {
                IList<IRule> rules = Validator.RuleSet.GetRules(obj, ContextIdentifier.Save);
                foreach (IRule rule in rules) {
                    bool ruleInUse = rule.UsedProperties.Any(property => usedProperties.Contains(property) || !string.IsNullOrEmpty(usedProperties.FirstOrDefault(p => p.EndsWith(String.Format(".{0}", property)))));

                    string reason;
                    if (ruleInUse && RuleSet.NeedToValidateRule(ObjectSpace, rule, obj, out reason)) {
                        RuleValidationResult result = rule.Validate(obj);

                        if (result.State == ValidationState.Invalid) {
                            validationResults.AddResult(new RuleSetValidationResultItem(obj, ContextIdentifier.Save, rule, result));
                        }
                    }
                }
            }

            foreach (ResultsHighlightController resultsHighlightController in resultsHighlightControllers) {
                resultsHighlightController.ClearHighlighting();
                if (validationResults.State == ValidationState.Invalid) {
                    resultsHighlightController.HighlightResults(validationResults);
                }
            }

            return validationResults.State != ValidationState.Invalid;
        }

        /// <summary>
        /// Fires afer the Finish button has been clicked
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">Cancel EventArgs</param>
        private void WizardControl_FinishClick(object sender, CancelEventArgs e) {
            if (!View.ObjectTypeInfo.IsPersistent && !Validate(_wizardForm.WizardControl.SelectedPage as XafWizardPage)) {
                e.Cancel = true;
                return;
            }

            UpdateControllers(View);
            var currentObject = View.CurrentObject;
            var controller = Frame.GetController<ModificationsController>();
            if (controller.SaveAndCloseAction.Active && controller.SaveAndCloseAction.Enabled) {
                Frame.GetController<ModificationsController>().SaveAndCloseAction.DoExecute();
            } else {
                Frame.GetController<CloseWindowController>().CloseAction.DoExecute();
            }

            if (_wizardForm.ShowRecordAfterCompletion) {
                var os = Application.CreateObjectSpace(View.ObjectTypeInfo.Type);

                var showViewParameter = new ShowViewParameters {
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
        private void WizardControl_CancelClick(object sender, CancelEventArgs e) {
            UpdateControllers(View);
            Frame.GetController<CloseWindowController>().CloseAction.DoExecute();
            if (((WizardControl)sender).SelectedPage != null && View != null) {
                UpdateCurrentView(((WizardControl)sender).SelectedPage);
            }

            e.Cancel = ((Form)Frame.Template).DialogResult != DialogResult.Cancel;
        }

        /// <summary>
        /// Sets the current View for all Controllers in the Frame
        /// </summary>
        /// <param name="view">current View</param>
        private void UpdateControllers(DevExpress.ExpressApp.View view) {
            foreach (Controller controller in Frame.Controllers) {
                if (controller is ViewController && !controller.Equals(this)) {
                    ((ViewController)controller).SetView(null);
                    ((ViewController)controller).SetView(view);
                }
            }
        }

        #endregion
    }
}
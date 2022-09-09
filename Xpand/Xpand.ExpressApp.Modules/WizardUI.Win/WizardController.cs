using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
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
using Fasterflect;
using Xpand.ExpressApp.WizardUI.Win.Templates;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.WizardUI.Win {
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

        public WizardDetailViewForm WizardForm => _wizardForm;

        #region Methods

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            Frame.TemplateChanged+=FrameOnTemplateChanged;
        }

        private void FrameOnTemplateChanged(object sender, EventArgs eventArgs){
            var wizardDetailViewForm = Frame.Template as WizardDetailViewForm;
            if (wizardDetailViewForm != null){
                wizardDetailViewForm.WizardControl.CancelClick += WizardControl_CancelClick;
                wizardDetailViewForm.WizardControl.FinishClick += WizardControl_FinishClick;
                wizardDetailViewForm.WizardControl.NextClick += WizardControl_NextClick;
                wizardDetailViewForm.WizardControl.SelectedPageChanged += WizardControl_SelectedPageChanged;
            }
        }

        protected override void OnActivated() {
            base.OnActivated();

            var wizardDetailViewForm = Frame.Template as WizardDetailViewForm;
            if (wizardDetailViewForm != null) {
                var modelWizard = (IModelDetailViewWizard)((DetailView)View).Model;
                _wizardForm = wizardDetailViewForm;
                _wizardForm.WizardControl.BeginUpdate();

                try {
                    var finishPage = _wizardForm.WizardControl.Pages[0] as CompletionWizardPage;
                    foreach (var page in modelWizard.Wizard){
                        CreateWizardPage(page, finishPage);
                    }

                    if (modelWizard.Wizard.Any()&& !modelWizard.Wizard.ShowCompletionWizardPage) {
                        _wizardForm.WizardControl.Pages.Remove(finishPage);
                    }
                } finally {
                    _wizardForm.WizardControl.EndUpdate();
                }
            }
        }

        private void CreateWizardPage(IModelDetailViewWizardPage page, CompletionWizardPage finishPage){
            OnWizardPageDetailViewCreating();
            var detailView = Application.CreateDetailView(ObjectSpace, page.DetailView, false);
            OnWizardPageDetailViewCreated();
            detailView.CurrentObject = View.CurrentObject;

            var wizardPage = new XafWizardPage{
                View = detailView,
                Text = page.Caption,
                DescriptionText = page.Description
            };
            _wizardForm.WizardControl.Pages.Insert(finishPage, wizardPage);
        }

        protected virtual void OnWizardPageDetailViewCreating(){
            WizardPageDetailViewCreating?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnWizardPageDetailViewCreated(){
            WizardPageDetailViewCreated?.Invoke(this, EventArgs.Empty);
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

                foreach (BaseWizardPage page in _wizardForm.WizardControl.Pages) {
                    var wizardPage = page as XafWizardPage;
                    if (wizardPage != null) {
                        wizardPage.View.SaveModel();
//                        wizardPage.View.Dispose();
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
            var wizardPage = e.Page as XafWizardPage;
            if (wizardPage != null){
                UpdateCurrentView(wizardPage);
                FocusDefaultItem();
            }
        }


        /// <summary>
        /// Sets the current view and updates the viewsitepanel
        /// </summary>
        /// <param name="page">current WizardPage</param>
        private void UpdateCurrentView(XafWizardPage page){
            var xafWizardPage = page;
            var wizardPage = xafWizardPage;
            wizardPage.View.SaveModel();
            ((Control)((IViewSiteTemplate)Frame.Template).ViewSiteControl).Parent = page;
            Frame.SetFieldValue("view", null);
            Frame.SetView(wizardPage.View, true, Frame, false);
            if (!View.ErrorMessages.IsEmpty)
                wizardPage.View.ErrorMessages.LoadMessages(View.ErrorMessages);

        }

        private void FocusDefaultItem() {
            Frame.GetController<FocusDefaultDetailViewItemController>(controller => controller.CallMethod("FocusDefaultItemControl"));
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
        private bool Validate(XafWizardPage page){
            if (page == null)
                return true;
            var validationResults = new RuleSetValidationResult();
            var usedProperties = new List<string>();
            var resultsHighlightControllers = new List<ResultsHighlightController> { Frame.GetController<ResultsHighlightController>() }.Where(controller => controller!=null).ToList();

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

            var modifiedObjects = ModifiedObjects(page);
            foreach (var obj in modifiedObjects) {
                IList<IRule> rules = Validator.RuleSet.GetRules(obj, ContextIdentifier.Save);
                foreach (IRule rule in rules) {
                    bool ruleInUse = rule.UsedProperties.Any(property => usedProperties.Contains(property) || !string.IsNullOrEmpty(usedProperties.FirstOrDefault(p => p.EndsWith(
                                                                             $".{property}"))));
                    string reason;
                    if (ruleInUse && RuleSet.NeedToValidateRule(ObjectSpace, rule, obj, out reason)) {
                        var objectSpaceLink = rule as IObjectSpaceLink;
                        if (objectSpaceLink != null) objectSpaceLink.ObjectSpace=ObjectSpace;
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

        private IList ModifiedObjects(XafWizardPage page){
            IList modifiedObjects;
            if (page.View.ObjectTypeInfo.IsPersistent){
                modifiedObjects = ObjectSpace.ModifiedObjects;
                if (modifiedObjects.Count == 0)
                    modifiedObjects.Add(page.View.CurrentObject);
            }
            else
                modifiedObjects =
                    new List<object>{page.View.CurrentObject};
            return modifiedObjects;
        }

        /// <summary>
        /// Fires afer the Finish button has been clicked
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">Cancel EventArgs</param>
        [SuppressMessage("Usage", "XAF0022:Avoid calling the ShowViewStrategyBase.ShowView() method")]
        private void WizardControl_FinishClick(object sender, CancelEventArgs e) {
            if (!Validate(_wizardForm.WizardControl.SelectedPage as XafWizardPage)) {
                e.Cancel = true;
                return;
            }

            ObjectSpace.CommitChanges();

            if (_wizardForm.ShowRecordAfterCompletion) {
                var os = Application.CreateObjectSpace(View.ObjectTypeInfo.Type);

                var showViewParameter = new ShowViewParameters {
                    Context = TemplateContext.View,
                    CreatedView = Application.CreateDetailView(os, os.GetObject(View.CurrentObject)),
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
//            UpdateControllers(View);
            Frame.GetController<CloseWindowController>(controller => controller.CloseAction.DoExecute());
            if (((WizardControl)sender).SelectedPage != null && View != null) {
                UpdateCurrentView((XafWizardPage) ((WizardControl)sender).SelectedPage);
            }

            e.Cancel = ((Form)Frame.Template).DialogResult != DialogResult.Cancel;
        }


        #endregion
    }
}
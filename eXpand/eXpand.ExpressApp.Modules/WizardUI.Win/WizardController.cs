//-----------------------------------------------------------------------
// <copyright file="WizardController.cs" created="02.06.2009" company="VenDoc Software GmbH">
//     Copyright (c) VenDoc Software GmbH. All rights reserved.
// </copyright>
// <author>Martin Praxmarer</author>
//-----------------------------------------------------------------------

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
using View=DevExpress.ExpressApp.View;

namespace eXpand.ExpressApp.WizardUI.Win{
    /// <summary>
    /// Controller handles Page generation and View Management
    /// </summary>
    public class WizardController : ViewController{
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the WizardController class
        /// </summary>
        public WizardController(){
            TargetViewType = ViewType.DetailView;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Occours when activating the controller
        /// </summary>
        protected override void OnActivated(){
            base.OnActivated();

            if (Frame.Template != null && Frame.Template is WizardRibbonDetailViewForm){
                DictionaryNode wizardNode = View.Info.FindChildNode("Wizard");
                var wizardForm = Frame.Template as WizardRibbonDetailViewForm;

                wizardForm.WizardControl.CancelClick += WizardControl_CancelClick;
                wizardForm.WizardControl.FinishClick += WizardControl_FinishClick;
                wizardForm.WizardControl.NextClick += WizardControl_NextClick;
                wizardForm.WizardControl.SelectedPageChanged += WizardControl_SelectedPageChanged;
                wizardForm.WizardControl.SelectedPageChanging += WizardControl_SelectedPageChanging;
                wizardForm.WizardControl.BeginUpdate();

                try{
                    wizardForm.WizardControl.Pages.Clear();

                    foreach (DictionaryNode node in wizardNode.ChildNodes.GetOrderedByIndex()){
                        DetailView dv = Application.CreateDetailView(ObjectSpace, node.GetAttributeValue("ViewID"),
                                                                     false);
                        dv.CurrentObject = View.CurrentObject;

                        var page = new XafWizardPage{
                                                        View = dv,
                                                        Text = node.GetAttributeValue("Caption"),
                                                        DescriptionText = node.GetAttributeValue("Description")
                                                    };
                        wizardForm.WizardControl.Pages.Add(page);
                    }
                }
                finally{
                    wizardForm.WizardControl.EndUpdate();
                }
            }
        }

        /// <summary>
        /// Occours when the selected Wizard Page has changed
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">WizardPageChanged EventArgs</param>
        private void WizardControl_SelectedPageChanged(object sender, WizardPageChangedEventArgs e){
            ((WizardControl) sender).SelectedPageChanged -= WizardControl_SelectedPageChanged;
            UpdateCurrentView(e.Page);
        }

        /// <summary>
        /// Occours when the selected Wizard Page is changing
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">WizardPageChanged EventArgs</param>
        private void WizardControl_SelectedPageChanging(object sender, WizardPageChangingEventArgs e){
            UpdateCurrentView(e.Page);
        }

        /// <summary>
        /// Sets the current view and updates the viewsitepanel
        /// </summary>
        /// <param name="page">current WizardPage</param>
        private void UpdateCurrentView(BaseWizardPage page){
            ((XafWizardPage) page).View.ErrorMessages.Clear();
            ((Control) ((IViewSiteTemplate) Frame.Template).ViewSiteControl).Parent = page;

            UpdateControllers(((XafWizardPage) page).View);
            Frame.Template.SetView(((XafWizardPage) page).View);
            if (!View.ErrorMessages.IsEmpty){
                ((XafWizardPage) page).View.ErrorMessages.LoadMessages(View.ErrorMessages);
            }
        }

        /// <summary>
        /// Fires after the next button has been clicked
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">WizardCommandButtonClick EventArgs</param>
        private void WizardControl_NextClick(object sender, WizardCommandButtonClickEventArgs e){
            RuleValidationResult result;
            var validationResults = new RuleSetValidationResult();
            var usedProperties = new List<string>();
            var resultsHighlightControllers = new List<ResultsHighlightController>
                                              {Frame.GetController<ResultsHighlightController>()};

            foreach (PropertyEditor item in ((XafWizardPage) e.Page).View.GetItems<PropertyEditor>()){
                if (((Control) item.Control).Visible){
                    usedProperties.Add(item.PropertyName);
                    if (item is ListPropertyEditor){
                        foreach (string property in ((ListPropertyEditor) item).ListView.Editor.RequiredProperties){
                            usedProperties.Add(property.TrimEnd('!'));
                        }

                        var nestedController =
                            ((ListPropertyEditor) item).Frame.GetController<ResultsHighlightController>();
                        if (nestedController != null){
                            resultsHighlightControllers.Add(nestedController);
                        }
                    }
                }
            }

            foreach (object obj in ObjectSpace.ModifiedObjects){
                IList<IRule> rules = Validator.RuleSet.GetRules(obj, ContextIdentifier.Save);
                foreach (IRule rule in rules){
                    bool ruleInUse = false;
                    foreach (string property in rule.UsedProperties){
                        if (usedProperties.Contains(property)){
                            ruleInUse = true;
                            break;
                        }
                    }

                    if (ruleInUse){
                        result = rule.Validate(obj);
                        if (result.State == ValidationState.Invalid){
                            validationResults.AddResult(new RuleSetValidationResultItem(obj, ContextIdentifier.Save,
                                                                                        rule, result));
                        }
                    }
                }
            }

            foreach (ResultsHighlightController resultsHighlightController in resultsHighlightControllers){
                resultsHighlightController.ClearHighlighting();
                if (validationResults.State == ValidationState.Invalid){
                    resultsHighlightController.HighlightResults(validationResults);
                }
            }

            if (validationResults.State == ValidationState.Invalid){
                e.Handled = true;
            }
        }

        /// <summary>
        /// Fires afer the Finish button has been clicked
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">Cancel EventArgs</param>
        private void WizardControl_FinishClick(object sender, CancelEventArgs e){
            UpdateControllers(View);
            Frame.GetController<DetailViewController>().SaveAndCloseAction.DoExecute();
        }

        /// <summary>
        /// Fires after the Cancel button has been clicked
        /// </summary>
        /// <param name="sender">Wizard Control</param>
        /// <param name="e">Cancel EventArgs</param>
        private void WizardControl_CancelClick(object sender, CancelEventArgs e){
            UpdateControllers(View);
            Frame.GetController<CloseWindowController>().CloseAction.DoExecute();
            if (((WizardControl) sender).SelectedPage != null && View != null){
                UpdateCurrentView(((WizardControl) sender).SelectedPage);
            }

            e.Cancel = ((Form) Frame.Template).DialogResult != DialogResult.Cancel;
        }

        /// <summary>
        /// Sets the current View for all Controllers in the Frame
        /// </summary>
        /// <param name="view">current View</param>
        private void UpdateControllers(View view){
            foreach (Controller controller in Frame.Controllers){
                if (controller is ViewController && !controller.Equals(this)){
                    ((ViewController) controller).SetView(null);
                    ((ViewController) controller).SetView(view);
                }
            }
        }
        #endregion
    }
}
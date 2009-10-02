//-----------------------------------------------------------------------
// <copyright file="WizardShowViewStrategy.cs" created="02.06.2009" company="VenDoc Software GmbH">
//     Copyright (c) VenDoc Software GmbH. All rights reserved.
// </copyright>
// <author>Martin Praxmarer</author>
//-----------------------------------------------------------------------

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;

namespace eXpand.ExpressApp.WizardUI.Win{
    /// <summary>
    /// Custom ShowViewStrategy to handle the creation of an Wizard Template
    /// </summary>
    public class WizardShowViewStrategy : ShowInMultipleWindowsStrategy{
        #region Members
        /// <summary>
        /// Indicates if WizardController Template should be created
        /// </summary>
        private bool _ShowInWizard;
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the WizardShowViewStrategy class
        /// </summary>
        /// <param name="application">XafApplication Object</param>
        public WizardShowViewStrategy(XafApplication application)
            : base(application){
        }
        #endregion
        #region Properties
        /// <summary>
        /// Gets a value indicating whether the created View should be shown in an Wizard
        /// </summary>
        public bool ShowInWizard{
            get { return _ShowInWizard; }
        }
        #endregion
        #region Methods
        /// <summary>
        /// Gets called when a view is shown
        /// </summary>
        /// <param name="parameters">ShowView Parameters</param>
        /// <param name="showViewSource">ShowView Source</param>
        protected override void ShowViewCore(ShowViewParameters parameters, ShowViewSource showViewSource){
            _ShowInWizard = false;
            if (parameters.CreatedView is DetailView){
                DictionaryNode wizardNode = parameters.CreatedView.Info.FindChildNode("Wizard");
                _ShowInWizard = wizardNode != null && wizardNode.ChildNodes.Count > 0 &&
                                wizardNode.GetAttributeBoolValue("ShowInWizard", false);

                if (_ShowInWizard){
                    parameters.TargetWindow = TargetWindow.NewModalWindow;
                }

            }

            base.ShowViewCore(parameters, showViewSource);
        }
        #endregion
    }
}
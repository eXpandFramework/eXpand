//-----------------------------------------------------------------------
// <copyright file="WizardDetailViewForm.cs" created="20.01.2010" company="VenDoc Software GmbH">
//     Copyright (c) VenDoc Software GmbH. All rights reserved.
// </copyright>
// <author>Martin Praxmarer</author>
//-----------------------------------------------------------------------

namespace Xpand.ExpressApp.WizardUI.Win.Templates
{
    using System.Windows.Forms;
    using DevExpress.ExpressApp.Model;
    using DevExpress.ExpressApp.Utils;
    using DevExpress.ExpressApp.Win.Templates;
    using DevExpress.XtraWizard;

    /// <summary>
    /// DetailView Template for Wizard Control
    /// </summary>
    public partial class WizardDetailViewForm : XtraFormTemplateBase {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardDetailViewForm"/> class.
        /// </summary>
        public WizardDetailViewForm()
        {
            InitializeComponent();

            showRecordAfterCompletion.Text = CaptionHelper.GetLocalizedText("Texts", "WizardShowRecordAfterFinish");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the WizardControl
        /// </summary>
        public WizardControl WizardControl
        {
            get { return wizardControl; }
        }

        /// <summary>
        /// Gets a value indicating whether [show record after completion].
        /// </summary>
        /// <value>
        /// <c>true</c> if [show record after completion]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowRecordAfterCompletion
        {
            get
            {
                return showRecordAfterCompletion.Checked;
            }
        }

        #endregion

        #region Methods

        protected override IModelFormState GetFormStateNode()
        {
            if ((View == null) || (ModelTemplate == null))
            {
                return base.GetFormStateNode();
            }

            return ModelTemplate.FormStates[View.Id] ??
                                    ModelTemplate.FormStates.AddNode<IModelFormState>(View.Id);
        }

        #endregion
    }
}
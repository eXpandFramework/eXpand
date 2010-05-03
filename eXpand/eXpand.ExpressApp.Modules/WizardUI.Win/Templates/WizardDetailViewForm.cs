//-----------------------------------------------------------------------
// <copyright file="WizardDetailViewForm.cs" created="20.01.2010" company="VenDoc Software GmbH">
//     Copyright (c) VenDoc Software GmbH. All rights reserved.
// </copyright>
// <author>Martin Praxmarer</author>
//-----------------------------------------------------------------------

namespace eXpand.ExpressApp.WizardUI.Win.Templates
{
    using System.Collections.Generic;
    using System.Windows.Forms;
    using DevExpress.ExpressApp.Model;
    using DevExpress.ExpressApp.Templates;
    using DevExpress.ExpressApp.Utils;
    using DevExpress.ExpressApp.Win.Templates;
    using DevExpress.XtraEditors;
    using DevExpress.XtraEditors.Controls;
    using DevExpress.XtraWizard;

    /// <summary>
    /// DetailView Template for Wizard Control
    /// </summary>
    public partial class WizardDetailViewForm : XtraFormTemplateBase
    {
        #region Members

        /// <summary>
        /// PanelControl for the DetailView
        /// </summary>
        private PanelControl _ViewSitePanel = new PanelControl() { Dock = DockStyle.Fill, BorderStyle = BorderStyles.NoBorder };

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardDetailViewForm"/> class.
        /// </summary>
        public WizardDetailViewForm()
        {
            this.InitializeComponent();
            this.Initialize(null, new List<IActionContainer>(), new IActionContainer[0], this._ViewSitePanel, null);

            this.showRecordAfterCompletion.Text = CaptionHelper.GetLocalizedText("Texts", "WizardShowRecordAfterFinish");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the WizardControl
        /// </summary>
        public WizardControl WizardControl
        {
            get { return this.wizardControl; }
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
                return this.showRecordAfterCompletion.Checked;
            }
        }

        #endregion

        #region Methods

        protected override IModelFormState GetFormStateNode()
        {
            if ((base.View == null) || (base.ModelTemplate == null))
            {
                return base.GetFormStateNode();
            }
            IModelFormState state = base.ModelTemplate.FormStates[base.View.Id];
            if (state == null)
            {
                state = base.ModelTemplate.FormStates.AddNode<IModelFormState>(base.View.Id);
            }

            return state;
        }

        #endregion
    }
}
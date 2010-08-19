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
        private readonly PanelControl _ViewSitePanel = new PanelControl { Dock = DockStyle.Fill, BorderStyle = BorderStyles.NoBorder };

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardDetailViewForm"/> class.
        /// </summary>
        public WizardDetailViewForm()
        {
            InitializeComponent();
            Initialize(null, new List<IActionContainer>(), new IActionContainer[0], _ViewSitePanel, null);

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
            IModelFormState state = ModelTemplate.FormStates[View.Id] ??
                                    ModelTemplate.FormStates.AddNode<IModelFormState>(View.Id);

            return state;
        }

        #endregion
    }
}
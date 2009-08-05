//-----------------------------------------------------------------------
// <copyright file="WizardRibbonDetailViewForm.cs" created="02.06.2009" company="VenDoc Software GmbH">
//     Copyright (c) VenDoc Software GmbH. All rights reserved.
// </copyright>
// <author>Martin Praxmarer</author>
//-----------------------------------------------------------------------

namespace eXpand.ExpressApp.WizardUI.Win.Templates
{
    using System.Collections.Generic;
    using System.Windows.Forms;
    using DevExpress.ExpressApp.RibbonUI.Win;
    using DevExpress.ExpressApp.RibbonUI.Win.Templates;
    using DevExpress.ExpressApp.Templates;
    using DevExpress.XtraBars.Ribbon;
    using DevExpress.XtraEditors;
    using DevExpress.XtraEditors.Controls;
    using DevExpress.XtraWizard;

    /// <summary>
    /// RibbonDetailView with an WizardControl on it
    /// </summary>
    public partial class WizardRibbonDetailViewForm : RibbonFormTemplateBase
    {
        #region Members

        /// <summary>
        /// PanelControl for the DetailView
        /// </summary>
        private PanelControl _ViewSitePanel = new PanelControl() { Dock = DockStyle.Fill, BorderStyle = BorderStyles.NoBorder };

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the WizardRibbonDetailViewForm class
        /// </summary>
        /// <param name="ribbonTemplateInfo">TemplateInfo NodeWrapper</param>
        public WizardRibbonDetailViewForm(TemplateInfoNodeWrapper ribbonTemplateInfo)
            : base(ribbonTemplateInfo)
        {
            this.InitializeComponent();
            this.Initialize(
                new string[] { "ObjectsCreation", "RecordEdit", "View", "Print", "Export" },
                this._ViewSitePanel, 
                null);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the RibbonControl
        /// </summary>
        public RibbonControl RibbonControl
        {
            get { return this.ribbonControl; }
        }

        /// <summary>
        /// Gets the WizardControl
        /// </summary>
        public WizardControl WizardControl
        {
            get { return this.wizardControl; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds the Ribbon Bar
        /// </summary>
        protected override void BuildRibbon()
        {
            return;
        }

        /// <summary>
        /// Creates the Action Containers
        /// </summary>
        /// <returns>a List of ActionContainers</returns>
        protected override List<IActionContainer> CreateActionContainers()
        {
            return new List<IActionContainer>();
        }

        #endregion
    }
}
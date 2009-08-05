//-----------------------------------------------------------------------
// <copyright file="XafWizardPage.cs" created="02.06.2009" company="VenDoc Software GmbH">
//     Copyright (c) VenDoc Software GmbH. All rights reserved.
// </copyright>
// <author>Martin Praxmarer</author>
//-----------------------------------------------------------------------

namespace eXpand.ExpressApp.WizardUI.Win
{
    using DevExpress.ExpressApp;
    using DevExpress.XtraWizard;

    /// <summary>
    /// Extends the default WizardPage class
    /// </summary>
    public class XafWizardPage : WizardPage
    {
        #region Members

        /// <summary>
        /// DetailView Object
        /// </summary>
        private DetailView _View;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the DetailView
        /// </summary>
        public DetailView View
        {
            get
            {
                return this._View;
            }

            set
            {
                this._View = value;
            }
        }

        #endregion
    }
}

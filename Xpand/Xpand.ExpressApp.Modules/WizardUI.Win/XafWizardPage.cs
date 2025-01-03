﻿//-----------------------------------------------------------------------
// <copyright file="XafWizardPage.cs" created="02.06.2009" company="VenDoc Software GmbH">
//     Copyright (c) VenDoc Software GmbH. All rights reserved.
// </copyright>
// <author>Martin Praxmarer</author>
//-----------------------------------------------------------------------

using DevExpress.ExpressApp;
using DevExpress.XtraWizard;
using System.ComponentModel;

namespace Xpand.ExpressApp.WizardUI.Win{
    /// <summary>
    /// Extends the default WizardPage class
    /// </summary>
    public class XafWizardPage : WizardPage{
        #region Members
        #endregion
        
        /// <summary>
        /// Gets or sets the DetailView
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        #region Members
        #endregion
        #region Properties
        /// <summary>
        /// Gets or sets the DetailView
        /// </summary>
        public DetailView View { get; set; }
        #endregion
    }
}
/***************************************************************************
 
Copyright (c) Microsoft Corporation. All rights reserved.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace Xpand.VSIX.Options{
    public class OptionsPage : DialogPage{
        #region Fields

        private OptionsControl _optionsControl;

        #endregion Fields

        #region Methods

        public override void SaveSettingsToStorage(){
            _optionsControl.Save();
            base.SaveSettingsToStorage();
        }

        protected override void Dispose(bool disposing){
            if (disposing)
                if (_optionsControl != null){
                    _optionsControl.Dispose();
                    _optionsControl = null;
                }
            base.Dispose(disposing);
        }


        #endregion Methods

        #region Properties

        /// <summary>
        ///     Gets the window an instance of DialogPage that it uses as its user interface.
        /// </summary>
        /// <devdoc>
        ///     The window this dialog page will use for its UI.
        ///     This window handle must be constant, so if you are
        ///     returning a Windows Forms control you must make sure
        ///     it does not recreate its handle.  If the window object
        ///     implements IComponent it will be sited by the
        ///     dialog page so it can get access to global services.
        /// </devdoc>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected override IWin32Window Window => _optionsControl ?? (_optionsControl = new OptionsControl());


        #endregion Properties
    }
}
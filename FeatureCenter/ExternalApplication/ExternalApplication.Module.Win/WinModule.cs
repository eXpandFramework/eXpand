using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DevExpress.ExpressApp;

namespace ExternalApplication.Module.Win
{
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class ExternalApplicationWindowsFormsModule : ModuleBase
    {
        public ExternalApplicationWindowsFormsModule()
        {
            InitializeComponent();
        }
    }
}

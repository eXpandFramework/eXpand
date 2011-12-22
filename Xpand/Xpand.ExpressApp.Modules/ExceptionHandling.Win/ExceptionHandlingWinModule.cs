using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;

namespace Xpand.ExpressApp.ExceptionHandling.Win {
    [ToolboxBitmap(typeof(ExceptionHandlingWinModule))]
    [ToolboxItem(true)]
    public sealed partial class ExceptionHandlingWinModule : ExceptionHandlingModule {
        public event EventHandler<CustomHandleExceptionEventArgs> CustomHandleException;

        

        void OnCustomHandleException(CustomHandleExceptionEventArgs e) {
            EventHandler<CustomHandleExceptionEventArgs> handler = CustomHandleException;
            if (handler != null) handler(this, e);
        }

        public ExceptionHandlingWinModule() {
            InitializeComponent();
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (application is WinApplication)
                ((WinApplication)application).CustomHandleException += OnCustomHandleException;
        }

        private void OnCustomHandleException(object sender, CustomHandleExceptionEventArgs args) {
            var exception = args.Exception;
            var customHandleExceptionEventArgs = new CustomHandleExceptionEventArgs(exception);
            OnCustomHandleException(customHandleExceptionEventArgs);
            if (!(customHandleExceptionEventArgs.Handled))
                Log(exception);
        }

    }
}
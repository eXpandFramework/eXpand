using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;

namespace eXpand.ExpressApp.ExceptionHandling.Win
{
    public sealed partial class ExceptionHandlingWinModule : ExceptionHandlingModule
    {
        
        public ExceptionHandlingWinModule()
        {
            InitializeComponent();
        }

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            ((WinApplication)application).CustomHandleException += OnCustomHandleException;
        }

        private void OnCustomHandleException(object sender, CustomHandleExceptionEventArgs args)
        {
            var exception = args.Exception;
            Log(exception);
        }

    }
}
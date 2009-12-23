namespace eXpand.ExpressApp.WorldCreator.Win {
    public sealed partial class WorldCreatorWinModule : WorldCreatorModuleBase
    {
        public WorldCreatorWinModule()
        {
            InitializeComponent();
        }

        public override string GetPath() {
            return System.Windows.Forms.Application.ExecutablePath;
        }
    }
}
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.WorldCreator.Controllers
{
    public partial class ShowFormViewController : ShowNonPersistentObjectDetailViewFromNavigationControllerBase<UIContainerObject>
    {
        public ShowFormViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }


    }
}

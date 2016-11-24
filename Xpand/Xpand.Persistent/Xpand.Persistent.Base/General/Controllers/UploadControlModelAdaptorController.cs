using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.Web;
using Xpand.Persistent.Base.ModelAdapter;
using System.Linq;

namespace Xpand.Persistent.Base.General.Controllers {
    [ModelAbstractClass]
    public interface IModelPropertyEditorUploadControl:IModelPropertyEditor {
        IModelUploadControl UploadControl { get; }
    }

    [ModelNodesGenerator(typeof(ModelUploadControlAdaptersNodeGenerator))]
    public interface IModelUploadControlModelAdapters : IModelList<IModelUploadControlModelAdapter>, IModelNode {

    }

    public class ModelUploadControlAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelUploadControl, IModelUploadControlModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelUploadControlModelAdapter : IModelCommonModelAdapter<IModelUploadControl> {
    }

    [DomainLogic(typeof(IModelUploadControlModelAdapter))]
    public class ModelUploadControlModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelUploadControl> {
        public static IModelList<IModelUploadControl> Get_ModelAdapters(IModelUploadControlModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }

    public interface IModelUploadControl : IModelModelAdapter {
        IModelUploadControlModelAdapters ModelAdapters { get; }
    }

    public interface IASPxPropertyEditorUploadControlProvider {
        ASPxUploadControl ASPxUploadControl { get; }
        WebPropertyEditor WebPropertyEditor { get; }
    }

    public class ASPxPropertyEditorUploadControlProvider:IASPxPropertyEditorUploadControlProvider {
        readonly ASPxUploadControl _asPxUploadControl;
        readonly WebPropertyEditor _webPropertyEditor;

        public ASPxPropertyEditorUploadControlProvider(ASPxUploadControl asPxUploadControl, WebPropertyEditor webPropertyEditor) {
            _asPxUploadControl = asPxUploadControl;
            _webPropertyEditor = webPropertyEditor;
        }

        public ASPxUploadControl ASPxUploadControl {
            get { return _asPxUploadControl; }
        }

        public WebPropertyEditor WebPropertyEditor {
            get { return _webPropertyEditor; }
        }
    }
    public abstract class UploadControlPropertyEditorProviderController:ViewController<DetailView> {

        public event EventHandler<ASPxPropertyEditorUploadControlProviderArgs> UploadControlProviderCreated;

        protected virtual void OnUploadControlProviderCreated(ASPxPropertyEditorUploadControlProviderArgs e) {
            EventHandler<ASPxPropertyEditorUploadControlProviderArgs> handler = UploadControlProviderCreated;
            if (handler != null) handler(this, e);
        }
    }

    public class ASPxPropertyEditorUploadControlProviderArgs : EventArgs {
        readonly IASPxPropertyEditorUploadControlProvider _controlProvider;

        public ASPxPropertyEditorUploadControlProviderArgs(IASPxPropertyEditorUploadControlProvider controlProvider) {
            _controlProvider = controlProvider;
        }

        public IASPxPropertyEditorUploadControlProvider ControlProvider {
            get { return _controlProvider; }
        }
    }

    public interface IModuleSupportUploadControl {
         
    }
    public class UpdloadControlSynchronizer : ModelAdapter.ModelSynchronizer<ASPxUploadControl, IModelPropertyEditor>  {
        UpdloadControlSynchronizer(ASPxUploadControl component, IModelPropertyEditor modelNode) : base(component, modelNode) {
        }

        public UpdloadControlSynchronizer(IASPxPropertyEditorUploadControlProvider uploadControlProvider) : this(uploadControlProvider.ASPxUploadControl, (IModelPropertyEditor) uploadControlProvider.WebPropertyEditor.Model) {
        }

        protected override void ApplyModelCore() {
            ApplyModel(Model, Control, ApplyValues);
        }

        public override void SynchronizeModel() {
            throw new NotImplementedException();
        }
    }

    public class UploadControlModelAdaptorController:ModelAdapterController,IModelExtender {
        public UploadControlModelAdaptorController() {
            TargetViewType=ViewType.DetailView;
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var detailView = ((DetailView) View);
            var providerControllers = Frame.Controllers.Values.OfType<UploadControlPropertyEditorProviderController>();
            foreach (var providerController in providerControllers) {
                providerController.UploadControlProviderCreated+=ProviderControllerOnUploadControlProviderCreated;
            }
            var uploadControlProviders = detailView.GetItems<IASPxPropertyEditorUploadControlProvider>();
            ApplyModel(uploadControlProviders);
        }

        void ProviderControllerOnUploadControlProviderCreated(object sender, ASPxPropertyEditorUploadControlProviderArgs asPxPropertyEditorUploadControlProviderArgs) {
            ((UploadControlPropertyEditorProviderController) sender).UploadControlProviderCreated-=ProviderControllerOnUploadControlProviderCreated;
            ApplyModel(new[]{asPxPropertyEditorUploadControlProviderArgs.ControlProvider});
        }

        void ApplyModel(IEnumerable<IASPxPropertyEditorUploadControlProvider> uploadControlProviders) {
            foreach (var uploadControlProvider in uploadControlProviders) {
                new UpdloadControlSynchronizer(uploadControlProvider).ApplyModel();
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelPropertyEditor,IModelPropertyEditorUploadControl>();
            var builder = new InterfaceBuilder(extenders);
            var assembly = builder.Build(CreateBuilderData(), GetPath(typeof(ASPxUploadControl).Name));
            builder.ExtendInteface<IModelUploadControl, ASPxUploadControl>(assembly);
        }

        IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            yield return new InterfaceBuilderData(typeof(ASPxUploadControl)) {
                Act = info => (info.DXFilter(new List<Type>{ typeof (ASPxUploadControl) }, typeof(object)) ||
                     typeof(PropertiesBase).IsAssignableFrom(info.PropertyType))
            };
        }
    }
}

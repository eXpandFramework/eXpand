using System.IO;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;

namespace eXpand.ExpressApp.ModelDifference.Core
{
    
    public class ModelApplication 
    {
        readonly ModelApplicationBase _lastLayer;

        public ModelApplication(ModelApplicationBase lastLayer) {
//            Guard.CreateArgumentOutOfRangeException(true, "lastLayer.IsMaster");
//            Guard.CreateArgumentOutOfRangeException(null, "lastLayer.Master");
            _lastLayer = lastLayer;
//            CreateMasterLayer(lastLayer);
        }


        public ModelApplication() {
            _lastLayer = ModuleBase.ModelApplicationCreator.CreateModelApplication();
//            CreateMasterLayer(_lastLayer);
        }

        void CreateMasterLayer(ModelApplicationBase lastLayer) {
            
            var masterLayer = (ModelApplicationBase) ((ModelApplicationBase)ModuleBase.Application.Model).Master??(ModelApplicationBase) ((ISupportModelsManager) ModuleBase.Application).ModelsManager.CreateModelApplication();;
            masterLayer.AddLayer(lastLayer);
        }


        public ModelApplicationBase Model {
            get { return _lastLayer; }
        }


        public void ReadFromString(string aspect, string xml) {
            new ModelXmlReader().ReadFromString(Model, aspect, xml);
        }

        public void ReadFromStream(string aspect, Stream stream) {
            new ModelXmlReader().ReadFromStream(Model, aspect, stream);
        }

        public ModelApplication Clone() {
            return new ModelApplication(Model);
        }
    }
}

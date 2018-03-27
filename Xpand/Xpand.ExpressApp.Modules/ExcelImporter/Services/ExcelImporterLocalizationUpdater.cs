using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

namespace Xpand.ExpressApp.ExcelImporter.Services{
    public class ExcelImporterLocalizationUpdater:ModelNodesGeneratorUpdater<ModelLocalizationGroupGenerator>{
        public const string ExcelImport = "eXpand.ExcelImport";
        public const string ImportFailed = "ImportFailed";
        public const string Importing= "Importing";
        public const string ImportSucceded= "ImportSucceded";

        public override void UpdateNode(ModelNode node){
            if (node.Application.Localization[ExcelImport] == null){
                var localizationGroup = node.Application.Localization.AddNode<IModelLocalizationGroup>(ExcelImport);
                var localizationItem = localizationGroup.AddNode<IModelLocalizationItem>(Importing);
                localizationItem.Value =@"Importing";
                localizationItem = localizationGroup.AddNode<IModelLocalizationItem>(ImportFailed);
                localizationItem.Value =@"Importing failed in {0} rows out of {1}. Check the Failed Results Tab or press Save to commit.";
                localizationItem = localizationGroup.AddNode<IModelLocalizationItem>(ImportSucceded);
                localizationItem.Value =@"Importing succeeded for all {0} rows. Press Save to commit.";
            }
        }
    }
}
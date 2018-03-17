using DevExpress.ExpressApp.DC;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ExcelImporter {
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed partial class ExcelImporterModule : XpandModuleBase {
        public ExcelImporterModule() {
            InitializeComponent();
        }
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
        }


        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters){
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ExcelImporterLocalizationUpdater());
        }
    }

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

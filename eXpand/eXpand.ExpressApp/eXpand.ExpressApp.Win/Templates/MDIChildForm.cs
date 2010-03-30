using System.Collections.Generic;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.Win.Templates {
	public partial class MDIChildForm : XtraFormTemplateBase {
		private const string FrameTemplatesDetailViewForm = @"FrameTemplates\DetailViewForm";
		protected override DictionaryNode GetFormStateNode() {
			if(MdiParent == null) {
				if(View != null && TemplateInfo != null) {
					return TemplateInfo.GetChildNode(XtraFormTemplateBase.FormStateCustomizationNodeName, "ID", View.Id + "_FormState");
				}
				return base.GetFormStateNode();
			}
			return null;
		}
		protected override DictionaryNode GetBarCustomizationNode() {
			if(View != null && TemplateInfo != null) {
				return TemplateInfo.GetChildNode(XtraFormTemplateBase.MenuBarsCustomizationNodeName, "ID", View.Id + "_BarsCustomization");
			}
			else {
				return base.GetBarCustomizationNode();
			}
		}
		public void ResetSettings() {
			SetSettings(TemplateInfo);
		}
		public MDIChildForm() {
			InitializeComponent();

			MainMenuBar.Text = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "MainMenu", "MainMenu");
			barSubItemFile.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "FileSubMenu", "FileSubMenu");
			cObjectsCreation.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "ObjectsCreation", "ObjectsCreation");
			cFile.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "File", "File");
			cPrint.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "Print", "Print");
			cExport.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "Export", "Export");
			barSubItemEdit.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "EditSubMenu", "EditSubMenu");
			cRecordEdit.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "RecordEdit", "RecordEdit");
			barSubItemView.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "ViewSubMenu", "ViewSubMenu");
			cRecordsNavigation.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "RecordsNavigation", "RecordsNavigation");
			cView.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "View", "View");
			barSubItemTools.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "ToolsSubMenu", "Tools");
			cTools.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "Tools", "Tools");		
			cDiagnostic.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "Diagnostic", "Diagnostic");
			cOptions.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "Options", "Options");
			barSubItemHelp.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "HelpSubMenu", "HelpSubMenu");
			cAbout.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "About", "About");
			StandardToolBar.Text = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "MainToolbar", "MainToolbar");
			cFiltersSearch.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "Search", "Search");
			cFilters.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesDetailViewForm, "Filters", "Filters");
			List <IActionContainer> containers = new List<IActionContainer>();
			containers.AddRange(new IActionContainer[] {
											  cAbout, cTools, cFile, cObjectsCreation, cPrint, cExport, cRecordEdit, 
											  cRecordsNavigation, cFiltersSearch, cFilters,
											  cView, cDiagnostic, cOptions, cClose
										  });
			Initialize(barManager, containers,
				new IActionContainer[] { cObjectsCreation, cClose, cRecordEdit, cView, cPrint, cExport },
				viewSitePanel, null);
		}
		public override IActionContainer DefaultContainer {
			get { return cView; }
		}
	}
}

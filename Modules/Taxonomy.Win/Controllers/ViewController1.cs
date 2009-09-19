using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.Taxonomy.BaseObjects;
using System.Linq;

namespace eXpand.ExpressApp.Taxonomy.Win.Controllers{
    public partial class ViewController1 : ViewController
    {
        private SingleChoiceAction newObjectAction;

        public ViewController1()
        {
            InitializeComponent();
            RegisterActions(components);
            
            TargetViewType = ViewType.ListView;
            TargetObjectType = typeof(Term);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            newObjectAction = Frame.GetController<WinNewObjectViewController>().NewObjectAction;
            newObjectAction.Execute += NewObjectActionOnExecute;
            View.CurrentObjectChanged+=ViewOnCurrentObjectChanged;
        }
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            newObjectAction.Execute -= NewObjectActionOnExecute;
            View.CurrentObjectChanged -= ViewOnCurrentObjectChanged;
        }
        private void ViewOnCurrentObjectChanged(object sender, EventArgs args){
            var term1 = ((Term) View.CurrentObject);

            var structuralTerms = new XPQuery<StructuralTerm>(View.ObjectSpace.Session).Where(structuralTerm => structuralTerm.Level == term1.Level+1 && structuralTerm.Taxonomy.Key == term1.Taxonomy.Key);
            
            newObjectAction.Items.Clear();
            foreach (var term in structuralTerms){
                if (term.TypeOfObject != null){
                    Type type = ReflectionHelper.GetType(term.TypeOfObject);
                    var choiceActionItem = new ChoiceActionItem(type.Name, type);
                    newObjectAction.Items.Add(choiceActionItem);
                }
            }
            
        }

        private void NewObjectActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs args){
            ObjectSpace objectSpace = Application.CreateObjectSpace();
            ShowViewParameters parameters = args.ShowViewParameters;
            parameters.CreatedView = Application.CreateDetailView(objectSpace,
                                                                               objectSpace.CreateObject((Type) args.SelectedChoiceActionItem.Data));
            parameters.TargetWindow=TargetWindow.NewModalWindow;
            var controller = new DialogController();
            parameters.Controllers.Add(controller);
            controller.AcceptAction.Execute+=AcceptActionOnExecute;

        }

        private void AcceptActionOnExecute(object sender, SimpleActionExecuteEventArgs args){
            var taxonomyBaseObject = ((TaxonomyBaseObject) args.CurrentObject);
            ObjectSpace objectSpace = ObjectSpace.FindObjectSpace(taxonomyBaseObject);
            var newTerm = new Term(taxonomyBaseObject.Session){
                                                                  Key = taxonomyBaseObject.ToString(),
                                                                  Caption = taxonomyBaseObject.ToString(),
                                                                  ParentTerm = objectSpace.GetObject(((Term)View.CurrentObject))
                                                              };
            taxonomyBaseObject.ObjectInfos.Add(new TaxonomyBaseObjectInfo(taxonomyBaseObject.Session){Key = newTerm});
            
            objectSpace.CommitChanges();
        }
    }
}
using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.Taxonomy.BaseObjects;
using eXpand.Persistent.TaxonomyImpl;

namespace eXpand.ExpressApp.Taxonomy.Controllers{
    public abstract partial class TermViewController : ViewController{
        protected SingleChoiceAction newObjectAction;

        protected TermViewController(){
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated(){
            base.OnActivated();
            View.CurrentObjectChanged += ViewOnCurrentObjectChanged;
        }

        protected override void OnDeactivating(){
            base.OnDeactivating();
            View.CurrentObjectChanged -= ViewOnCurrentObjectChanged;
        }

        protected void ViewOnCurrentObjectChanged(object sender, EventArgs args){
            var term = ((Term) View.CurrentObject);

            if (term != null){
                IQueryable<StructuralTerm> structuralTerms = new XPQuery<StructuralTerm>(View.ObjectSpace.Session).Where(structuralTerm => structuralTerm.Level == term.Level + 1 && structuralTerm.Taxonomy.Key == term.Taxonomy.Key);
                if (newObjectAction != null){
                    newObjectAction.Items.Clear();
                    foreach (StructuralTerm structuralTerm in structuralTerms){
                        if (structuralTerm.TypeOfObject != null){
                            Type type = ReflectionHelper.GetType(structuralTerm.TypeOfObject);
                            var choiceActionItem = new ChoiceActionItem(type.Name, type);
                            newObjectAction.Items.Add(choiceActionItem);
                        }
                    }
                }
            }
        }

        protected void NewObjectActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs args){
            ObjectSpace objectSpace = Application.CreateObjectSpace();
            ShowViewParameters parameters = args.ShowViewParameters;
            parameters.CreatedView = Application.CreateDetailView(objectSpace,
                                                                  objectSpace.CreateObject((Type) args.SelectedChoiceActionItem.Data));
            parameters.TargetWindow = TargetWindow.NewModalWindow;
            var controller = new DialogController();
            parameters.Controllers.Add(controller);
            controller.AcceptAction.Execute += AcceptActionOnExecute;
        }

        private void AcceptActionOnExecute(object sender, SimpleActionExecuteEventArgs args){
            var taxonomyBaseObject = ((BaseObject) args.CurrentObject);
            ObjectSpace objectSpace = ObjectSpace.FindObjectSpace(taxonomyBaseObject);
            var newTerm = new Term(taxonomyBaseObject.Session){
                                                                  Key = taxonomyBaseObject.ToString(),
                                                                  Name = taxonomyBaseObject.ToString(),
                                                                  ParentTerm = objectSpace.GetObject(((TermBase) View.CurrentObject))
                                                              };

            objectSpace.CommitChanges();
        }
    }
}
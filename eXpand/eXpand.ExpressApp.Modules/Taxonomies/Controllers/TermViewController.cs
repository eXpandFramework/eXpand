using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.Taxonomies;
using eXpand.Xpo;

namespace eXpand.ExpressApp.Taxonomy.Controllers{
    public abstract partial class TermViewController : ViewController{
        protected SingleChoiceAction newObjectAction;
        private StructuralTerm selectedStrusturalTerm;

        protected TermViewController(){
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated(){
            base.OnActivated();
            
            View.CurrentObjectChanged += ViewOnCurrentObjectChanged;
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            PopulateValidNewActionItems(null);
        }
        protected override void OnDeactivating(){
            base.OnDeactivating();
            View.CurrentObjectChanged -= ViewOnCurrentObjectChanged;
        }

        protected void ViewOnCurrentObjectChanged(object sender, EventArgs args) {
            PopulateValidNewActionItems(((Term) View.CurrentObject));
        }

        private void PopulateValidNewActionItems(Term term){
            IQueryable<StructuralTerm> structuralTerms =
                term != null
                    ? new XPQuery<StructuralTerm>(View.ObjectSpace.Session).Where(structuralTerm => structuralTerm.Level == term.Level + 1 && structuralTerm.TaxonomyBase.Key == term.TaxonomyBase.Key)
                    : new XPQuery<StructuralTerm>(View.ObjectSpace.Session).Where(structuralTerm => structuralTerm.ParentTerm == null);
            

            if (newObjectAction != null) {
                PopulateChoiceAction(structuralTerms);
            }
        }

        private class ChoiceActionItem : DevExpress.ExpressApp.Actions.ChoiceActionItem{
            private readonly StructuralTerm structuralTerm;

            public ChoiceActionItem(string caption, object data,StructuralTerm structuralTerm) : base(caption, data){
                this.structuralTerm = structuralTerm;
            }

            public StructuralTerm StructuralTerm{
                get { return structuralTerm; }
            }
        }
        private void PopulateChoiceAction(IQueryable<StructuralTerm> structuralTerms) {
            newObjectAction.Items.Clear();
            foreach (StructuralTerm structuralTerm in structuralTerms){
                if (structuralTerm.TypeOfObject != null){
                    var choiceActionItem = new ChoiceActionItem(structuralTerm.TypeOfObject.Name, structuralTerm.TypeOfObject,structuralTerm);
                    newObjectAction.Items.Add(choiceActionItem);
                }
            }
            newObjectAction.Items.Add(new ChoiceActionItem("Term", typeof (Term), null));
        }

        protected void NewObjectActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs args){
            
            ObjectSpace objectSpace = Application.CreateObjectSpace();
            ShowViewParameters parameters = args.ShowViewParameters;
            selectedStrusturalTerm = ((ChoiceActionItem) args.SelectedChoiceActionItem).StructuralTerm;
            parameters.CreatedView = Application.CreateDetailView(objectSpace,
                                                                  objectSpace.CreateObject(selectedStrusturalTerm.TypeOfObject));

            parameters.TargetWindow = TargetWindow.NewModalWindow;
            var controller = new DialogController();
            parameters.Controllers.Add(controller);
            //controller.AcceptAction.Execute += AcceptActionOnExecute;
        }

        //private void AcceptActionOnExecute(object sender, SimpleActionExecuteEventArgs args){
        //    var taxonomyBaseObject = ((BaseObject) args.CurrentObject);
        //    ObjectSpace objectSpace = ObjectSpace.FindObjectSpace(taxonomyBaseObject);
        //    Term newTerm = GetTermForNewObject(taxonomyBaseObject, objectSpace, objectSpace.GetObject(selectedStrusturalTerm));
        //    createDefault(newTerm.StructuralTerm.Terms.Cast<StructuralTerm>(), newTerm);
        //    RelateNewObjectWithTaxonomyTree(taxonomyBaseObject, newTerm);

        //    objectSpace.CommitChanges();
        //}

        private void createDefault(IEnumerable<StructuralTerm> collection, Term newTerm){
            IEnumerable<StructuralTerm> structuralTerms = collection.Where(term => term.TypeOfObject==null);
            foreach (var structuralTerm in structuralTerms){
                var term = new Term(newTerm.Session){Key = structuralTerm.Key, Name = structuralTerm.Name, ParentTerm = newTerm, StructuralTerm = structuralTerm, TaxonomyBase = structuralTerm.TaxonomyBase};
                createDefault(structuralTerm.Terms.Cast<StructuralTerm>(),term);
            }
        }

        //private void RelateNewObjectWithTaxonomyTree(BaseObject taxonomyBaseObject, Term newTerm) {
           
        //}

        //private Term GetTermForNewObject(BaseObject taxonomyBaseObject, ObjectSpace objectSpace, StructuralTerm structuralTerm) {
        //    return new Term(taxonomyBaseObject.Session){
        //                                                   Key = taxonomyBaseObject.ToString(),
        //                                                   Name = taxonomyBaseObject.ToString(),
        //                                                   ParentTerm = View.CurrentObject != null ? (TermBase) objectSpace.GetObject(View.CurrentObject) : null,
        //                                                   TaxonomyBase = objectSpace.GetObject(structuralTerm.TaxonomyBase),
        //                                                   StructuralTerm = structuralTerm
        //                                               };
        //}

        
    }
}
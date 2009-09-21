using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using eXpand.ExpressApp.Taxonomy.BaseObjects;
using Foxhound.ExpressApp.Administration.BaseObjects;

namespace Foxhound.ExpressApp.Administration{
    public class Updater : ModuleUpdater{
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {}

        public override void UpdateDatabaseAfterUpdateSchema(){
            if (Session.FindObject<CompanyUnit>(null) == null){
                using (NestedUnitOfWork unit = Session.BeginNestedUnitOfWork()){
                        var taxonomy = new Taxonomy(unit){Key = "administration", Name = "Administration", Group = "Module Taxonomies"};
                        taxonomy.Terms.Add(new StructuralTerm(unit){Name = Taxonomy.DefaultStructuralRootNodeKey, Key = Taxonomy.DefaultStructuralRootNodeKey});
                        taxonomy.Terms.Add(new Term(unit){Name = Taxonomy.DefaultBrowsableViewRootNodeKey, Key = Taxonomy.DefaultBrowsableViewRootNodeKey});
                        taxonomy.ReevaluatePaths();
                        StructuralTerm structuralTerm = taxonomy.GetStructure("/Companies/Offices/Units/Employees"
                                                                              , new[]{typeof (Employee), typeof (CompanyUnit), typeof (CompanyUnit), typeof (CompanyUnit)});

                        //taxonomy.ReevaluatePaths();    
                    unit.CommitChanges();
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Xpo;
using eXpand.ExpressApp.DictionaryDifferenceStore.BaseObjects;
using eXpand.ExpressApp.SystemModule;
using System.Linq;
using eXpand.Persistent.Base;
using ListView=DevExpress.ExpressApp.ListView;
using XpoUserModelDictionaryDifferenceStore=
    eXpand.ExpressApp.DictionaryDifferenceStore.DictionaryStores.XpoUserModelDictionaryDifferenceStore;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.Controllers
{
    public partial class CombineXpoDictionaryDifferenceStoreViewController : BaseViewController
    {
        public CombineXpoDictionaryDifferenceStoreViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (XpoModelDictionaryDifferenceStore);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            combineSimpleAction.ConfirmationMessage = View is ListView?"Do you want to combine the selected models with the active application model?":null;
        }

        private void combineSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (View is ListView)
            {
                XpoModelDictionaryDifferenceStore activeStore =
                    XpoModelDictionaryDifferenceStoreBuilder.GetActiveStore(ObjectSpace.Session, DifferenceType.Model,
                                                                            Application.GetType().FullName);
                foreach (XpoModelDictionaryDifferenceStore selectedObject in e.SelectedObjects)
                    if (!ReferenceEquals(selectedObject, activeStore))
                        Combine(new List<XpoModelDictionaryDifferenceStore> {activeStore},
                                selectedObject,
                                (XpoUserModelDictionaryDifferenceStore) Application.Model.LastDiffStore);
                ObjectSpace.CommitChanges();
                
            }
            else
            {
                var controller = new DialogController();
                e.ShowViewParameters.Controllers.Add(controller);
                controller.AcceptAction.Execute+=AcceptActionOnExecute;
                e.ShowViewParameters.Context=TemplateContext.PopupWindow;
                e.ShowViewParameters.TargetWindow=TargetWindow.NewModalWindow;
                var collectionSource = new CollectionSource(Application.CreateObjectSpace(), typeof(XpoModelDictionaryDifferenceStore));
                collectionSource.Criteria["excludeCurrentObject"] = new BinaryOperator("Oid",((XpoModelDictionaryDifferenceStore) View.CurrentObject).Oid,
                                                                                       BinaryOperatorType.NotEqual);
                e.ShowViewParameters.CreatedView = Application.CreateListView(Application.FindListViewId(typeof(XpoModelDictionaryDifferenceStore)),
                                                                              collectionSource,
                                                                              true);
                                
            }
        }

        private void AcceptActionOnExecute(object sender, SimpleActionExecuteEventArgs args)
        {
            var objects = args.SelectedObjects;
            var store = (XpoModelDictionaryDifferenceStore) View.CurrentObject;
            
            
            Combine(objects.Cast<XpoModelDictionaryDifferenceStore>().ToList(), store,
                    (XpoUserModelDictionaryDifferenceStore) Application.Model.LastDiffStore);
            if (objects.Count>0)
            {
                ObjectSpace findObjectSpace = ObjectSpace.FindObjectSpace(objects[0]);
                findObjectSpace.CommitChanges();
            }
        }

        public static void Combine(List<XpoModelDictionaryDifferenceStore> objects, XpoModelDictionaryDifferenceStore store, XpoUserModelDictionaryDifferenceStore lastDiffStore)
        {
            foreach (var selectedStore in objects)
            {
                Dictionary dictionary = lastDiffStore.ApplicationModel.Clone();
                dictionary.AddAspect(selectedStore.Aspect, selectedStore.Model);
                dictionary.AddAspect(store.Aspect, store.Model);
                Dictionary diffs = dictionary.GetDiffs();

                string xml = (new DictionaryXmlWriter()).GetAspectXml(DictionaryAttribute.DefaultLanguage,
                                                                      diffs.RootNode);
                selectedStore.XmlContent = xml;
                selectedStore.Save();
            }
            
        }


//        public static void Combine(BaseObjects.XpoModelDictionaryDifferenceStore selectedObject, BaseObjects.XpoModelDictionaryDifferenceStore activeStore)
//        {
//            DictionaryNode activeNode = new DictionaryXmlReader().ReadFromString(activeStore.XmlContent);
//
//            DictionaryNode selectedNode = new DictionaryXmlReader().ReadFromString(selectedObject.XmlContent);
//            var selectedDictionary = new Dictionary(selectedNode);
//            selectedDictionary.AddAspect(selectedObject.Aspect, activeNode);
//
//            selectedObject.XmlContent = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
//                                        new DictionaryXmlWriter().GetAspectXml(
//                                            DictionaryAttribute.DefaultLanguage, selectedDictionary.RootNode);
//        }
    }
}
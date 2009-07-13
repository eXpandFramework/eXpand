using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.BaseImpl;
using eXpand.ExpressApp.DictionaryDifferenceStore.BaseObjects;
using System.Linq;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.Controllers
{
    public partial class CombineWithNewUserModelsViewController : ViewController
    {
        public CombineWithNewUserModelsViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (IAuthenticationStandardUser);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            if (View.CurrentObject != null && View.ObjectSpace.IsNewObject(View.CurrentObject))
            {
                View.ObjectSpace.ObjectSaving+=ObjectSpaceOnObjectSaving;
            }
        }

        private void ObjectSpaceOnObjectSaving(object sender, ObjectManipulatingEventArgs args)
        {
            if (args.Object==View.CurrentObject)
            {
                var user = ((User) args.Object);

                string propertyName = typeof (XpoUserModelDictionaryDifferenceStore).Name;
                var store =
                    (XpoUserModelDictionaryDifferenceStore)
                    user.GetMemberValue(propertyName);
                if (store== null)
                {
                    store=new XpoUserModelDictionaryDifferenceStore(user.Session);
                    XpoModelDictionaryDifferenceStoreBuilder.SetUp(store,Application.GetType().FullName);
                    user.SetMemberValue(propertyName,store);
                }
                List<XpoModelDictionaryDifferenceStore> stores =
                    user.Roles.SelectMany(role => role.Users).Select(user1 => user1.GetMemberValue(propertyName)).Cast
                        <XpoModelDictionaryDifferenceStore>().ToList();

                CombineXpoDictionaryDifferenceStoreViewController.Combine(stores,store ,(DictionaryStores.XpoUserModelDictionaryDifferenceStore)Application.Model.LastDiffStore);

                ObjectSpace objectSpace = ObjectSpace.FindObjectSpace(user);
                objectSpace.CommitChanges();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using eXpand.ExpressApp.Attributes;

namespace eXpand.ExpressApp.SystemModule
{
    public partial class HideFromNewMenuViewController : ViewController
    {
        public HideFromNewMenuViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            foreach (var type in GetHiddenTypes()){
                var attribute = View.ObjectTypeInfo.FindAttribute<HideFromNewMenuAttribute>();
                if (attribute != null)
                {
                    var controller = Frame.GetController<NewObjectViewController>();
                    Type typeInfo = type.Type;
                    ChoiceActionItem choiceActionItem = controller.NewObjectAction.Items.Where(item => item.Data==typeInfo).FirstOrDefault();
                    controller.NewObjectAction.Items.Remove(choiceActionItem);
                }    
            }
            
        }

        public List<ITypeInfo> GetHiddenTypes(){
            var objects = new List<ITypeInfo>();
            if (View.ObjectTypeInfo.FindAttribute<HideFromNewMenuAttribute>()!= null)
                objects.Add(View.ObjectTypeInfo);
            foreach (var typeInfo in View.ObjectTypeInfo.Descendants){
                if (typeInfo.FindAttribute<HideFromNewMenuAttribute>() != null)
                    objects.Add(typeInfo);
            }
            return objects;
        }
    }
}

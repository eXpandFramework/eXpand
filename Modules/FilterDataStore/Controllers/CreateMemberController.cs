using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using System.Linq;
using eXpand.ExpressApp.FilterDataStore.Core;

namespace eXpand.ExpressApp.FilterDataStore.Controllers
{
    public partial class CreateMemberController : ViewController
    {
        public CreateMemberController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            var applicationNodeWrapper = new ApplicationNodeWrapper(Application.Info);
            if (applicationNodeWrapper.Node.GetChildNode(FilterDataStoreModule.FilterDataStoreModuleAttributeName).GetAttributeBoolValue("Enabled"))
            {
                IEnumerable<ClassInfoNodeWrapper> classInfoNodeWrappers =
                    applicationNodeWrapper.BOModel.Classes.Where(
                        wrapper => wrapper.ClassTypeInfo.IsPersistent);
                foreach (ClassInfoNodeWrapper classInfo in classInfoNodeWrappers)
                {

                    
                    foreach (FilterProviderBase provider in FilterProviderManager.Providers)
                    {
                        if (classInfo.ClassTypeInfo.FindMember(provider.FilterMemberName) == null)
                            CreateMember(classInfo.ClassTypeInfo, provider);
                        ObjectSpace.Session.UpdateSchema(new[] { classInfo.ClassTypeInfo.Type });
                        XafTypesInfo.Instance.RefreshInfo(classInfo.ClassTypeInfo.Type);    
                    }
                    
                }
            }
            Active["RunOnce"] = false;
            
        }

        public static void CreateMember(ITypeInfo typeInfo , FilterProviderBase provider)
        {
            var attributes = new List<Attribute>{new BrowsableAttribute(false),new MemberDesignTimeVisibilityAttribute(
                                                                                                              false)};
            
            IMemberInfo member = typeInfo.CreateMember(provider.FilterMemberName, provider.FilterMemberType);
            if (provider.FilterMemberIndexed)
                attributes.Add(new IndexedAttribute());
            if (provider.FilterMemberSize!=SizeAttribute.DefaultStringMappingFieldSize)
                attributes.Add(new SizeAttribute(provider.FilterMemberSize));
            foreach (var attribute in attributes)
                member.AddAttribute(attribute);
        }
    }
}
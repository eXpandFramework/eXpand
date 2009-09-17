using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Foxhound.ExpressApp.Scheduler.Attributes;
using Foxhound.Persistent.BaseImpl;

namespace Foxhound.ExpressApp.Scheduler.BaseObjects.Events{
    public class ResourceHelper {
        private readonly BaseObject owner;
        private String resourceIds;
        public readonly IEnumerable<XPMemberInfo> ResourceProperties;
        
        private ResourceImpls resources;        
        public ResourceImpls Resources{
            get{
                if (resources == null){
                    UpdateResourceBindings();
                }
                return resources;
            }
        }

        public ResourceHelper(BaseObject owner) {
            this.owner = owner;
            ResourceProperties = owner.ClassInfo.Members.Where(info => info.HasAttribute(typeof (EventResourcePropertyAttribute)));
            ResourceTypes = new List<ITypeInfo>();
            if (ResourceProperties.Count() > 0) {
                foreach (XPMemberInfo info in ResourceProperties) {
                    if (info.IsCollection) {
                        ((XPCollection) (owner.GetMemberValue(info.Name))).CollectionChanged += OwnerResourcesCollectionChangeHandler;
                        ResourceTypes.Add(XafTypesInfo.Instance.FindTypeInfo(info.CollectionElementType.ClassType));
                    } else if (info.ReferenceType != null) {
                        owner.Changed += OnwerChangedHandler;
                        ResourceTypes.Add(XafTypesInfo.Instance.FindTypeInfo(info.ReferenceType.ClassType));
                    }
                }
            }
        }

        public List<ITypeInfo> ResourceTypes { get; private set; }

        public String ResourceId {
            get {
                if (resourceIds == null && !owner.IsLoading && !owner.Session.IsObjectsSaving) {
                    UpdateResourceBindings();
                }
                return resourceIds;
            }
            set {
                if (resourceIds != value && resourceIds != null) {
                    resourceIds = value;
                    UpdateResources();
                }
            }
        }

        private void OnwerChangedHandler(object sender, ObjectChangeEventArgs e) {
            if (ResourceProperties.Where(info => info.Name == e.PropertyName).SingleOrDefault() != null){
                if (!owner.Session.IsNewObject(owner)){
                    UpdateResourceBindings();
                }
            }
        }

        protected void OwnerResourcesCollectionChangeHandler(object sender, XPCollectionChangedEventArgs e) {
            if ((e.CollectionChangedType == XPCollectionChangedType.AfterAdd) || (e.CollectionChangedType == XPCollectionChangedType.AfterRemove)) {
                UpdateResourceBindings();
                owner.SetMemberValue("ResourceId", resourceIds);
            }
        }

        protected void UpdateResources() {
            resources = new ResourceImpls(resourceIds);
            IList allResourceIds = resources.Select(binding => binding.Id).ToList();
            foreach (XPMemberInfo info in ResourceProperties) {
                XPClassInfo resourcePropertyInfo = info.IsCollection ? info.CollectionElementType : info.ReferenceType;
                ICollection resourcesOfType = owner.Session.GetObjects(resourcePropertyInfo, new InOperator("Oid", allResourceIds), null, int.MaxValue, false, false);
                if (resources.Count > 0) {
                    if (info.IsCollection && resourcesOfType.Count > 0) {
                        var col = ((XPCollection) (owner.GetMemberValue(info.Name)));
                        try {
                            col.SuspendChangedEvents();
                            while (col.Count > 0) {
                                col.Remove(col[0]);
                            }

                            foreach (object resource in resourcesOfType) {
                                col.Add(resource);
                            }
                        } finally {
                            col.ResumeChangedEvents();
                        }
                    } else {
                        object resource = ((IList) resourcesOfType)[0];
                        if (resource != null) {
                            info.SetValue(this, resource);
                        }
                    }
                }
            }
            UpdateResourceBindings();
        }

        public virtual void UpdateResourceBindings() {
            resources = new ResourceImpls();
            foreach (XPMemberInfo info in ResourceProperties) {
                if (info.IsCollection) {
                    IEnumerable<ResourceImpl> bindings = ((XPCollection)(owner.GetMemberValue(info.Name))).Cast<IResource>().Select(res => new ResourceImpl(res));
                    if (bindings.Count() > 0) {
                        resources.AddRange(bindings);
                    }
                } else if (info.ReferenceType != null) {
                    var resource = (IResource) owner.GetMemberValue(info.Name);
                    if (resource != null) {
                        var binding = new ResourceImpl(resource);
                        resources.Add(binding);
                    }
                } else if (info.ReferenceType == null) {
                    IEnumerable<ResourceImpl> bindings = ((XPCollection)(owner.GetMemberValue(info.Name))).Cast<IResource>().Select(res => new ResourceImpl(res));
                    if (bindings.Count() > 0) {
                        resources.AddRange(bindings);
                    }
                }
            }

            resourceIds = resources.CreateXElement().ToString();
        }
    }
}
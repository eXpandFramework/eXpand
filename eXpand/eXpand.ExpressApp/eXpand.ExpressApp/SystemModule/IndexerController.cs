using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.Attributes;

namespace eXpand.ExpressApp.SystemModule
{
    public interface IModelIndexOptions : IModelNode
    {
        [Category("eXpand")]
        bool CreateIndexForAllMembers { get; set; }
    }

    public interface IModelSkipIndex : IModelNode
    {
        [Category("eXpand")]
        bool SkipIndexing { get; set; }
    }

    public partial class IndexerController : ViewController, IModelExtender
    {
        public event EventHandler<MemberInfoEventArgs> IndexAdding;

        protected virtual void InvokeIndexAdding(MemberInfoEventArgs e)
        {
            EventHandler<MemberInfoEventArgs> adding = IndexAdding;
            if (adding != null) adding(this, e);
        }

        public const string CreateIndexForAllMembers = "CreateIndexForAllMembers";
        public const string SkipIndexing = "SkipIndexing";
        public IndexerController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            if (((IModelIndexOptions)Application.Model.Options).CreateIndexForAllMembers)
            {
                Active["RunOnlyOnce"] = false;
                var indexAdded = new List<ITypeInfo>();
                foreach (var classInfoNodeWrapper in Application.Model.BOModel.Where
                    (wrapper => wrapper.TypeInfo.IsPersistent && !((IModelSkipIndex)wrapper).SkipIndexing))
                {
                    foreach (var propertyInfoNodeWrapper in classInfoNodeWrapper.AllMembers)
                    {
                        var memberInfo = classInfoNodeWrapper.TypeInfo.FindMember(propertyInfoNodeWrapper.Name);
                        if (memberInfo != null &&
                            (!((IModelSkipIndex)propertyInfoNodeWrapper).SkipIndexing &&
                             !Equals(memberInfo.MemberType, typeof (byte[])) && !memberInfo.IsAssociation))
                        {
                            var findAttribute = memberInfo.FindAttribute<SizeAttribute>();
                            if (findAttribute != null && findAttribute.Size == SizeAttribute.Unlimited)
                                continue;
                            if (memberInfo.FindAttribute<NonIndexedAttribute>() != null)
                                continue;
                            if (memberInfo.FindAttribute<IndexedAttribute>() == null)
                            {
                                var args = new MemberInfoEventArgs(memberInfo);
                                InvokeIndexAdding(args);
                                if (!args.Handled)
                                {
                                    if (!indexAdded.Contains(classInfoNodeWrapper.TypeInfo))
                                        indexAdded.Add(classInfoNodeWrapper.TypeInfo);
                                    memberInfo.AddAttribute(new IndexedAttribute());
                                }
                            }
                        }
                    }
                }

                if (indexAdded.Count>0)
                {
                    var classInfos = new List<XPClassInfo>();
                    foreach (var typeInfo in indexAdded)
                        classInfos.Add(ObjectSpace.Session.GetClassInfo(typeInfo.Type));
                    ObjectSpace.Session.DataLayer.UpdateSchema(false, classInfos.ToArray());
                }
            }
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelClass, IModelSkipIndex>();
            extenders.Add<IModelMember, IModelSkipIndex>();
            extenders.Add<IModelOptions, IModelIndexOptions>();
        }
    }
}
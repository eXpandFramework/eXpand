using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using System.Linq;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.SystemModule
{
    public interface IModelOptionIndexMembers : IModelNode
    {
        [Category("eXpand")]
        [Description("Automatically create database index for all members")]
        bool CreateIndexForAllMembers { get; set; }
    }

    public interface IModelMemberSkipIndex : IModelNode
    {
        [Category("eXpand")]
        bool SkipIndexing { get; set; }
    }

    public class IndexerController : ViewController, IModelExtender
    {

        public event EventHandler<MemberInfoEventArgs> IndexAdding;


        protected virtual void InvokeIndexAdding(MemberInfoEventArgs e)
        {
            EventHandler<MemberInfoEventArgs> adding = IndexAdding;
            if (adding != null) adding(this, e);
        }



        protected override void OnActivated()
        {
            base.OnActivated();
            if (((IModelOptionIndexMembers)Application.Model.Options).CreateIndexForAllMembers)
            {
                Active["RunOnlyOnce"] = false;
                var indexAdded = new List<ITypeInfo>();
                foreach (var classInfoNodeWrapper in Application.Model.BOModel.Where
                    (wrapper => wrapper.TypeInfo.IsPersistent && !((IModelMemberSkipIndex)wrapper).SkipIndexing))
                {
                    foreach (var propertyInfoNodeWrapper in classInfoNodeWrapper.AllMembers)
                    {
                        var memberInfo = classInfoNodeWrapper.TypeInfo.FindMember(propertyInfoNodeWrapper.Name);
                        if (memberInfo != null &&
                            (!((IModelMemberSkipIndex)propertyInfoNodeWrapper).SkipIndexing &&
                             !Equals(memberInfo.MemberType, typeof(byte[])) && !memberInfo.IsAssociation))
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

                if (indexAdded.Count > 0)
                {
                    ObjectSpace.Session.DataLayer.UpdateSchema(false, indexAdded.Select(typeInfo => ObjectSpace.Session.GetClassInfo(typeInfo.Type)).ToArray());
                }
            }
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelMember, IModelMemberSkipIndex>();
            extenders.Add<IModelOptions, IModelOptionIndexMembers>();
        }
    }
}
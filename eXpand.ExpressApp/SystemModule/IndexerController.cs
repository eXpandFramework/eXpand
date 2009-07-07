using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.Attributes;
using System.Linq;

namespace eXpand.ExpressApp.SystemModule
{
    public partial class IndexerController : ViewController
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
            if (Application.Info.GetChildNode("Options").GetAttributeBoolValue(CreateIndexForAllMembers))
            {
                Active["RunOnlyOnce"] = false;
                var indexAdded = new List<ITypeInfo>();
                foreach (var classInfoNodeWrapper in new ApplicationNodeWrapper(Application.Info).BOModel.Classes.Where(wrapper => wrapper.ClassTypeInfo.IsPersistent&&!wrapper.Node.GetAttributeBoolValue(SkipIndexing)))
                {
                    foreach (var propertyInfoNodeWrapper in classInfoNodeWrapper.Properties)
                    {
                        var memberInfo = classInfoNodeWrapper.ClassTypeInfo.FindMember(propertyInfoNodeWrapper.Name);
                        if (memberInfo != null &&
                            (!propertyInfoNodeWrapper.Node.GetAttributeBoolValue(SkipIndexing) &&
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
                                    if (!indexAdded.Contains(classInfoNodeWrapper.ClassTypeInfo))
                                        indexAdded.Add(classInfoNodeWrapper.ClassTypeInfo);
                                    memberInfo.AddAttribute(new IndexedAttribute());
                                }
                            }
                        }
                    }
                }
//                foreach (XPClassInfo typeInfo in dictionary.Classes)
////                    if (typeInfo.ClassType != null && typeInfo.ClassType.FullName.EndsWith("Contact"))
//                    if (typeInfo.IsPersistent)
//                        foreach (XPMemberInfo memberInfo in typeInfo.PersistentProperties)
//                        {
//                            if (!Equals(memberInfo.MemberType, typeof(byte[])) && !memberInfo.IsAssociation)
//                            {
//                                if (memberInfo.HasAttribute(typeof(SizeAttribute)) &&
//                                    ((SizeAttribute)memberInfo.FindAttributeInfo(typeof(SizeAttribute))).Size ==
//                                    SizeAttribute.Unlimited)
//                                    continue;
//                                if (memberInfo.HasAttribute(typeof(NonIndexedAttribute)))
//                                    continue;
//                                if (!memberInfo.HasAttribute(typeof(IndexedAttribute)))
//                                {
//                                    var args = new MemberInfoEventArgs(memberInfo);
//                                    InvokeIndexAdding(args);
//                                    if (!args.Handled)
//                                    {
//                                        if (!indexAdded.Contains(typeInfo))
//                                            indexAdded.Add(typeInfo);
//                                        memberInfo.AddAttribute(new IndexedAttribute());
//                                    }
//                                }
//                            }
//                        }
                if (indexAdded.Count>0)
                {
                    var classInfos = new List<XPClassInfo>();
                    foreach (var typeInfo in indexAdded)
                        classInfos.Add(ObjectSpace.Session.GetClassInfo(typeInfo.Type));
                    ObjectSpace.Session.DataLayer.UpdateSchema(false, classInfos.ToArray());
                }
            }
        }



        public override Schema GetSchema()
        {
            const string s = @"<Element Name=""Application"">;
                            <Element Name=""BOModel"">          
                                <Element Name=""Class"">
                                    <Attribute Name=""" + SkipIndexing + @""" Choice=""False,True""/>
                                    <Element Name=""Member"">
                                        <Attribute Name=""" + SkipIndexing + @""" Choice=""False,True""/>
                                    </Element>
                                </Element>
                            </Element>
                            <Element Name=""Options"">
                                <Attribute Name=""" + CreateIndexForAllMembers + @""" Choice=""False,True""/>
                            </Element>
                    </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(s));
        }

    }
}
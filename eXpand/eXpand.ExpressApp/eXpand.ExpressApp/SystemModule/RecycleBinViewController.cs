using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.Enums;

namespace eXpand.ExpressApp.SystemModule
{
    public partial class RecycleBinViewController : BaseViewController
    {
        public const string RecycleBin = "RecycleBin";

        public RecycleBinViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType = ViewType.ListView;

            recylcleBinSingleChoiceAction.Items.AddRange(new[]
                                                             {
                                                                 new ChoiceActionItem(RecycleBinEnum.Normal.ToString(),
                                                                                      RecycleBinEnum.Normal),
                                                                 new ChoiceActionItem(
                                                                     RecycleBinEnum.Deleted.ToString(),
                                                                     RecycleBinEnum.Deleted),
                                                                 new ChoiceActionItem(RecycleBinEnum.All.ToString(),
                                                                                      RecycleBinEnum.All)
                                                             });
        }


        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
//            return;
//            XPDictionary xpDictionary = XafTypesInfo.XpoTypeInfoSource.XPDictionary;
//            foreach (ReflectionClassInfo reflectionClassInfo in xpDictionary.Classes)
//            {
//                if (!reflectionClassInfo.ClassType.FullName.StartsWith("DevEx") && !reflectionClassInfo.IsAbstract &&
//                    !reflectionClassInfo.HasAttribute(NonPersistentAttribute.AttributeType))
//                {
//                    foreach (XPMemberInfo objectProperty in reflectionClassInfo.ObjectProperties)
//                    {
//                        if (reflectionClassInfo.GetMember("Deleted" + objectProperty.Name) == null)
//                            reflectionClassInfo.CreateMember("Deleted" + objectProperty.Name,
//                                                             objectProperty.ReferenceType.KeyProperty.MemberType,
//                                                             new BrowsableAttribute(false));
//                    }
//                }
//            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            Active[RecycleBin] = View.Info.GetAttributeBoolValue(RecycleBin, false);
            Active["EnableRecycleBin"] = Application.Info.GetAttributeBoolValue("EnableRecycleBin", false);
            if (Active.ResultValue)
            {
                restoreFormRecycleBinsimpleAction.Enabled["IsDeleted"] = false;
                recylcleBinSingleChoiceAction.SelectedIndex = 0;
//                ((MSSqlConnectionProvider) ((BaseDataLayer) (View.ObjectSpace.Session.DataLayer)).ConnectionProvider).
//                    RecycleBin =
//                    ((RecycleBinEnum) recylcleBinSingleChoiceAction.SelectedItem.Data);
//                View.CurrentObjectChanged += delegate
//                                                 {
//                                                     if (View.CurrentObject is BaseObject)
//                                                         restoreFormRecycleBinsimpleAction.Enabled["IsDeleted"] =
//                                                             ((BaseObject) View.CurrentObject).IsDeleted;
//                                                 };


                attachSessionEvents();
            }
        }


        ///<summary>
        ///
        ///<para>
        ///Returns the Schema extension which is combined with the entire Schema when loading the Application Model.
        ///
        ///</para>
        ///
        ///</summary>
        ///
        ///<returns>
        ///The <b>Schema</b> object that represents the Schema extension to be added to the application's entire Schema.
        ///
        ///</returns>
        ///
        public override Schema GetSchema()
        {
            const string s =
                @"<Element Name=""Application"">
                            <Attribute Name=""EnableRecycleBin"" Choice=""False,True""/>
                            <Element Name=""Views"">
                                <Element Name=""ListView"">;
                                    <Attribute Name=""" +
                RecycleBin +
                @""" Choice=""False,True""/>
</Element>
                            </Element>
                    </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(s));
        }

        private void attachSessionEvents()
        {
            return;
//            View.ObjectSpace.Session.ObjectDeleting += Session_OnObjectDeleting;
        }


//        private void Session_OnObjectDeleting(object sender, ObjectManipulationEventArgs e)
//        {
//            var xpBaseObject = (XPBaseObject) e.Object;
//            foreach (XPMemberInfo memberInfo in xpBaseObject.ClassInfo.ObjectProperties)
//            {
//                object value = memberInfo.GetValue(xpBaseObject);
//                if (value != null)
//                    xpBaseObject.SetMemberValue("Deleted" + memberInfo.Name,
//                                                memberInfo.ReferenceType.KeyProperty.GetValue(
//                                                    value));
//            }
//        }


//        /<summary>
//        /
//        /<para>
//        /Updates the Application Model.
//        /
//        /</para>
//        /
//        /</summary>
//        /
//        /<param name="dictionary">
//        /		A <b>Dictionary</b> object that provides access to the Application Model's nodes.
//        /
//        /            </param>
//        public override void UpdateModel(Dictionary dictionary)
//        {
//            base.UpdateModel(dictionary);
//            return;
//            DictionaryNodeCollection dictionaryNodeCollection =
//                dictionary.RootNode.GetChildNode(ViewsNodeWrapper.NodeName).GetChildNodes(ListView.InfoNodeName, false);
//            foreach (DictionaryNode dictionaryNode in dictionaryNodeCollection)
//            {
//                DictionaryNode ruleChildNode = dictionaryNode.GetChildNode("ConditionalFormatting").GetChildNode("Rule");
//                ruleChildNode.SetAttribute("ID", "deleteRule");
//                ruleChildNode.SetAttribute("Criteria", "IsDeleted==true");
//                DictionaryNode targetChildNode = ruleChildNode.GetChildNode("Target");
//                targetChildNode.SetAttribute("Name", ColorHighlightingTarget.Foreground.ToString());
//                targetChildNode.SetAttribute("Color", KnownColor.Red.ToString());
//            }
//        }


        private void recylcleBinSingleChoiceAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
//            ((MSSqlConnectionProvider) ((BaseDataLayer) (View.ObjectSpace.Session.DataLayer)).ConnectionProvider).
//                RecycleBin =
//                ((RecycleBinEnum) recylcleBinSingleChoiceAction.SelectedItem.Data);
            CollectionSourceBase collectionSourceBase = ((ListView) View).CollectionSource;
            collectionSourceBase.Reload();
            collectionSourceBase.ResetCollection();
        }

//        private void restoreFormRecycleBinsimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
//        {
//            IList objects = e.SelectedObjects;
//            if (objects.Count > 0)
//            {
//                restore(objects);
//                View.ObjectSpace.CommitChanges();
//            }
//        }

//        private void restore(IList objects)
//        {
//            foreach (BaseObject baseObject in objects)
//            {
//                foreach (XPMemberInfo memberInfo in baseObject.ClassInfo.CollectionProperties)
//                {
//                    if (memberInfo.HasAttribute(typeof (AggregatedAttribute)))
//
//                    {
//                        XPMemberInfo info = memberInfo.GetAssociatedMember();
//                        var operands = new BinaryOperator(
//                            "Deleted" +
//                            info.Name,
//                            new Guid(
//                                baseObject.
//                                    Oid.
//                                    ToString()));
//                        var collection =
//                            new XPCollection(baseObject.Session, memberInfo.CollectionElementType,
//                                             new GroupOperator(GroupOperatorType.Or,
//                                                               operands));
//
//                        restore(collection);
//                    }
//                }
//                foreach (XPMemberInfo memberInfo in baseObject.ClassInfo.ObjectProperties)
//                {
//                    baseObject.SetMemberValue(memberInfo.Name,
//                                              baseObject.Session.FindObject(memberInfo.ReferenceType,
//                                                                            new BinaryOperator("Oid",
//                                                                                               baseObject
//                                                                                                   .
//                                                                                                   GetMemberValue
//                                                                                                   ("Deleted" +
//                                                                                                    memberInfo
//                                                                                                        .
//                                                                                                        Name)),
//                                                                            true));
//                    baseObject.SetMemberValue("Deleted" + memberInfo.Name, null);
//                }
//
//                baseObject.SetMemberValue(GCRecordField.StaticName, null);
//            }
//        }
    }
}
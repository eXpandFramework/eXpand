using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using eXpand.Xpo.Attributes;
using eXpand.Xpo.Collections;

namespace eXpand.Xpo
{
    public delegate DBObject FindDelegate(Session session, string value);

    public delegate DBObject SelfReferenceFindMethodDelegate(Session session, string value);

    public interface ICustomFieldStore
    {
        object GetValue(XPMemberInfo member);
        void SetValue(XPMemberInfo member, object value);
    }

    public class DBObjectKey : XPBaseObject
    {
        #region Public Constractors
        public DBObjectKey()
        {
        }

        public DBObjectKey(Session session) : base(session)
        {
        }
        #endregion
        [Key(false)]
        public string TableName { get; set; }

        public int LastKey { get; set; }
    }

    [NonPersistent, MemberDesignTimeVisibility(false)]
    public abstract class DBObject : XPBaseObject, IDataErrorInfo, 
                                     ICustomFieldStore
    {
        public const string Type_Name = "TypeNameString";
        private readonly Hashtable customFields = new Hashtable();

        [Browsable(false)]
        public bool Encrypt
        {
            set { encrypt = value; }
        }

        [Browsable(false)]
        public string TypeNameString
        {
            get { return GetType().Name; }
        }

        [Browsable(false)]
        [NonPersistent]
        public bool TruncateStrings
        {
            get { return truncateStrings; }
            set { truncateStrings = value; }
        }

        [Browsable(false)]
        public bool Encrypted
        {
            set { encrypted = value; }
            get { return encrypted; }
        }
        #region ICustomFieldStore Members
        public object GetValue(XPMemberInfo member)
        {
            return member.IsCollection ? GetCollection(member.Name) : customFields[member];
        }

        public void SetValue(XPMemberInfo member, object value)
        {
            customFields[member] = value;
            OnChanged(member.Name);
        }
        #endregion
        #region IDataErrorInfo Members
        public string this[string columnName]
        {
            get { return dataErrorInfoProvider.GetColumnError(columnName); }
            set
            {
                dataErrorInfoProvider.SetColumnError(columnName, value);
                doActionForParentAggregatedAssociations(setAssociationErrors);
            }
        }

        [Browsable(false)]
        public string Error
        {
            get { return dataErrorInfoProvider.NoteError; }
        }
        #endregion
        #region IsDeleted implentation
        [Browsable(false)]
        public bool IsDeleted
        {
            get { return isDeleted; }
        }


//		/// <summary>
//		/// verifies that if KeyID=0 then a NullReferenceException is expected
//		/// </summary>
//		/// <returns></returns>
//		private bool nullReferenceException()
//		{
//			if (ClassInfo.GetPersistentMember(ClassInfo.KeyProperty.Name).GetValue(this).Equals(0))
//				try
//				{
//					StackHelper stackHelper = new StackHelper();
//					bool inStack = stackHelper.IsMethodInStack("ProcessCollectionsMember", new StackTrace());
//					if (!inStack)
//					{
//						base.Delete();
//						throw new Exception("A NullReferenceException is expected");
//					}
//					else
//						return true;
//				}
//				catch (NullReferenceException)
//				{
//					return true;
//				}
//			else
//				return false;
//		}
        #endregion
        #region CloneRR implementation
        [Browsable(false)]
        public bool IsSelfReference
        {
            get { return isSelfReference; }
        }

        public DBObject SetProperties(DBObject dbObject, Session session, bool m_disableKeySet, ArrayList hashtables)
        {
            var clone = (DBObject) dbObject.ClassInfo.CreateObject(session);
            foreach (XPMemberInfo m in dbObject.ClassInfo.PersistentProperties)
            {
                if (m is ServiceField || (m.IsKey && !m_disableKeySet))
                    continue;
                object val = m.GetValue(dbObject);
                if (hashtables != null && typeof (DBObject).IsAssignableFrom(m.MemberType))
                {
                    bool notFound = true;
                    foreach (Hashtable hashtable in hashtables)
                    {
                        if (val != null && hashtable.ContainsKey(val))
                        {
                            val = hashtable[val];
                            notFound = false;
                        }
                    }
                    if (val != null && notFound)
                    {
                        //						if (!setSelfReferenceProperty((DBObject)val,session,m_disableKeySet,m,hashtables))
                        throw new ArgumentException("An object of type " + m.MemberType.Name + " not found for member " +
                                                    m.Name +
                                                    Environment.NewLine +
                                                    "DbObjectType=" + dbObject.TypeNameString);
                    }
                }
                m.SetValue(clone, val);
            }
            clone.disableKeySet = m_disableKeySet;
            return clone;
        }

        //		private bool setSelfReferenceProperty(DBObject dbObject, Session session, bool disableKeySet, XPMemberInfo m, ArrayList hashtables)
        //		{
        //			if (m.MemberType.Equals(dbObject.GetType()))
        //			{
        //				SetProperties(dbObject, session, disableKeySet, hashtables);
        //				return true;
        //			}
        //			return false;
        //		}

//        private IXPSimpleObject CloneRR(IXPSimpleObject source)
//        {
//            if (source == null)
//                return null;
//            IXPSimpleObject clone = (IXPSimpleObject) source.ClassInfo.CreateObject(source.Session);
//            foreach (XPMemberInfo m in source.ClassInfo.PersistentProperties)
//            {
//                if (m is ServiceField)
//                    continue;
//                object val;
//                if (m.ReferenceType != null && m.HasAttribute(typeof (AggregatedAttribute)))
//                    val = CloneRR((IXPSimpleObject) m.GetValue(source));
//                else
//                    val = m.GetValue(source);
//
//                m.SetValue(clone, val);
//            }
//            foreach (XPMemberInfo m in source.ClassInfo.CollectionProperties)
//            {
//                if (m.HasAttribute(typeof (AggregatedAttribute)))
//                {
//                    DBCollection col = (DBCollection) m.GetValue(clone);
//                    foreach (IXPSimpleObject obj in (DBCollection) m.GetValue(source))
//                        col.Add(CloneRR(obj));
//                }
//            }
//            return clone;
//        }

//        public object CloneRR()
//        {
//            return CloneRR(this);
//        }
        #endregion
        #region empty  virtual methods
//        protected virtual void AfterSave()
//        {
//        }
        #endregion
        [Browsable(false)]
        public DataRow GetDataRow(DataTable dataTable)
        {
            DataRow dataRow = dataTable.NewRow();
            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                try
                {
                    object value = ReflectorHelper.GetPropertyInfoValue(dataColumn.ColumnName, this);
                    if (value is DBObject)
                        value = ((DBObject) value).ClassInfo.KeyProperty.GetValue(value);
                    dataRow[dataColumn.ColumnName] = value;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return dataRow;
        }

        public CriteriaOperator GetClassTypeFilter()
        {
            return DBCollection.GetClassTypeFilter(GetType(), Session);
        }

        public static string GetFileData(Session session, DBObject document, string propertyName)
        {
            #region getField name
            XPMemberInfo member = document.ClassInfo.GetMember(propertyName);
            if (member.HasAttribute(typeof (PersistentAttribute)))
            {
                var attribute =
                    (PersistentAttribute) member.GetAttributeInfo(typeof (PersistentAttribute));
                propertyName = attribute.MapTo;
            }
            #endregion
            #region getTableName
            string tableName = document.GetType().Name;
            object[] attributes = document.GetType().GetCustomAttributes(typeof (MapToAttribute), true);
            if (attributes.Length > 0)
                tableName = ((MapToAttribute) attributes[0]).MappingName;
            #endregion
            IDbCommand command = session.Connection.CreateCommand();
            command.CommandText = "SELECT " + propertyName + " FROM " + tableName + " WHERE " +
                                  document.ClassInfo.KeyProperty.Name + "=" +
                                  document.ClassInfo.KeyProperty.GetValue(document);
            object reader = command.ExecuteScalar();
            var fileData = reader as string;
            command.Dispose();

            //            session.Connection.Close();
            return fileData;
        }

        private void doActionForParentAggregatedAssociations(actionForParentAssociations actionForParentAssociations)
        {
            foreach (XPMemberInfo xpMemberInfo in ClassInfo.PersistentProperties)
            {
                if ((xpMemberInfo.HasAttribute(typeof (AssociationAttribute))))
                {
                    var associationAttribute =
                        (AssociationAttribute) xpMemberInfo.GetAttributeInfo(typeof (AssociationAttribute));
                    string collection = getAssociatedCollection(associationAttribute.Name, xpMemberInfo.ReferenceType);
                    if (collection != null)
                    {
                        XPMemberInfo member = Session.GetClassInfo(xpMemberInfo.MemberType).GetMember(collection);

//                        PropertyInfo property = xpMemberInfo.MemberType.GetProperty(collection);
//                        object[] attributes = property.GetCustomAttributes(typeof (AggregatedAttribute), false);
                        if (member.HasAttribute(typeof (AggregatedAttribute)))
                        {
//                            attributes = property.GetCustomAttributes(typeof (NonErrorSerializeAttribute), false);
//                            if (member.HasAttribute(typeof (NonErrorSerializeAttribute)))
//                                actionForParentAssociations.Method.Invoke(this,
//                                                                          new object[]
//                                                                              {xpMemberInfo, associationAttribute});
                        }
                    }
                }
            }
        }

//        ///<returns>
//        ///
//        ///<para>
//        ///An <see cref="T:System.Xml.Schema.XmlSchema" /> object that represents the XML schema.
//        ///</para>
//        ///
//        ///</returns>
//        ///
//        public XmlSchema GetSchema()
//        {
//            //			throw new NotImplementedException();
//            return null;
//        }

//        ///<summary>
//        ///
//        ///<para>
//        ///Converts an XML document into an object using the specified reader.
//        ///</para>
//        ///
//        ///</summary>
//        ///
//        ///<param name="reader">The <see cref="T:System.Xml.XmlReader" /> used to read the XML document.</param>
//        public void ReadXml(XmlReader reader)
//        {
//            //			return null;
//        }

//        ///<summary>
//        ///
//        ///<para>
//        ///Converts the serializable members of an object into an XML document.
//        ///</para>
//        ///
//        ///</summary>
//        ///
//        ///<param name="writer">The XmlWriter used to write the XML-document instance.</param>
//        public void WriteXml(XmlWriter writer)
//        {
//            foreach (
//                PropertyInfo propertyInfo in
//                    GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
//            {
//                if (propertyInfo.GetIndexParameters().Length == 0)
//                {
//                    object value = propertyInfo.GetValue(this, null);
//                    writer.WriteElementString(propertyInfo.Name, value == null ? null : value.ToString());
//                }
//            }
//        }

        public static DBCollection GetAll(Session session, Type dbObjectType, BinaryOperator binaryOperator)
        {
            return new DBCollection(session, dbObjectType, binaryOperator);
        }

//        public static void AddMember(Session session, string memberName, Type memberType, Type declaringType,
//                                     IndexedAttribute indexedAttribute,
//                                     NotNullAbleValueAttribute notNullAbleValueAttribute, SizeAttribute sizeAttribute,
//                                     DelayedAttribute delayedAttribute, AssociationAttribute associationAttribute)
//        {
//            XPClassInfo ci =
//                session.GetClassInfo(declaringType);
//            if (ci.GetMember(memberName) == null)
//                new CustomFieldMember(ci, memberName, memberType,
//                                      new Attribute[]
//                                          {
//                                              indexedAttribute, notNullAbleValueAttribute, sizeAttribute, delayedAttribute,
//                                              associationAttribute
//                                          });
//        }

        public object GetValue(string memberName)
        {
            return ClassInfo.GetMember(memberName).GetValue(this);
        }

        public void SetValue(string memberName, object value)
        {
            ClassInfo.GetMember(memberName).SetValue(this, value);
        }

//        public XPCollection GetCustomCollection(string name)
//        {
//            return getCollectionNoSaveAggregation(name);
//        }
        public new virtual void Delete()
        {
            var collectionsToDelete = new ArrayList();
            string unableToDelete = null;
            foreach (XPMemberInfo memberInfo in ClassInfo.CollectionProperties)
            {
                if (memberInfo.HasAttribute(typeof (AggregatedDelete)))
                    collectionsToDelete.Add(memberInfo.GetValue(this));
                else
                {
                    var collection = (XPBaseCollection) memberInfo.GetValue(this);
                    Type type = collection.GetObjectClassInfo().ClassType;
                    if (DBCollection.GetCount(collection.Session, type, collection.Criteria) > 0)
                    {
                        unableToDelete = "Conflicts found in table " +
                                         collection.Session.GetClassInfo(type).TableName + " (";
                        foreach (DBObject dbObject in collection)
                            unableToDelete += dbObject.ClassInfo.KeyProperty.GetValue(dbObject) + ",";
                        unableToDelete = unableToDelete.TrimEnd(',') + ")";
                    }
                }
            }
//            if (unableToDelete != null)
//                throw new DALException(unableToDelete, GetType());
            foreach (IList list in collectionsToDelete)
                DBCollection.DeleteAllSuper(list, constrainsDisabled);

            base.Delete();
        }

        /// <summary>
        /// Any code should be executed befaore base.Saving()
        /// </summary>
        protected override void OnSaving()
        {
            base.OnSaving();
            GetKey();
            doEncryption();
            if (constrainsDisabled)
            {
                PropertyInfo[] propertyInfos = ReflectorHelper.GetPropertiesAssignAbleFrom(GetType(), typeof (DBObject));
                foreach (PropertyInfo info in propertyInfos)
                {
                    if (info.GetSetMethod() != null)
                    {
                        var value = (DBObject) info.GetValue(this, null);
                        if (value != null) value.ConstrainsDisabled = true;
                    }
                }
            }
            if (isDeleted)
                return;
            if (!constrainsDisabled && !HasErrors())
            {
                //                GetKey();
//                try
//                {
                trucateStrings();
//                    Save();

//                }
//                catch (Exception e)
//                {
//                    throw new Exception("{" + ClassInfo.KeyProperty.GetValue(this) + "}" + GetType().FullName, e);
//                }
//                isChanged = false;
            }
            else if (constrainsDisabled)
            {
                //                GetKey();
                trucateStrings();
//                Save();
//                isChanged = false;
            }
//            else
//                throw new DALException(dataErrorInfoProvider.NoteError, GetType(), this);
        }

//        public virtual void Save()
//        {
//            
////            EndEdit();
//            
//            OnSaving();
//            if (isDeleted)
//                Save();
//            else if (!constrainsDisabled && !HasErrors())
//            {
//                
////                GetKey();
//                try
//                {
//                    trucateStrings();
//                    Save();
//                    saveAggregated(this);
//                }
//                catch (Exception e)
//                {
//                    throw new Exception("{" + ClassInfo.KeyProperty.GetValue(this) + "}" + GetType().FullName, e);
//                }
//                isChanged = false;
//            }
//            else if (constrainsDisabled)
//            {
////                GetKey();
//                trucateStrings();
//                Save();
//                isChanged = false;
//            }
//            else
//                throw new DALException(dataErrorInfoProvider.NoteError, GetType(), this);
//            //            if (dbObjectKey != null)
//            //            {
//            //                dbObjectKey.LastKey += 1;
//            //                dbObjectKey.Save();
//            //            }
//            
////            AfterSave();
//            //            Reload();
//        }

        protected override void OnSaved()
        {
            base.OnSaved();
            isChanged = false;
            saveAggregated(this);
            DoDecryption();
        }

        private void trucateStrings()
        {
            if (truncateStrings)
            {
                foreach (XPMemberInfo xpMemberInfo in ClassInfo.PersistentProperties)
                {
                    if (xpMemberInfo.MemberType == typeof (string))
                    {
                        var value = xpMemberInfo.GetValue(this) as string;
                        if (value != null)
                        {
//                            object[] attributes = xpMemberInfo.GetCustomAttributes(typeof (SizeAttribute),true);
                            if (xpMemberInfo.HasAttribute(typeof (SizeAttribute)))
                            {
                                int size = ((SizeAttribute) xpMemberInfo.GetAttributeInfo(typeof (SizeAttribute))).Size;
                                if (size > -1 && value.Length > size)
                                    value = value.Substring(0, size - 1);
                            }
                            else if (value.Length > 99)
                                value = value.Substring(0, 99);
                            xpMemberInfo.SetValue(this, value);
                        }
                    }
                }
            }
        }
        #region getAssociatedCollection
        /// <summary>
        /// searches the persistent properties of a <see cref="DBObject"/> for a member that implements <see cref="XPCollection"/> and has an <see cref="AssociationAttribute"/>
        /// </summary>
        /// <param name="associationName"></param>
        /// <returns></returns>
        ///<param name="classInfo"></param>
        private static string getAssociatedCollection(string associationName, XPClassInfo classInfo)
        {
            foreach (XPMemberInfo xpMemberInfo in classInfo.Members)
            {
                if (xpMemberInfo.IsCollection)
                {
                    if (xpMemberInfo.HasAttribute(typeof (AssociationAttribute)))
                    {
                        var associationAttribute =
                            (AssociationAttribute) xpMemberInfo.GetAttributeInfo(typeof (AssociationAttribute));
                        if (associationAttribute.Name == associationName)
                            return xpMemberInfo.Name;
                    }
                }
            }
            return null;
//            throw new AssociatedCollectionMissing(associationName, classInfo);
        }

        /// <summary>
        /// searches the persistent properties of a DBObject for a member that implements ICollection and has an Attribute of type Association
        /// </summary>
        /// <param name="associationName"></param>
        /// <param name="dbObject"></param>
        /// <returns></returns>
        public static string GetAssociatedCollection(string associationName, DBObject dbObject)
        {
            return getAssociatedCollection(associationName, dbObject.ClassInfo);
        }
        #endregion
        #region private fields
        private readonly DataErrorInfoProvider dataErrorInfoProvider;
        private readonly ArrayList excludedConstrains = new ArrayList();
        private readonly bool isLoaded;
//        private Hashtable collectionsNoSaveAggregation;
        private bool constrainsDisabled;
        private bool disableKeySet;
        private bool encrypt;
        private bool encrypted;
        private bool isChanged;
        private bool isDeleted;
        private bool isSelfReference;
        private bool truncateStrings;

        [Browsable(false)]
        public ArrayList ExcludedConstrains
        {
            get { return excludedConstrains; }
        }

        private delegate void actionForParentAssociations(
            XPMemberInfo xpMemberInfo, AssociationAttribute associationAttribute
            );
        #endregion
        #region constructors
        protected DBObject(Session session, XPClassInfo classInfo) : base(session, classInfo)
        {
        }

        protected DBObject(Session session) : base(session)
        {
            dataErrorInfoProvider = new DataErrorInfoProvider(this);
            Changed += DBObject_Changed;
        }


        protected DBObject()
        {
            dataErrorInfoProvider = new DataErrorInfoProvider(this);
        }
        #endregion
        #region NoSaveAggreation
//        private XPCollection getCollectionNoSaveAggregation(string name)
//        {
//            return GetCollection(name);
////            XPMemberInfo info1 = GetCollectionProperty(name);
////            XPCollection collection1 = CollectionsNoSaveAggregation[info1] as XPCollection;
////            if (collection1 == null)
////            {
////                collection1 = CreatePropertyCollectionNoSaveAggregation(this, info1);
////                if (!CollectionsNoSaveAggregation.ContainsKey(info1))
////                    CollectionsNoSaveAggregation.Add(info1, collection1);
////                else
////                    collectionsNoSaveAggregation[info1] = collection1;
////            }
////            return collection1;
//        }

        public XPCollection getCollectionNoSaveAggregation(MethodBase methodBase)
        {
            return GetCollection(methodBase.Name.Replace("get_", ""));
        }

/*
        private XPMemberInfo GetCollectionProperty(string name)
        {
            XPMemberInfo info1 = ClassInfo.GetMember(name);
            if (info1 == null)
            {
                throw new PropertyMissingException(ClassInfo.FullName, name);
            }
            return info1;
        }
*/

//        /// <summary>
//        /// Stores all XPCollections marked with NoSaveAggregation
//        /// </summary>
//        private Hashtable CollectionsNoSaveAggregation
//        {
//            get
//            {
//                if (collectionsNoSaveAggregation == null)
//                    collectionsNoSaveAggregation = new Hashtable();
//
//                return collectionsNoSaveAggregation;
//            }
//        }

        public static XPCollection CreatePropertyCollectionNoSaveAggregation(DBObject owner,
                                                                             XPMemberInfo collectionProperty)
        {
            if (collectionProperty.IsManyToMany)
//                return new XPManyToManyNoSaveAggregation(owner, collectionProperty);
                throw new NotImplementedException("IsManyToMany on CreatePropertyCollectionNoSaveAggregation on " +
                                                  owner.GetType().FullName);
            return new XPCollection(owner.Session, owner, collectionProperty);
//            return new XPOneToManyCollectionNoSaveAggregation(owner, collectionProperty.Name);
//            AssociationAttribute attribute = (AssociationAttribute) collectionProperty.GetAttributeInfo(typeof(AssociationAttribute));
//
//            Type typeOfCollection = FindTypeOfCollection(attribute);
////            Type[] types = new Type[] {owner.GetType()};
////            Type builderType = GetBuilderType(typeOfCollection);
////            MethodInfo methodInfo = builderType.GetMethod("GetAll", types);
////            if (methodInfo == null)
////                throw new MethodNotFoundException(builderType, "GetAll", types);
//            XPOneToManyCollectionNoSaveAggregation xpOneToManyCollectionNoSaveAggregation = new XPOneToManyCollectionNoSaveAggregation(owner, collectionProperty.Name);
//            PropertyInfo[] propertyInfos = ReflectorHelper.GetDecoratedProperties(typeOfCollection, typeof(AssociationAttribute));
//            string propertyName = null;
//            foreach (PropertyInfo info in propertyInfos)
//                if (((AssociationAttribute) info.GetCustomAttributes(typeof (AssociationAttribute), true)[0]).Name ==
//                    attribute.Name)
//                    propertyName = info.Name;
//
//            DBCollection dbCollection = new DBCollection(owner.Session,typeOfCollection,new BinaryOperator(propertyName,owner));
//            foreach (DBObject dbObject in dbCollection)
//            {
//                if (xpOneToManyCollectionNoSaveAggregation.Lookup(dbObject.ClassInfo.KeyProperty.GetValue(dbObject)) ==null)
//                    xpOneToManyCollectionNoSaveAggregation.Add(dbObject);
//            }
//            
//            return xpOneToManyCollectionNoSaveAggregation;
        }
        #endregion
        #region base Overrides
        [Browsable(false)]
        public bool IsNewObject
        {
            get { return Session.IsNewObject(this); }
        }

        [Browsable(false)]
        [NonPersistent]
        public bool ConstrainsDisabled
        {
            get { return constrainsDisabled; }
            set
            {
                constrainsDisabled = value;
//                setConstrains(this, value);
            }
        }

        ///<summary>
        ///
        ///<para>
        ///Provided for customization of the object creation logic.
        ///
        ///</para>
        ///
        ///</summary>
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            isSelfReference =
                Convert.ToBoolean(ReflectorHelper.GetPropertiesAssignAbleFrom(GetType(), GetType()).Length);
            truncateStrings = true;
        }


        public void DoDecryption()
        {
            if (!encrypted)
                return;
            PropertyInfo[] properties = ReflectorHelper.GetDecoratedProperties(GetType(), typeof (Encrypt));
            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType.Equals(typeof (string)))
                {
                    var value = property.GetValue(this, null) as string;
//                    if (value + "" != "")
//                        property.SetValue(this, Encryption.Decrypt(value), null);
                }
            }
            encrypted = false;
        }

//        protected override void EndLoad()
//        {
//            base.EndLoad();
//            isLoaded = true;
//        }

//        public override void Reload()
//        {
//            CollectionsNoSaveAggregation.Clear();
//            base.Reload();
//        }

        protected override void BeginEdit()
        {
        }

        protected override void EndEdit()
        {
        }

        protected override void CancelEdit()
        {
        }


        //        protected override void AfterLoad()
//        {
//            base.AfterLoad();
//            isChanged = false;
//            DoDecryption();
//        }

        public void Save(bool disableKeySet)
        {
            this.disableKeySet = disableKeySet;
            Save();
        }

        protected override void OnLoaded()
        {
//            CollectionsNoSaveAggregation.Clear();
            base.OnLoaded();
            isChanged = false;
            DoDecryption();
        }


//        public override void Save()
//        {
//            doEncryption();
//            EndEdit();
//            if (constrainsDisabled)
//            {
//                PropertyInfo[] propertyInfos = ReflectorHelper.GetPropertiesAssignAbleFrom(GetType(), typeof (DBObject));
//                foreach (PropertyInfo info in propertyInfos)
//                {
//                    DBObject value = (DBObject) info.GetValue(this, null);
//                    if (value != null) value.ConstrainsDisabled = true;
//                }
//            }
//            if (isDeleted)
//                base.Save();
//            else if (!constrainsDisabled && !HasErrors())
//            {
//                saveAggregated(this);
//                GetKey();
//                try
//                {
//                    base.Save();
//                }
//                catch (Exception e)
//                {
//                    throw new Exception("{" + ClassInfo.KeyProperty.GetValue(this) + "}" + GetType().FullName, e);
//                }
//                isChanged = false;
//            }
//            else if (constrainsDisabled)
//            {
//                GetKey();
//                base.Save();
//                isChanged = false;
//            }
//            else
//                throw new DALException(dataErrorInfoProvider.NoteError, GetType(), this);
////            if (dbObjectKey != null)
////            {
////                dbObjectKey.LastKey += 1;
////                dbObjectKey.Save();
////            }
//            DoDecryption();
//            AfterSave();
////            Reload();
//        }

        private void saveAggregated(DBObject dbObject)
        {
//            PropertyInfo[] properties =
//                ReflectorHelper.GetDecoratedProperties(dbObject.GetType(), typeof (MyAggregated));
//            foreach (PropertyInfo info in properties)
//            {
//                object value = info.GetValue(dbObject, null);
//                if (value != null)
//                {
//                    var dbObjectValue = (DBObject) value;
//                    saveAggregated(dbObjectValue);
//                    bool disabled = dbObjectValue.ConstrainsDisabled;
//                    dbObjectValue.ConstrainsDisabled = ConstrainsDisabled;
//                    dbObjectValue.Save();
//                    dbObjectValue.ConstrainsDisabled = disabled;
//                }
//            }
        }

        private void doEncryption()
        {
            if (!encrypt || encrypted) return;
            PropertyInfo[] properties = ReflectorHelper.GetDecoratedProperties(GetType(), typeof (Encrypt));
            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType.Equals(typeof (string)))
                {
//                    var value = property.GetValue(this, null) as string;
//                    if (value + "" != "")
//                        property.SetValue(this, Encryption.Encrypt(value), null);
                }
            }
            encrypted = true;
//			foreach (XPMemberInfo memberInfo in ClassInfo.PersistentProperties)
//			{
//				if (memberInfo.MemberType.Equals(typeof (string)))
//				{
//					string value = memberInfo.GetValue(this) as string;
//					if (value + "" != "")
//					{
//						memberInfo.SetValue(this, Encryption.Encrypted(value));
//					}
//				}
//			}
        }

        public void GetKey()
        {
            if (disableKeySet || !IsNewObject || !ClassInfo.KeyProperty.MemberType.Equals(typeof (int)) ||
                !ClassInfo.KeyProperty.GetValue(this).Equals(0))
                return;
            var key = ClassInfo.KeyProperty.GetAttributeInfo(typeof (KeyAttribute)) as KeyAttribute;
            if (key != null && !key.AutoGenerate)
            {
                Type baseType = GetBaseType(GetType());
                string tableName = Session.GetClassInfo(baseType).TableName;
                using (var session = new Session(Session.DataLayer))
                {
                    var dbObjectKey =
                        session.FindObject(typeof (DBObjectKey),
                                           new BinaryOperator("TableName",
                                                              tableName)) as
                        DBObjectKey;
                    if (dbObjectKey == null)
                    {
                        dbObjectKey = new DBObjectKey(session)
                                          {
                                              TableName = tableName,
                                              LastKey = (GetLastKey(session, ClassInfo) + 1)
                                          };
                        dbObjectKey.Save();
                    }
                    else
                    {
                        dbObjectKey.LastKey += 1;
                        dbObjectKey.Save();
                    }
                    ClassInfo.KeyProperty.SetValue(this, dbObjectKey.LastKey);
                }
            }
        }


        public static int GetLastKey(Session session, XPClassInfo classInfo)
        {
            Type type = GetBaseType(classInfo.ClassType);
            var collection = new DBCollection(session, type, null,
                                              new[]
                                                  {new SortProperty("ID", SortingDirection.Descending)})
                                 {TopReturnedObjects = 1};
            int value = collection.Count > 0
                            ? (int) ((IXPSimpleObject) collection[0]).ClassInfo.KeyProperty.GetValue(collection[0])
                            : 0;
            collection.Dispose();
            return value;
        }

        /// <summary>
        /// returns the type of the 1st persistent inheritor of <see cref="DBObject"/>
        /// </summary>
        /// <returns></returns>
        public static Type GetBaseType(Type type)
        {
            Type retType = type;
            while (ReflectorHelper.GetAttribute(type.BaseType, typeof (NonPersistentAttribute)) == null &&
                   !type.BaseType.IsAbstract)
            {
                retType = type.BaseType;
                type = type.BaseType;
            }

            return retType;
        }

//        private void setConstrains(DBObject dbObject, bool constrainsDisabled)
//        {
//            if (dbObject!= null)
//            {
//                PropertyInfo[] propertyInfos = ReflectorHelper.GetPropertiesAssignAbleFrom(dbObject.GetType(), typeof(DBObject));
//                foreach (PropertyInfo propertyInfo in propertyInfos)
//                {
//                    DBObject value = propertyInfo.GetValue(dbObject, null) as DBObject;
//                    if (value != null) value.ConstrainsDisabled=constrainsDisabled;
//                }
//            }
//        }

        //		private void destroyB4Save()
        //		{
        //			PropertyInfo[] properties = GetType().GetProperties();
        //			foreach (PropertyInfo property in properties)
        //			{
        //				object[] attributes = property.GetCustomAttributes(typeof (DestroyB4Save), true);
        //				if (attributes.Length > 0)
        //				{
        //					DestroyB4Save destroyB4Save = (DestroyB4Save) attributes[0];
        //					object value = property.GetValue(this, null);
        //					if (value != null)
        //					{
        //						object aggregatedPropertyValue = value.GetType().GetProperty(destroyB4Save.PropertyName).GetValue(value, null);
        //						if (aggregatedPropertyValue.Equals(0) || aggregatedPropertyValue == null || aggregatedPropertyValue.Equals(""))
        //							property.SetValue(this, null, null);
        //					}
        //				}
        //			}
        //		}


        protected override void OnDeleted()
        {
            base.OnDeleted();
            isDeleted = true;
        } //        public override void Delete()
//        {
//            //			if (!nullReferenceException())
//            if (!isDeleted)
//                base.Delete();
//            isDeleted = true;
//        }
        #endregion
        #region error methods
        public virtual bool HasErrors()
        {
            //			destroyB4Save();

            bool b = errors(ReflectorHelper.GetXpMemberInfos(ClassInfo));
            return b;
        }

        /// <summary>
        /// runs all attribute based contraints of the property 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns>The constrains error string or ""</returns>
        public string GetPropertyError(string propertyName)
        {
            var xpMemberInfos =
                new[] {ReflectorHelper.GetXpMemberInfo(Session, GetType(), propertyName)};
            dataErrorInfoProvider.GetErrors(false, xpMemberInfos);
            return this[propertyName];
        }

        /// <summary>
        /// u should overidde this method allowind first the base method to run and put your constrains here
        /// </summary>
        /// <returns>Convert.ToBoolean(Error.Length)</returns>
        private bool errors(IEnumerable<XPMemberInfo> xpMemberInfos)
        {
            dataErrorInfoProvider.ClearErrors();


//			Hashtable uniqConstrains = new Hashtable();
            foreach (XPMemberInfo xpMemberInfo in xpMemberInfos)
            {
                if (xpMemberInfo != null)
                {
                    notNullAbleValueAttribute(xpMemberInfo);

//                    stringLenghtAttribute(xpMemberInfo);

                    integerBetweenAttribute(xpMemberInfo);

                    //					MakeUniqueContrainsHashTable(property, uniqConstrains);

                    MakeUniqueContrainsHashTable(xpMemberInfo);

                    aggregatedAttribute(xpMemberInfo);
                }
            }
//			setUniqueConstrainsErrors(uniqConstrains);

            dataErrorInfoProvider.GetErrors(false, ReflectorHelper.GetXpMemberInfos(ClassInfo));

            return Convert.ToBoolean(dataErrorInfoProvider.NoteError.Length);
        }

        public object[] MakeUniqueContrainsHashTable(XPMemberInfo xpMemberInfo)
        {
            object[] args = null;
            PropertyInfo propertyInfo = GetType().GetProperty(xpMemberInfo.Name);
//            var attribute =
//                (UniqueConstraintAttribute)
//                ReflectorHelper.GetAttribute(propertyInfo, typeof (UniqueConstraintAttribute));
//            if (attribute != null)
//            {
//                if (!constrainMustBeChecked(attribute, xpMemberInfo)) return args;
//                args = new object[attribute.Fields.Length + 1];
//                args[0] = Session;
//                int j = 1;
//                foreach (string field in attribute.Fields)
//                {
//                    XPMemberInfo memberInfo = ClassInfo.GetMember(field);
//                    args[j] = getPropertyValue(memberInfo);
//                    j++;
//                }
//                DBObject dbObject = GetDbObject(args, attribute.BuilderFindMethod);
//                setUniqueConstrainError(dbObject, attribute);
//                //            }
//            }
            return args;
        }


        private void aggregatedAttribute(XPMemberInfo memberInfo)
        {
            if (typeof (DBObject).IsAssignableFrom(memberInfo.MemberType))
            {
                PropertyInfo propertyInfo = GetType().GetProperty(memberInfo.Name);
                var aggregatedAttribute =
                    (AggregatedAttribute) ReflectorHelper.GetAttribute(propertyInfo, typeof (AggregatedAttribute));
                if (aggregatedAttribute != null)
                {
                    var dbObject = memberInfo.GetValue(this) as DBObject;
                    if (dbObject != null)
                    {
                        dbObject.HasErrors();
                        this[memberInfo.Name + "!"] = dbObject.Error;
                    }
                }
            }
        }

        private object getPropertyValue(XPMemberInfo xpMemberInfo)
        {
            object value = "";
            try
            {
                value = xpMemberInfo.GetValue(this);
            }
            catch (NullReferenceException)
            {
            }
            catch (Exception e)
            {
                if (!e.InnerException.GetType().Equals(typeof (NullReferenceException)))
                    throw e.InnerException;
            }
            return value;
        }

        protected void integerBetweenAttribute(XPMemberInfo xpMemberInfo)
        {
            PropertyInfo propertyInfo = GetType().GetProperty(xpMemberInfo.Name);
//            var betweenAttribute =
//                (BetweenAttribute) ReflectorHelper.GetAttribute(propertyInfo, typeof (BetweenAttribute));
//            if (betweenAttribute != null)
//            {
//                if (constrainMustBeChecked(betweenAttribute, xpMemberInfo))
//                {
//                    object value = getPropertyValue(xpMemberInfo);
//                    if (xpMemberInfo.MemberType.Equals(typeof (int)))
//                    {
//                        if (((int) value < betweenAttribute.Min) | (int) value > betweenAttribute.Max)
//                            this[xpMemberInfo.Name] = betweenAttribute.ConstrainMessage + " " + xpMemberInfo.Name;
//                    }
//                    else if (xpMemberInfo.MemberType.Equals(typeof (Single)))
//                    {
//                        if (((Single) value < betweenAttribute.Min) | (Single) value > betweenAttribute.Max)
//                            this[xpMemberInfo.Name] = betweenAttribute.ConstrainMessage + " " + xpMemberInfo.Name;
//                    }
//                    else if (xpMemberInfo.MemberType.Equals(typeof (Double)))
//                    {
//                        if (((Double) value < betweenAttribute.Min) | (Double) value > betweenAttribute.Max)
//                            this[xpMemberInfo.Name] = betweenAttribute.ConstrainMessage + " " + xpMemberInfo.Name;
//                    }
//                    else if (xpMemberInfo.MemberType.Equals(typeof (Decimal)))
//                    {
//                        if (((Decimal) value < betweenAttribute.Min) | (Decimal) value > betweenAttribute.Max)
//                            this[xpMemberInfo.Name] = betweenAttribute.ConstrainMessage + " " + xpMemberInfo.Name;
//                    }
//                }
//            }
        }

//		public void MakeUniqueContrainsHashTable(PropertyInfo propertyInfo, Hashtable uniqConstrains)
//		{
//			object[] attributes = propertyInfo.GetCustomAttributes(typeof (UniqueConstraintAttribute), true);
//			for (int i = 0; i < attributes.Length; i++)
//			{
//				UniqueConstraintAttribute attribute = (UniqueConstraintAttribute) attributes[i];
//				if (!constrainMustBeChecked(attribute, propertyInfo)) return;
//				object value = getXpMemberInfoValue(propertyInfo);
//				if (!uniqConstrains.ContainsKey(attribute))
//					uniqConstrains.Add(attribute, new ArrayList());
//
//				((ArrayList) uniqConstrains[attribute]).Add(value);
//			}
//
//			
//		}

        //TODO: use a GOF patern to remove ifs
        protected void notNullAbleValueAttribute(XPMemberInfo xpMemberInfo)
        {
//            PropertyInfo propertyInfo = GetType().GetProperty(xpMemberInfo.Name);
//            var notNullAbleValueAttribute =
//                (NotNullAbleValueAttribute)
//                ReflectorHelper.GetAttribute(propertyInfo, typeof (NotNullAbleValueAttribute));
//
//            if (notNullAbleValueAttribute != null)
//            {
//                if (!constrainMustBeChecked(notNullAbleValueAttribute, xpMemberInfo)) return;
//                object value = getPropertyValue(xpMemberInfo);
//                if (xpMemberInfo.MemberType.Equals(typeof (string)))
//                {
//                    if (StringHelper.Trim(value as string) == "")
//                        this[xpMemberInfo.Name] = notNullAbleValueAttribute.ConstrainMessage;
//                }
//                else if (xpMemberInfo.MemberType.Equals(typeof (int)) ||
//                         xpMemberInfo.MemberType.Equals(typeof (float)) ||
//                         xpMemberInfo.MemberType.Equals(typeof (double)) ||
//                         xpMemberInfo.MemberType.Equals(typeof (Decimal)))
//                {
//                    if (value.Equals(0.0d) || value.Equals(0.0f) || value.Equals(0))
//                        this[xpMemberInfo.Name] = notNullAbleValueAttribute.ConstrainMessage;
//                }
//                else if (xpMemberInfo.MemberType.Equals(typeof (bool)))
//                {
//                    if (value == null)
//                        this[xpMemberInfo.Name] = notNullAbleValueAttribute.ConstrainMessage;
//                }
//                else if (xpMemberInfo.MemberType.Equals(typeof (DateTime)))
//                {
//                    if (value == null)
//                        this[xpMemberInfo.Name] = notNullAbleValueAttribute.ConstrainMessage;
//                    else if (value.Equals(new DateTime(1, 1, 1)))
//                        this[xpMemberInfo.Name] = notNullAbleValueAttribute.ConstrainMessage;
//                }
//                else if (xpMemberInfo.MemberType.Equals(typeof (TimeSpan)))
//                {
//                    throw new NotSupportedException("Use " + typeof (NullableInt32).Name + " Type");
//                }
//
//                else if (typeof (DBObject).IsAssignableFrom(xpMemberInfo.MemberType))
//                {
//                    if (value == null)
//                        this[xpMemberInfo.Name + "!"] = notNullAbleValueAttribute.ConstrainMessage;
//                    else if (((DBObject) value).Error != "")
//                        this[xpMemberInfo.Name + "!"] = ((DBObject) value).Error;
//                }
//                else if (xpMemberInfo.MemberType.Equals(typeof (NullableInt32)) ||
//                         xpMemberInfo.MemberType.Equals(typeof (NullableTimeSpan)))
//                {
//                    if (value == null)
//                        this[xpMemberInfo.Name] = notNullAbleValueAttribute.ConstrainMessage;
//                }
//                else if (xpMemberInfo.MemberType.Equals(typeof (XPCollection)))
//                {
//                    if (((XPCollection) value).Count == 0)
//                        this[xpMemberInfo.Name] = notNullAbleValueAttribute.ConstrainMessage;
//                }
//                else
//                    throw new NotImplementedException(xpMemberInfo.MemberType.ToString());
//                //				aggregatedAttribute();
//            }
        }

        public void checkStringProperty(string propertyName, string invalidPropertyNameMessage,
                                        string invalidPropertyValueLengthMessage)
        {
        }

//        private bool constrainMustBeChecked(ConditionalConstrainAttribute attribute, XPMemberInfo xpMemberInfo)
//        {
//            if (attribute.FactoryType != null)
//            {
//                #region check for property
//                PropertyInfo property =
//                    attribute.FactoryType.GetProperty(attribute.MethodName, BindingFlags.Static | BindingFlags.Public);
//                if (property != null)
//                {
//                    object actualValue = property.GetValue(null, null);
//
//                    var list = new ArrayList();
//                    if (attribute.Value is Array)
//                        list = new ArrayList((ICollection) attribute.Value);
//                    else
//                        list.Add(attribute.Value);
//                    return list.Contains(actualValue);
//                }
//                #endregion
//                Attribute info = null;
//                if (xpMemberInfo.HasAttribute(typeof (NotNullAbleValueAttribute)))
//                    info = xpMemberInfo.GetAttributeInfo(attribute.GetType());
//                return
//                    dataErrorInfoProvider.GetConditionalConstrainAttributeErrors(xpMemberInfo, true, new object[] {info});
//            }
//            return !(attribute.Guid != null && excludedConstrains.Contains(attribute.Guid));
//        }


        private void setAssociationErrors(XPMemberInfo xpMemberInfo, AssociationAttribute associationAttribute)
        {
            var dbObject = (DBObject) xpMemberInfo.GetValue(this);
            if (dbObject != null)
                dbObject[GetAssociatedCollection(associationAttribute.Name, dbObject)] = Error;
        }


//        protected void stringLenghtAttribute(XPMemberInfo xpMemberInfo)
//        {
//            if ((xpMemberInfo.MemberType.Equals(typeof (string))) && (this[xpMemberInfo.Name] == ""))
//            {
//                PropertyInfo propertyInfo = GetType().GetProperty(xpMemberInfo.Name);
//                var lenghtAttribute =
//                    (LenghtAttribute) ReflectorHelper.GetAttribute(propertyInfo, typeof (LenghtAttribute));
//
//                if (lenghtAttribute != null)
//                {
//                    if (!constrainMustBeChecked(lenghtAttribute, xpMemberInfo)) return;
//                    object value = getPropertyValue(xpMemberInfo);
//                    bool b = value.ToString().Length > lenghtAttribute.Lenght;
//                    if (lenghtAttribute.IsMinimumLenght)
//                        b = value.ToString().Length < lenghtAttribute.Lenght;
//                    if (b)
//                        this[xpMemberInfo.Name] =
//                            lenghtAttribute.ConstrainMessage.Replace(ConditionalConstrainAttribute.UseValue,
//                                                                     lenghtAttribute.Lenght.ToString());
//                }
//            }
//        }

//		private void setUniqueConstrainsErrors(Hashtable uniqConstrains)
//		{
//			foreach (UniqueConstraintAttribute  uniqueConstraintAttribute in uniqConstrains.Keys)
//			{
//				DBObject dbObject = GetDbObject(uniqConstrains, uniqueConstraintAttribute);
//				setUniqueConstrainError(dbObject, uniqueConstraintAttribute);
//			}
//		}

/*
        private void setUniqueConstrainError(IXPClassInfoProvider dbObject, UniqueConstraintAttribute uniqueConstraintAttribute)
        {
            if (dbObject != null)
            {
                object keyValue = ClassInfo.KeyProperty.GetValue(this);
                object foundKeyvalue = dbObject.ClassInfo.KeyProperty.GetValue(dbObject);
                if (!foundKeyvalue.Equals(keyValue))
                    this[uniqueConstraintAttribute.BoundPropertyName] = uniqueConstraintAttribute.ConstrainMessage;
            }
        }
*/

        /// <summary>
        /// returns the DBObject of the method defived by the <see cref="BuilderDepandance"/> and the <see cref="builderFindMethod"/> 
        /// </summary>
        /// <param name="builderFindMethod"></param>
        /// <returns></returns>
        ///<param name="args"></param>
        public DBObject GetDbObject(object[] args, string builderFindMethod)
        {
            throw new NotImplementedException();
//            object[] builderDepandances = GetType().GetCustomAttributes(typeof (BuilderDepandance), true);
//            if (builderDepandances.Length == 0)
//                throw new AttributeMissingException(typeof (BuilderDepandance), GetType());
//            Type type = ((BuilderDepandance) builderDepandances[0]).BuilderType;
//
//            DBObject member = (DBObject) type.InvokeMember(builderFindMethod,
//                                                           BindingFlags.InvokeMethod | BindingFlags.Static |
//                                                           BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
//                                                           null,
//                                                           new object(),
//                                                           args);
//
//            return member;
        }

//		/// <summary>
//		/// returns the DBObject of the method defived by the <see cref="BuilderDepandance"/> and the <see cref="UniqueConstraintAttribute"/> attributes
//		/// </summary>
//		/// <param name="uniqConstrains"></param>
//		/// <param name="uniqueConstraintAttribute"></param>
//		/// <returns></returns>
//		public DBObject GetDbObject(Hashtable uniqConstrains, UniqueConstraintAttribute uniqueConstraintAttribute)
//		{
//			ArrayList constrain = (ArrayList) uniqConstrains[uniqueConstraintAttribute];
//			object[] args = new object[constrain.Count + 1];
//			args[0] = Session;
//			(constrain).CopyTo(args, 1);
//
//			return GetDbObject(args, uniqueConstraintAttribute.BuilderFindMethod);
//		}
        #endregion
        #region IsChanged implementation
        [Browsable(false)]
        [NonPersistent]
        public bool IsChanged
        {
            get { return isChanged; }
            set
            {
                isChanged = value;
                if (isLoaded || Session.IsNewObject(this))
                    doActionForParentAggregatedAssociations(
                        setIsChangedForParentAssociation);
            }
        }

        private void DBObject_Changed(object sender, ObjectChangeEventArgs args)
        {
            IsChanged = true;
        }

        private void setIsChangedForParentAssociation(XPMemberInfo xpMemberInfo,
                                                      AssociationAttribute associationAttribute)
        {
            object value = xpMemberInfo.GetValue(this);
            if (value != null)
                ((DBObject) value).IsChanged = true;
            #region
            //			foreach (XPMemberInfo xpMemberInfo in ClassInfo.PersistentProperties)
            //			{
            //				if ((xpMemberInfo.HasAttribute(typeof (AssociationAttribute))))
            //				{
            //					AssociationAttribute associationAttribute = (AssociationAttribute) xpMemberInfo.GetAttributeInfo(typeof (AssociationAttribute));
            //					object[] attributes = xpMemberInfo.MemberType.GetProperty(
            //						getAssociatedCollection(associationAttribute.Name, xpMemberInfo.ReferenceType)).GetCustomAttributes(typeof (AggregatedAttribute), false);
            //					if (attributes.Length > 0)
            //					{
            //						object value = xpMemberInfo.GetValue(this);
            //						if (value != null)
            //							((DBObject) value).IsChanged = true;
            //					}
            //				}
            //			}
            #endregion
        }
        #endregion
        #region GetObjectProperties
        /// <summary>
        /// return a list of strings that contain the structure of a persistent object
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public string[] GetObjectProperties(Type objectType)
        {
            XPClassInfo classInfo = Session.DefaultSession.Dictionary.GetClassInfo(objectType);
            if (classInfo != null)
                return GetObjectProperties(classInfo, new ArrayList());
            return new string[] {};
        }

        /// <summary>
        /// return a list of strings that contain the structure of a persistent object
        /// </summary>
        /// <param name="xpoInfo"></param>
        /// <param name="processed"></param>
        /// <returns></returns>
        public string[] GetObjectProperties(XPClassInfo xpoInfo, ArrayList processed)
        {
            if (processed.Contains(xpoInfo)) return new string[] {};
            processed.Add(xpoInfo);
            var result = new ArrayList();
            foreach (XPMemberInfo m in xpoInfo.PersistentProperties)
                if (!(m is ServiceField) && m.IsPersistent)
                {
                    result.Add(m.Name);
                    if (m.ReferenceType != null)
                    {
                        string[] childProps = GetObjectProperties(m.ReferenceType, processed);
                        foreach (string child in childProps)
                            result.Add(string.Format("{0}.{1}", m.Name, child));
                    }
                }

            foreach (XPMemberInfo m in xpoInfo.CollectionProperties)
            {
                string[] childProps = GetObjectProperties(m.CollectionElementType, processed);
                foreach (string child in childProps)
                    result.Add(string.Format("{0}.{1}", m.Name, child));
            }
            return result.ToArray(typeof (string)) as string[];
        }
        #endregion
        #region public static methods
        public static string GetTableName(Type type, Session session)
        {
            return session.GetClassInfo(type).TableName;
        }

        public static void AddPrimaryKeys(Assembly assembly, string fromSchema, string connectionString)
        {
            using (var session = new Session())
            {
                session.ConnectionString = connectionString;
                session.Connect();
                AddPrimaryKeys(assembly, fromSchema, session);
            }
        }

        public static void AddPrimaryKeys(Assembly assembly, string fromSchema, Session session)
        {
//            Type[] types = ReflectorHelper.GetTypes(typeof (DBObject), assembly);
//            ReflectorHelper.FilterTypes(ref types, true);
            XPClassInfo[] xpClassInfos = session.Dictionary.CollectClassInfos(new[] {assembly});
            foreach (XPClassInfo xpClassInfo in xpClassInfos)
            {
                Type type = xpClassInfo.ClassType;
                string tableName = type.Name;
                Attribute attribute = ReflectorHelper.GetAttribute(type, typeof (MapToAttribute));
                if (attribute != null)
                    tableName = ((MapToAttribute) attribute).MappingName;
                try
                {
//                    DBHelper.TransferToShema(session.DataLayer.CreateCommand(), fromSchema, tableName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                addPrimaryKey(session.DataLayer.CreateCommand(), tableName);
            }
            try
            {
//                DBHelper.TransferToShema(session.DataLayer.CreateCommand(), fromSchema, "XPObjectType");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            addPrimaryKey(session.DataLayer.CreateCommand(), "XPObjectType", "OID");
            session.Dispose();
        }

        private static void addPrimaryKey(IDbCommand command, string tableName, string pkName)
        {
            try
            {
                command.CommandText = "ALTER TABLE [dbo].[" + tableName + "] ADD CONSTRAINT pk" + tableName +
                                      "Id PRIMARY KEY(\"" + pkName + "\")";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }

        private static void addPrimaryKey(IDbCommand command, string tableName)
        {
            addPrimaryKey(command, tableName, "ID");
        }

        [Obsolete("Use reflectorHelper.SetProperty")]
        public static void SetProperty(string propertyName, object value, DBObject dbObject)
        {
            ReflectorHelper.SetProperty(propertyName, value, dbObject);
        }

        [Obsolete("Use reflectorHelper.GetPropertyInfo")]
        public static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            return ReflectorHelper.GetPropertyInfo(type, propertyName);
        }

        [Obsolete("Use reflectorHelper.GetPropertyInfoValue")]
        public static object GetPropertyInfoValue(string propertyName, DBObject dbObject)
        {
            return ReflectorHelper.GetPropertyInfoValue(propertyName, dbObject);
        }

        public static Type GetBuilderType(Type xpBaseObjectType)
        {
            throw new NotImplementedException();

//            object[] attributes = xpBaseObjectType.GetCustomAttributes(typeof (BuilderDepandance), true);
//            if (attributes.Length > 0)
//                return ((BuilderDepandance) attributes[0]).BuilderType;
//            throw new AttributeMissingException(typeof (BuilderDepandance), xpBaseObjectType);
        }


        /// <summary>
        /// finds the <see cref="AssociationAttribute.ElementTypeName"/> of XpCollections that are marked with an <see cref="AssociationAttribute"/> given the child GridLevelNode of the focused view
        /// </summary>
        /// <returns>returns the <see cref="AssociationAttribute.Name"/></returns>
        public static Type FindTypeOfCollection(AssociationAttribute associationAttribute)
        {
            Assembly assembly = Assembly.Load(associationAttribute.AssemblyName);
            return assembly.GetType(associationAttribute.ElementTypeName, true);
        }

        /// <summary>
        /// Executes the <see cref="queryName"/> and returns an array list with values of the 1st field of the query
        /// </summary>
        /// <param name="queryName"></param>
        /// <returns></returns>
        ///<param name="session"></param>
        public static ArrayList GetLookUpList(string queryName, Session session)
        {
            IDbCommand command = session.Connection.CreateCommand();
            command.CommandText = "SELECT * FROM " + queryName;
            IDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
            var list = new ArrayList();
            while (reader.Read())
                list.Add(reader.GetValue(0));
            reader.Close();
            reader.Dispose();
            command.Dispose();
            return list;
        }

        public static DBObject Find(Session session, int key, XPClassInfo xpClassInfo)
        {
            var o = (IXPSimpleObject) session.FindObject(xpClassInfo, new BinaryOperator("ID", key));
            return o as DBObject;
        }

        public static DBObject Find(Session session, int key, Type dbObjectType)
        {
            var o = (IXPSimpleObject) session.FindObject(dbObjectType, new BinaryOperator("ID", key));
            return o as DBObject;
        }
        #endregion
        #region protected Methods
//        protected void delete(XPCollection[] collections)
//        {
//            delete(this, collections);
//        }

//        protected void delete(DBObject dbObject, XPCollection[] collections)
//        {
//            foreach (XPCollection collection in collections)
//                DBCollection.DeleteAllSuper(collection);
//            
//            
//            base.Delete();
//        }
        #endregion
//        public IXPSimpleObject CloneTo()
//        {
//            return CloneTo(GetType());
//        }
//        public IXPSimpleObject CloneTo(Type targetType)
//        {
//            return new Cloner().CloneTo(this, targetType);
//        }
    }

    /// <summary>
    /// <para>this Class can be used as a field in any class to provide constrain error checking based on DataErrorInfoCheckAttribute attributes decoration in conjuction with IDataError interface</para>
    /// <para>For objects of type DBObject its provided for all public properties</para>
    /// </summary>
    public class DataErrorInfoProvider
    {
        public static string ErrorMessageDelimeter = Environment.NewLine;
        private readonly object hostObject;
        private readonly Hashtable propertyErrors = new Hashtable();
        private readonly bool useDataErrorInfoCheckAttribute;
        private string noteError = "";

        /// <summary>
        /// Will check only properies decorated with the DataErrorInfoCheckAttribute if o is not a DBOBject
        /// </summary>
        /// <param name="o">the host object</param>
        public DataErrorInfoProvider(object o) : this(o, true)
        {
        }

        /// <summary>
        /// _
        /// </summary>
        /// <param name="o">the host object</param>
        ///<param name="useDataErrorInfoCheckAttribute"></param>
        public DataErrorInfoProvider(object o, bool useDataErrorInfoCheckAttribute)
        {
            throw new NotImplementedException();
//            hostObject = o;
//            this.useDataErrorInfoCheckAttribute = useDataErrorInfoCheckAttribute;
//            Type[] interfaces = o.GetType().GetInterfaces();
//            var list = new ArrayList(interfaces);
//            if (!list.Contains(typeof (IDataErrorInfo)))
//                throw new InterfaceNotImplementedException(o.GetType(), typeof (IDataErrorInfo));
//            initPropertyErrors(o);
        }

        public string ErrorString
        {
            get
            {
                string errorString = "";
                foreach (Object obj in propertyErrors.Values)
                    if (obj.ToString() != "")
                        errorString += obj + ErrorMessageDelimeter;
                return errorString;
            }
        }

        public string NoteError
        {
            get
            {
                noteError = ErrorString;
                if (noteError.Length > 0)
                    noteError = noteError.Trim().TrimEnd(ErrorMessageDelimeter.ToCharArray());
                return noteError;
            }
            set
            {
                if (noteError == value) return;
                if (noteError + "" != "")
                    noteError += ErrorMessageDelimeter + value;
                noteError = value;
            }
        }

        private void initPropertyErrors(object o)
        {
            propertyErrors.Clear();
            PropertyInfo[] properties = o.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (o is DBObject)
                {
                    if (typeof (DBObject).IsAssignableFrom(property.PropertyType))
                    {
                        propertyErrors.Add(property.Name + "!", "");
                        propertyErrors.Add(property.Name + "!Key", "");
                    }
                    propertyErrors.Add(property.Name, "");
                }
                else
                {
                    if (useDataErrorInfoCheckAttribute)
                    {
//                        object[] attributes = property.GetCustomAttributes(typeof (DataErrorInfoCheckAttribute), true);
//                        if (attributes.Length > 0)
//                            propertyErrors.Add(property.Name, "");
                    }
                }
            }
        }

        public void ClearErrors()
        {
            initPropertyErrors(hostObject);
            NoteError = "";
        }


        public void SetColumnError(string elem, string error)
        {
            SetColumnError(elem, error, false);
        }

        public void SetColumnError(string elem, string error, bool append)
        {
            if (propertyErrors.ContainsKey(elem))
            {
                if ((string) propertyErrors[elem] == error) return;
                if (append)
                    propertyErrors[elem] += ErrorMessageDelimeter + error;
                else
                    propertyErrors[elem] = error;
            }
            else
            {
                throw new ArgumentException("Property " + elem + " does not exist");
            }
        }

        public string GetColumnError(string elem)
        {
            if (propertyErrors.ContainsKey(elem))
                return (string) propertyErrors[elem];
            return null;
        }

        public void GetErrors(bool clearErrorsB4, XPMemberInfo[] xpMemberInfos)
        {
            if (clearErrorsB4)
                ClearErrors();
            getErrors(xpMemberInfos);
        }

        private void getErrors(IEnumerable<XPMemberInfo> xpMemberInfos)
        {
            foreach (XPMemberInfo xpMemberInfo in xpMemberInfos)
            {
                if (xpMemberInfo != null)
                {
                    bool appendError = false;
                    if (!(hostObject is DBObject))
                        appendError = getNotNullAbleValueAttributeErrors(xpMemberInfo);

                    getFileExistAttributeErrors(xpMemberInfo, ref appendError);

                    getComparisonAttributeErrors(xpMemberInfo, ref appendError);

                    getConditionalConstrainAttributeErrors(xpMemberInfo, ref appendError);

                    getEmailAttributeErrors(xpMemberInfo, appendError);
                }
            }
        }

        private void getEmailAttributeErrors(XPMemberInfo xpMemberInfo, bool appendError)
        {
            throw new NotImplementedException();
//            if (xpMemberInfo.HasAttribute(typeof (EmailAttribute)))
//            {
//                object attributes = xpMemberInfo.GetAttributeInfo((typeof (EmailAttribute)));
//                //            if (attributes.Length > 0)
//                //            {
//                if (attributes != null)
//                {
//                    var value = xpMemberInfo.GetValue(hostObject) as string;
//                    var RegexObj = new Regex("\\A[a-z0-9._%-]*@{1}[a-z0-9._%-]*\\.[a-z0-9]{2,3}\\z");
//                    bool found = RegexObj.IsMatch(value + "");
//                    if (!found)
//                        SetColumnError(xpMemberInfo.Name, value + " is not a valid email", appendError);
//                }
//                //            }
//            }
        }

        /// <summary>
        /// clears all errors and retrieve the new ones
        /// </summary>
        public void GetErrors()
        {
            ClearErrors();
            getErrors(ReflectorHelper.GetXpMemberInfos(((DBObject) hostObject).ClassInfo));
        }
        #region get Attribute Error private methods
        private void getComparisonAttributeErrors(XPMemberInfo xpMemberInfo, ref bool appendError)
        {
            throw new NotImplementedException();
//            if (xpMemberInfo.HasAttribute(typeof (ComparisonAttribute)))
//            {
//                object attributes = xpMemberInfo.GetAttributeInfo((typeof (ComparisonAttribute)));
//                //            if (attributes.Length > 0)
//                //            {
//                var comparisonAttribute = (ComparisonAttribute) attributes;
//                object comparedValue =
//                    hostObject.GetType().GetProperty(comparisonAttribute.OtherPropertyName).GetValue(hostObject, null);
//                object currentValue = xpMemberInfo.GetValue(hostObject);
//                bool equal = currentValue == null ? currentValue == comparedValue : currentValue.Equals(comparedValue);
//                if (equal && comparisonAttribute.ComparisonAttributeEnum == ComparisonAttributeEnum.Equal ||
//                    !equal && comparisonAttribute.ComparisonAttributeEnum == ComparisonAttributeEnum.NotEqual)
//                    SetColumnError(xpMemberInfo.Name, comparisonAttribute.ConstrainMessage, appendError);
//                //            }
//            }
        }

        private void getFileExistAttributeErrors(XPMemberInfo xpMemberInfo, ref bool appendError)
        {
            throw new NotImplementedException();
//            if (xpMemberInfo.HasAttribute(typeof (FileExistAttribute)))
//            {
//                object attributes = xpMemberInfo.GetAttributeInfo((typeof (FileExistAttribute)));
//                //            if (attributes.Length > 0)
//                //            {
//                var value = xpMemberInfo.GetValue(hostObject) as string;
//                if (!File.Exists(value + ""))
//                {
//                    if (((FileExistAttribute) attributes).ConstrainMessage + "" == "")
//                        SetColumnError(xpMemberInfo.Name, "File " + value + " does not exist", appendError);
//                    else
//                        SetColumnError(xpMemberInfo.Name, ((FileExistAttribute) attributes).ConstrainMessage,
//                                       appendError);
//                    appendError = true;
//                }
//                //            }
//            }
        }

        private bool getNotNullAbleValueAttributeErrors(XPMemberInfo xpMemberInfo)
        {
            throw new NotImplementedException();
            bool appendError = false;
//            if (xpMemberInfo.HasAttribute(typeof (NotNullAbleValueAttribute)))
//            {
//                object attributeInfo = xpMemberInfo.GetAttributeInfo((typeof (NotNullAbleValueAttribute)));
//                //            if (attributeInfo.Length > 0)
//                //            {
//                object value = xpMemberInfo.GetValue(hostObject);
//                if (xpMemberInfo.MemberType.Equals(typeof (string)))
//                {
//                    if (value + "" == "")
//                    {
//                        SetColumnError(xpMemberInfo.Name, ((NotNullAbleValueAttribute) attributeInfo).ConstrainMessage);
//                        appendError = true;
//                    }
//                }
//                else if (xpMemberInfo.MemberType.Equals(typeof (bool)))
//                {
//                    if ((bool) value == false)
//                    {
//                        SetColumnError(xpMemberInfo.Name, ((NotNullAbleValueAttribute) attributeInfo).ConstrainMessage);
//                        appendError = true;
//                    }
//                }
//                //            }
//            }
            return appendError;
        }
        #region getConditionalConstrainAttributeErrors
        /// <summary>
        /// Finds all errors due to any conditionalConstrainAttribute that is passed as the attibutes argument
        /// </summary>
        /// <param name="xpMemberInfo"></param>
        /// <param name="appendError"></param>
        /// <param name="attributes">The attibutes of the property that u want to find its errors <example>attributes=property.GetCustomAttributes(typeof (ConditionalConstrainAttribute), true);</example></param>
        /// <returns>true if an error found</returns>
        /// <remarks>To cretae a <see cref="Session"/> parameter for a method that should added as the last parameter</remarks>
        public bool GetConditionalConstrainAttributeErrors(XPMemberInfo xpMemberInfo, bool appendError,
                                                           object[] attributes)
        {
            throw new NotImplementedException();
            bool errorFound = false;
            if (attributes.Length > 0)
//            {
//                foreach (ConditionalConstrainAttribute conditionalConstrainAttribute in attributes)
//                {
//                    if (conditionalConstrainAttribute.FactoryType != null)
//                    {
//                        object propertyValue = xpMemberInfo.GetValue(hostObject);
//                        MethodInfo method =
//                            conditionalConstrainAttribute.FactoryType.GetMethod(
//                                conditionalConstrainAttribute.MethodName,
//                                BindingFlags.Static | BindingFlags.Public);
//                        if (method == null)
//                            throw new MissingMethodException(conditionalConstrainAttribute.FactoryType.Name,
//                                                             conditionalConstrainAttribute.MethodName);
//                        object[] args = method.GetParameters().Length == 1 ? new[] {propertyValue} : null;
//
//                        if (method.ReturnType.Equals(typeof (void)))
//                            throw new MethodWithVoidReturnTypeException(method);
//
//                        bool usePropertyValue = args == null ? false : true;
//
//                        createArguemntsForConstainMethod(conditionalConstrainAttribute, ref usePropertyValue, ref args);
//
//                        object staticMethodReturnValue = method.Invoke(null, args);
//
//
//                        if (!usePropertyValue)
//                            usePropertyValue = conditionalConstrainAttribute.Value + "" ==
//                                               ConditionalConstrainAttribute.UseValue;
//                        object expectedValue = (usePropertyValue) ? propertyValue : conditionalConstrainAttribute.Value;
//                        if (staticMethodReturnValue == null
//                                ? staticMethodReturnValue != expectedValue
//                                : !staticMethodReturnValue.Equals(expectedValue))
//                        {
//                            string replaceValue = (propertyValue != null) ? propertyValue.ToString() : "";
//                            SetColumnError(xpMemberInfo.Name, conditionalConstrainAttribute.ConstrainMessage.
//                                                                  Replace(ConditionalConstrainAttribute.UseValue,
//                                                                          replaceValue + ""),
//                                           appendError);
//                            errorFound = true;
//                        }
//                    }
//                }
//            }
                return errorFound;
        }


/*
        private void createArguemntsForConstainMethod(ConditionalConstrainAttribute conditionalConstrainAttribute,
                                                      ref bool usePropertyValue, ref object[] args)
        {
            if (args == null && conditionalConstrainAttribute.Value is object[])
            {
                var objects = (object[]) conditionalConstrainAttribute.Value;
                if ((objects)[0] as string == ConditionalConstrainAttribute.UseValue)
                {
                    usePropertyValue = true;
                    int sessionParameterCount = hasSessionParameter(conditionalConstrainAttribute);
                    args = new object[objects.Length - 1 + sessionParameterCount];
                    for (int i = 1; i < objects.Length; i++)
                        args[i - 1] = hostObject.GetType().GetProperty((string) objects[i]).GetValue(hostObject, null);
                    if (sessionParameterCount == 1)
                        args[objects.Length - 1] = ((IError) hostObject).Session;
                }
                else
                    throw new NotImplementedException();
            }
        }
*/

/*
        private int hasSessionParameter(ConditionalConstrainAttribute conditionalConstrainAttribute)
        {
            MethodInfo method =
                conditionalConstrainAttribute.FactoryType.GetMethod(conditionalConstrainAttribute.MethodName);
            ParameterInfo[] parameters = method.GetParameters();
            foreach (ParameterInfo parameter in parameters)
            {
                if (parameter.ParameterType == typeof (Session))
                    return 1;
            }
            return 0;
        }
*/

        private void getConditionalConstrainAttributeErrors(XPMemberInfo xpMemberInfo, ref bool appendError)
        {
//            if (xpMemberInfo.HasAttribute((typeof (ConditionalConstrainAttribute))))
//            {
//                object attributes = xpMemberInfo.GetAttributeInfo((typeof (ConditionalConstrainAttribute)));
//                GetConditionalConstrainAttributeErrors(xpMemberInfo, appendError, new[] {attributes});
//            }
        }
        #endregion
        #endregion
    }
}
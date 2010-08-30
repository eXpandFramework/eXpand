using DevExpress.Persistent.Base;
using DevExpress.Xpo;
//[DefaultClassOptions]
//public class Derived:XPLiteObject {
//    public Derived(Session session) : base(session) {
//    }
//
//    private Base _oid;
//    [Key]
//    public Base Oid{
//        get
//        {
//            return _oid;
//        }
//        set
//        {
//            SetPropertyValue("Oid", ref _oid, value);
//        }
//    }
//    private string _derivedProp;
//    public string DerivedProp{
//        get
//        {
//            return _derivedProp;
//        }
//        set
//        {
//            SetPropertyValue("DerivedProp", ref _derivedProp, value);
//        }
//    }
//}
//[DefaultClassOptions]
//public class Base:XPLiteObject {
//    public Base(Session session) : base(session) {
//    }
//
//    private int _oid;
//    [Key]
//    public int Oid{
//        get
//        {
//            return _oid;
//        }
//        set
//        {
//            SetPropertyValue("Oid", ref _oid, value);
//        }
//    }
//    private string _baseProp;
//    public string BaseProp{
//        get
//        {
//            return _baseProp;
//        }
//        set
//        {
//            SetPropertyValue("BaseProp", ref _baseProp, value);
//        }
//    }
//}
//public class One : XPObject
//{
//    [Association("One-Manys")]
//    public XPCollection<Many> Manys
//    {
//        get
//        {
//            return GetCollection<Many>("Manys");
//        }
//    }            
//}
//public class Many : XPObject
//{
//    private One _one;
//    [Association("One-Manys")]
//    public One One{
//        get
//        {
//            return _one;
//        }
//        set
//        {
//            SetPropertyValue("One", ref _one, value);
//        }
//    }
//}
//public class BuildingDx : XPLiteObject
//{
//    PersonDx _personDx = null;
//    private int _oid;
//    [Key(true)]
//    [Persistent]
//    public int Oid{
//        get
//        {
//            return _oid;
//        }
//        set
//        {
//            SetPropertyValue("Oid", ref _oid, value);
//        }
//    }
//    public BuildingDx(Session session) : base(session) {
//    }
//
//    public PersonDx PersonDx
//    {
//        get { return _personDx; }
//        set
//        {
//            if (_personDx == value)
//                return;
//
//             
//            PersonDx prevPersonDx = _personDx;
//            _personDx = value;
//
//            if (IsLoading) return;
//
//             
//            if (prevPersonDx != null && prevPersonDx.BuildingDx == this)
//                prevPersonDx.BuildingDx = null;
//
//             
//            if (_personDx != null)
//                _personDx.BuildingDx = this;
//            OnChanged("Person");
//        }
//    }
//}
//
// [DefaultClassOptions]
//public class PersonDx : XPLiteObject
//{
//     public PersonDx(Session session) : base(session) {
//     }
//     private int _oid;
//     [Key(true)]
//     [Persistent]
//     public int Oid
//     {
//         get
//         {
//             return _oid;
//         }
//         set
//         {
//             SetPropertyValue("Oid", ref _oid, value);
//         }
//     }
//     BuildingDx _buildingDx = null;
//    
//    public BuildingDx BuildingDx
//    {
//        get { return _buildingDx; }
//        set
//        {
//            if (_buildingDx == value)
//                return;
//
//             
//            BuildingDx prevBuildingDx = _buildingDx;
//            _buildingDx = value;
//
//            if (IsLoading) return;
//
//             
//            if (prevBuildingDx != null && prevBuildingDx.PersonDx == this)
//                prevBuildingDx.PersonDx = null;
//
//             
//            if (_buildingDx != null)
//                _buildingDx.PersonDx = this;
//
//            OnChanged("House");
//        }
//    }
//}
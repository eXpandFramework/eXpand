using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using FeatureCenter.Base;

namespace ExternalApplication.Module.Win
{
//    public struct SalesKey {
//        [Persistent("stor_id")] public string StorId;
//        [Persistent("ord_num")] public string OrdNum;
//        [Persistent("title_id")] public string TitleId;
//    }
//
//[MapTo("sales"), OptimisticLocking(false)]
//public class Sales : XPBaseObject {
//    [Key, Persistent]
//    public SalesKey Key;
//
//    [Persistent("payterms")]
//    public string PayTerms;
//    
//}
//[Persistent("Customers")]
//    [DefaultClassOptions]
//public class Customer : XPObject
//{
//    public Customer(Session session) : base(session) { }
//
//    [Association, Aggregated]
//    public XPCollection<Order> Orders
//    {
//        get { return GetCollection<Order>("Orders"); }
//    }
//    private string _name;
//
//    public string Name {
//        get { return _name; }
//        set { SetPropertyValue("Name", ref _name, value); }
//    }
//}
//
//public struct OrderKey
//{
//    
//    
//    private int _orderID;
//    [Persistent("OrderID")]
//    public int OrderID
//    {
//        get { return _orderID; }
//        set
//        {
//            _orderID = value;
//        }
//    }
//
//    private Customer _customer;
//    [Persistent("CustomerID"), Association]
//    public Customer Customer
//    {
//        get { return _customer; }
//        set
//        {
//            _customer = value;
//        }
//    }
//    
//    
//}
//
//
//public class Order : XPCustomObject
//{
//    public Order(Session session) : base(session) { }
//
//    [Key, Persistent]
//    public OrderKey Key;
//    private int _total;
//    public int Total{
//        get
//        {
//            return _total;
//        }
//        set
//        {
//            SetPropertyValue("Total", ref _total, value);
//        }
//    }
//}
    public class Updater : ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            new DummyDataBuilder(Session).CreateObjects();
            if (Session.FindObject<PersistentAssemblyInfo>(CriteriaOperator.Parse("Name=?","TestAssembly"))== null)
                new PersistentAssemblyInfo(Session){Name = "TestAssembly"}.Save();
        }
    }
}

using System;

namespace FeatureCenter.Base {
    public interface IOrderLine {
        IOrder Order { get; set; }
        string Article { get; set; }
        float Quantity { get; set; }
        float UnitPrice { get; set; }
        DateTime OrderLineDate { get; set; }
        float TotalPrice { get; }
        void Save();
    }
}
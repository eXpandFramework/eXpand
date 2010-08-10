using System;

namespace FeatureCenter.Base {
    public interface IOrder{
        ICustomer Customer { get; set; }
        string Reference { get; set; }
        DateTime OrderDate { get; set; }
        float Total { get; set; }
        void Save();
    }
}
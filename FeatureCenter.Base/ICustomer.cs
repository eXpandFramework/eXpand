namespace FeatureCenter.Base {
    public interface ICustomer {
        string Name { get; set; }
        string City { get; set; }
        string Description { get; set; }
        void Save();
    }
}
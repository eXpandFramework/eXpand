namespace eXpand.Persistent.Base.PivotChart {
    public interface IPivotOption {
        string Name { get; set; }
        IPivotOptionView PivotOptionView { get; set; }
    }
}
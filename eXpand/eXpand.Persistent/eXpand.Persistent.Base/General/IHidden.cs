namespace eXpand.Persistent.Base.General{
    public interface IHidden:IDummy
    {
        bool Hidden { get; set; }
    }

    public interface IDummy {
    }
}
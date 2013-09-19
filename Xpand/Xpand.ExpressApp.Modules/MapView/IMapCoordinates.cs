namespace Xpand.ExpressApp.MapView {
    public interface IMapCoordinates {
        double Longtitude { get; }
        double Latitude { get; }
        bool HasCoordinates { get; }
    }
}

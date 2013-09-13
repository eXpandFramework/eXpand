using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.MapView
{
    public interface IMapCoordinates
    {
        double Longtitude { get; }
        double Latitude { get; }
        bool HasCoordinates { get; }
    }
}

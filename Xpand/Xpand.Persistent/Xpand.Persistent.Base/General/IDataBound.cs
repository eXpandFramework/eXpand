using System;

namespace Xpand.Persistent.Base.General{
    public interface IDataBound{
        event EventHandler<EventArgs> DataBound;
    }
}
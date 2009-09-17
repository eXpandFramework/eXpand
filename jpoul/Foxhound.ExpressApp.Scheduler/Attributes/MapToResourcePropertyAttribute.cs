using System;

namespace Foxhound.ExpressApp.Scheduler.Attributes{
    public class MapToResourcePropertyAttribute : Attribute{
        public string PropertyName { get; set; }
    }
}
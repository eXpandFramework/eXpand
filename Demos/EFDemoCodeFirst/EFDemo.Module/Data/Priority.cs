using System;
using System.Collections.Generic;

using DevExpress.Persistent.Base;

namespace EFDemo.Module.Data {
    public enum Priority {
        [ImageName("State_Priority_Low")]
        Low = 0,
        [ImageName("State_Priority_Normal")]
        Normal = 1,
        [ImageName("State_Priority_High")]
        High = 2
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.NH.Core
{
    public interface ITypeMetadata
    {
        Type Type { get; }
        IPropertyMetadata KeyProperty { get; }
        IList<IPropertyMetadata> Properties { get; }
    }
}

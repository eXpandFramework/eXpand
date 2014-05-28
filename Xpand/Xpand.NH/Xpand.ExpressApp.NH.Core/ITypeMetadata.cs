using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.NH.Core
{
    public interface ITypeMetadata
    {
        Type Type { get; }
        string KeyPropertyName { get; }
        IList<string> RelationProperties { get; }
    }
}

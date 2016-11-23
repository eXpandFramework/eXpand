using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.NH.Core
{
    public interface IPropertyMetadata
    {
        string Name { get; }
        RelationType RelationType { get;}
    }
}

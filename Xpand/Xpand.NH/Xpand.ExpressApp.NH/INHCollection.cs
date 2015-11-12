using DevExpress.Data.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.NH
{
    public interface INHCollection
    {
        CriteriaOperator Criteria { get; set; }
    }
}

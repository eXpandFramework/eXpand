using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xpand.ExpressApp.NH.BaseImpl;

namespace TestDataLayer.Maps
{
    public class ObjectPermissionMap : ClassMap<ObjectPermission>
    {
        public ObjectPermissionMap()
        {
            Id(x => x.Id).GeneratedBy.Guid().UnsavedValue(Guid.Empty);
            Map(x => x.Criteria);
            Map(x => x.AllowDelete);
            Map(x => x.AllowNavigate);
            Map(x => x.AllowRead);
            Map(x => x.AllowWrite);
            References(x => x.Owner);
            Not.LazyLoad();            
        }
    }
}

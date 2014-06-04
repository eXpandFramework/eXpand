using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xpand.ExpressApp.NH.BaseImpl;

namespace TestDataLayer.Maps
{
    public class TypePermissionMap : ClassMap<TypePermission>
    {
        public TypePermissionMap()
        {
            Id(x => x.Id);
            Map(x => x.AllowCreate);
            Map(x => x.AllowDelete);
            Map(x => x.AllowNavigate);
            Map(x => x.AllowRead);
            Map(x => x.AllowWrite);
            Map(x => x.TypeName);
            HasMany(x => x.ObjectPermissions).Not.LazyLoad();
            References(x => x.Owner).Column("Role_id").Not.LazyLoad();
            Not.LazyLoad();
        }
    }
}

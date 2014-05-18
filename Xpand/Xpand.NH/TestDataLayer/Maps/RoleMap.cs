using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xpand.ExpressApp.NH.BaseImpl;

namespace TestDataLayer.Maps
{
    public class RoleMap : ClassMap<Role>
    {
        public RoleMap()
        {
            Id(x => x.Id).GeneratedBy.Guid();
            Map(x => x.Name);
            Map(x => x.IsAdministrative);
            Map(x => x.CanEditModel);
            HasManyToMany(x => x.Users).Cascade.All().Table("UserToRole").Not.LazyLoad();
            HasMany(x => x.TypePermissions).Cascade.All().Not.LazyLoad();
            Not.LazyLoad();
        }

    }
}

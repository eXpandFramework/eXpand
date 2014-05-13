using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xpand.ExpressApp.NH.BaseImpl;

namespace TestDataLayer.Maps
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Id(x => x.Id).GeneratedBy.Guid();
            Map(x => x.UserName);
            Map(x => x.StoredPassword);
            Not.LazyLoad();
        }
    }
}

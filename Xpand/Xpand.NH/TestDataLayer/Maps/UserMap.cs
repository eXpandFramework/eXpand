using FluentNHibernate.Mapping;
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
            Map(x => x.IsActive);
            HasManyToMany(x => x.Roles).Cascade.All().Table("UserToRole").Not.LazyLoad();
            Not.LazyLoad();
        }
    }
}

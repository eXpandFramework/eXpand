using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestEntities;

namespace TestDataLayer.Maps
{
    public class PersonMap : ClassMap<Person>
    {
        public PersonMap()
        {
            Id(x => x.Id).GeneratedBy.Guid().UnsavedValue(Guid.Empty);
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.BirthDate).Nullable();
            HasMany(x => x.PhoneNumbers).KeyColumn("PersonId").Cascade.All().Not.LazyLoad();
            Not.LazyLoad();
        }
    }
}

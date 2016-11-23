using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestEntities;

namespace TestDataLayer.Maps
{
    public class PhoneNumberMap : ClassMap<PhoneNumber>
    {
        public PhoneNumberMap()
        {
            Id(x => x.Id).GeneratedBy.Guid().UnsavedValue(Guid.Empty);
            Map(x => x.Number);
            References(x => x.Person, "PersonId").Not.Nullable();
            Not.LazyLoad();
        }
    }
}

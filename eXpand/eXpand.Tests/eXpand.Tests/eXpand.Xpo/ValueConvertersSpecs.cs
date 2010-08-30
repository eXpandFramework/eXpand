using System;
using eXpand.Xpo.Converters.ValueConverters;
using Machine.Specifications;

namespace eXpand.Tests.eXpand.Xpo
{
    [Subject(typeof(SqlDateTimeOverFlowValueConverter),"Convert to storage")]
    public class When_datatime_is_less_than_1_1_1753 {
        static DateTime _convertToStorageType;
        static DateTime _dateTime;

        Establish context = () => {
            _dateTime = new DateTime(1752,1,1,1,1,1);
        };
         Because of = () => {
             _convertToStorageType = (DateTime) new SqlDateTimeOverFlowValueConverter().ConvertToStorageType(_dateTime);
         };
        It should_convert_it_1_1_1753 = () => _convertToStorageType.Date.ToShortDateString().ShouldEqual("1/1/1753");

        It should_add_the_time_of_day_to_it = () => _convertToStorageType.TimeOfDay.ToString().ShouldEqual("01:01:01");
    }
    [Subject(typeof(UtcDateTimeConverter), "Convert to storage")]
    public class When_datatime_is_sql_invalid
    {
        static DateTime _convertToStorageType;
        static DateTime _dateTime;

        Establish context = () =>
        {
            _dateTime = new DateTime(1752, 1, 1, 1, 1, 1);
        };
        Because of = () =>
        {
            _convertToStorageType = (DateTime)new UtcDateTimeConverter().ConvertToStorageType(_dateTime);
        };
        It should_convert_it_1_1_1753 = () => _convertToStorageType.Date.ToShortDateString().ShouldEqual("1/1/1753");

        It should_add_the_time_of_day_to_it = () => _convertToStorageType.TimeOfDay.ToString().ShouldEqual("23:01:01");
    }
}

using System;
using Machine.Specifications;
using Xpand.Xpo.Converters.ValueConverters;

namespace Xpand.Tests.Xpand.Xpo
{
    [Subject(typeof(SqlDateTimeOverFlowValueConverter), "Convert to storage")]
    public class When_datatime_is_less_than_1_1_1753
    {
        static DateTime _convertToStorageType;
        static DateTime _dateTime;

        Establish context = () =>
        {
            _dateTime = new DateTime(1752, 1, 1, 1, 1, 1);
        };
        Because of = () =>
        {
            _convertToStorageType = (DateTime)new SqlDateTimeOverFlowValueConverter().ConvertToStorageType(_dateTime);
        };
        It should_convert_it_1_1_1753 = () => _convertToStorageType.Date.ShouldEqual(new DateTime(1753, 1, 1));

        It should_add_the_time_of_day_to_it = () => _convertToStorageType.TimeOfDay.ShouldEqual(new TimeSpan(1, 1, 1));
    }
    [Subject(typeof(XpandUtcDateTimeConverter), "Convert to storage")]
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
            _convertToStorageType = (DateTime)new XpandUtcDateTimeConverter().ConvertToStorageType(_dateTime);
        };
        It should_convert_it_1_1_1753 = () => _convertToStorageType.Date.ShouldEqual(new DateTime(1753, 1, 1));

        It should_add_the_time_of_day_to_it = () => _convertToStorageType.TimeOfDay.ShouldEqual(new TimeSpan(0, 1, 1));
    }
    [Subject(typeof(XpandUtcDateTimeConverter), "Convert From Storage")]
    public class When_datetime_is_null
    {
        static DateTime? _convertFromStorageType;
        static DateTime _minValue;

        Establish context = () =>
        {
            _minValue = DateTime.MinValue;
        };

        Because of = () =>
        {
            _convertFromStorageType = (DateTime?)new XpandUtcDateTimeConverter().ConvertFromStorageType(_minValue);
        };

        It should_not_convert_it_to_anaything = () => _convertFromStorageType.ShouldBeNull();
    }
}

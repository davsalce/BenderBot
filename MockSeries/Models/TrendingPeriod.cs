using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSeries.Models
{
    public enum TrendingPeriod
    {
        [System.Runtime.Serialization.EnumMember(Value = @"AllTimes")]
        AllTimes = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"Today")]
        Today = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"LastWeek")]
        LastWeek = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"LastMonth")]
        LastMonth = 3,
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSeries.Models
{
    public enum TrendingPeriod
    {

        [System.Runtime.Serialization.EnumMember(Value = @"Today")]
        Today = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"LastWeek")]
        LastWeek = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"LastMonth")]
        LastMonth = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"AllTimes")]
        AllTimes = 3,
    }
}

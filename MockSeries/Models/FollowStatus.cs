using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSeries.Models
{
    public enum FollowStatus
    {

        [System.Runtime.Serialization.EnumMember(Value = @"Following")]
        Following = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"WishList")]
        WishList = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"Abandoned")]
        Abandoned = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"NotFollowing")]
        NotFollowing = 3,

    }
}

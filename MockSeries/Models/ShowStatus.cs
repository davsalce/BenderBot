namespace MockSeries.Models
{
    public enum ShowStatus
    {

        [System.Runtime.Serialization.EnumMember(Value = @"Upcoming")]
        Upcoming = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"Continuing")]
        Continuing = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"Ended")]
        Ended = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"ReturningSeries")]
        ReturningSeries = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"Planned")]
        Planned = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"Pilot")]
        Pilot = 5,

        [System.Runtime.Serialization.EnumMember(Value = @"InProduction")]
        InProduction = 6,

        [System.Runtime.Serialization.EnumMember(Value = @"Canceled")]
        Canceled = 7,

    }
}

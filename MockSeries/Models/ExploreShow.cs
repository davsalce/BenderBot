using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSeries.Models
{
    public class ExploreShow
    {
        public string? Name { get; set; }
        public string? Year { get; set; }
        public string? Overview { get; set; }
        public DayOfWeek? AirDay { get; set; }
        public ShowStatus? Status { get; set; }
        public string? Image { get; set; }
        public int? Followers { get; set; }
        public FollowStatus? UserFollowStatus { get; set; }

        public ExploreShow(string name, string year, string overview, DateTimeOffset? firstAired, DayOfWeek? airDay, ShowStatus? status, string image, int followers, FollowStatus? userFollowStatus)
        {
            Name = name;
            Year = year;
            Overview = overview;
            AirDay = airDay;
            Status = status;
            Image = image;
            Followers = followers;
            UserFollowStatus = userFollowStatus;
        }
        public ExploreShow()
        {

        }
    }
}

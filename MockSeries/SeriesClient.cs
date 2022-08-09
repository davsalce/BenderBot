using MockSeries.Models;

namespace MockSeries
{
    public class SeriesClient
    {
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Returns the Trending Shows based on a Trending Period.
        /// </summary>
        /// <param name="period">The Trending Period.</param>
        /// <param name="page">The page number.</param>
        /// <param name="limit">The number of shows per page.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<ICollection<ExploreShow>> GetTrendingShowsAsync(TrendingPeriod period, int? page = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return period switch
            {
                TrendingPeriod.Today => MockTrendingToday(),
                TrendingPeriod.LastWeek => MockTrendingLastWeek(),
                TrendingPeriod.LastMonth => MockTrendingLasMonth(),
                _ => MockTrendingLastWeekEver(),
            };
        }

        private ICollection<ExploreShow> MockTrendingLastWeekEver()
        {
            List<ExploreShow> list = new();

            ExploreShow show = new ExploreShow()
            {
                AirDay = DayOfWeek.Monday,
                Followers = 32575,
                Image = "https://static.trackseries.tv/banners/u3bZgnGQ9T01sWNhyveQz0wH0Hl_medium.jpg",
                Name = "Game of Thrones",
                Overview = "Seven noble families fight for control of the mythical land of Westeros. Friction between the houses leads to full-scale war. All while a very ancient evil awakens in the farthest north. Amidst the war, a neglected military order of misfits, the Night's Watch, is all that stands between the realms of men and icy horrors beyond.",
                Status = ShowStatus.Ended,
                UserFollowStatus = FollowStatus.Following,
                Year = "2011"
            };
            list.Add(show);

            show = new ExploreShow()
            {
                AirDay = DayOfWeek.Tuesday,
                Followers = 20476,
                Image = "https://static.trackseries.tv/banners/7WTsnHkbA0FaG6R9twfFde0I9hl_medium.jpg",
                Name = "Sherlock",
                Overview = "A modern update finds the famous sleuth and his doctor partner solving crime in 21st century London.",
                Status = ShowStatus.Ended,
                UserFollowStatus = FollowStatus.Following,
                Year = "2010"
            };
            list.Add(show);

            show = new ExploreShow()
            {
                AirDay = DayOfWeek.Wednesday,
                Followers = 20476,
                Image = "https://static.trackseries.tv/banners/49WJfeN0moxb9IPfGn8AIqMGskD_medium.jpg",
                Name = "Stranger Things",
                Overview = "When a young boy vanishes, a small town uncovers a mystery involving secret experiments, terrifying supernatural forces, and one strange little girl.",
                Status = ShowStatus.ReturningSeries,
                UserFollowStatus = FollowStatus.Following,
                Year = "2016"
            };
            list.Add(show);
            return list;
        }

        private ICollection<ExploreShow> MockTrendingToday()
        {
            List<ExploreShow> list = new();
            return list;
        }

        private ICollection<ExploreShow> MockTrendingLastWeek()
        {
            List<ExploreShow> list = new();
            return list;
        }

        private ICollection<ExploreShow> MockTrendingLasMonth()
        {
            List<ExploreShow> list = new();
            return list;
        }

    }
}
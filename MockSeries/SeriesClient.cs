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
                _ => MockTrendingEver(),
            };
        }

        private ICollection<ExploreShow> MockTrendingEver()
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

            ExploreShow show = new ExploreShow()
            {
                AirDay = DayOfWeek.Friday,
                Followers = 3082,
                Image = "https://static.trackseries.tv/banners/k5UALlcA0EnviaCUn2wMjOWYiOO_medium.jpg",
                Name = "The Simpsons",
                Overview = "Set in Springfield, the average American town, the show focuses on the antics and everyday adventures of the Simpson family; Homer, Marge, Bart, Lisa and Maggie, as well as a virtual cast of thousands. Since the beginning, the series has been a pop culture icon, attracting hundreds of celebrities to guest star. The show has also made name for itself in its fearless satirical take on politics, media and American life in general.",
                Status = ShowStatus.ReturningSeries,
                UserFollowStatus = FollowStatus.Following,
                Year = "1989"
            };
            list.Add(show);

            show = new ExploreShow()
            {
                AirDay = DayOfWeek.Wednesday,
                Followers = 761,
                Image = "https://static.trackseries.tv/banners/q54qEgagGOYCq5D1903eBVMNkbo_medium.jpg",
                Name = "The Sandman",
                Overview = "After years of imprisonment, Morpheus — the King of Dreams — embarks on a journey across worlds to find what was stolen from him and restore his power.",
                Status = ShowStatus.ReturningSeries,
                UserFollowStatus = FollowStatus.Following,
                Year = "2022"
            };
            list.Add(show);

            show = new ExploreShow()
            {
                AirDay = DayOfWeek.Tuesday,
                Followers = 7400,
                Image = "https://static.trackseries.tv/banners/zPIug5giU8oug6Xes5K1sTfQJxY_medium.jpg",
                Name = "Grey's Anatomy",
                Overview = "Follows the personal and professional lives of a group of doctors at Seattle’s Grey Sloan Memorial Hospital.",
                Status = ShowStatus.ReturningSeries,
                UserFollowStatus = FollowStatus.Following,
                Year = "2005 "
            };
            list.Add(show);
            return list;
        }

        private ICollection<ExploreShow> MockTrendingLastWeek()
        {
            List<ExploreShow> list = new();
            ExploreShow show = new ExploreShow()
            {
                AirDay = DayOfWeek.Monday,
                Followers = 178,
                Image = "https://static.trackseries.tv/banners/nz5fAg1OomLZ7fxiM4RnLbJxppc_medium.jpg",
                Name = "The Daily Show with Trevor Noah",
                Overview = "Trevor Noah and The World's Fakest News Team tackle the biggest stories in news, politics and pop culture.",
                Status = ShowStatus.ReturningSeries,
                UserFollowStatus = FollowStatus.Following,
                Year = "1996 "
            };
            list.Add(show);

            show = new ExploreShow()
            {
                AirDay = DayOfWeek.Wednesday,
                Followers = 761,
                Image = "https://static.trackseries.tv/banners/q54qEgagGOYCq5D1903eBVMNkbo_medium.jpg",
                Name = "The Sandman",
                Overview = "After years of imprisonment, Morpheus — the King of Dreams — embarks on a journey across worlds to find what was stolen from him and restore his power.",
                Status = ShowStatus.ReturningSeries,
                UserFollowStatus = FollowStatus.Following,
                Year = "2022"
            };
            list.Add(show);

            show = new ExploreShow()
            {
                AirDay = DayOfWeek.Monday,
                Followers = 4414,
                Image = "https://static.trackseries.tv/banners/qWnJzyZhyy74gjpSjIXWmuk0ifX_medium.jpg",
                Name = "The Office",
                Overview = "The everyday lives of office employees in the Scranton, Pennsylvania branch of the fictional Dunder Mifflin Paper Company.",
                Status = ShowStatus.Ended,
                UserFollowStatus = FollowStatus.Following,
                Year = "2005 "
            };
            list.Add(show);
            return list;
        }

        private ICollection<ExploreShow> MockTrendingLasMonth()
        {
            List<ExploreShow> list = new();
            ExploreShow show = new ExploreShow()
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

            show = new ExploreShow()
            {
                AirDay = DayOfWeek.Tuesday,
                Followers = 7391,
                Image = "https://static.trackseries.tv/banners/f496cm9enuEsZkSPzCwnTESEK5s_medium.jpg",
                Name = "Friends",
                Overview = "Six young people from New York City, on their own and struggling to survive in the real world, find the companionship, comfort and support they get from each other to be the perfect antidote to the pressures of life.",
                Status = ShowStatus.Ended,
                UserFollowStatus = FollowStatus.Following,
                Year = "1994"
            };
            list.Add(show);

            show = new ExploreShow()
            {
                AirDay = DayOfWeek.Wednesday,
                Followers = 1836,
                Image = "https://static.trackseries.tv/banners/posters/81797-4_medium.jpg",
                Name = "One Piece",
                Overview = "The adventures of Monkey D. Luffy and his pirate crew in order to find the greatest treasure ever left by the legendary Pirate, Gold Roger. The famous mystery treasure named \"One Piece\".",
                Status = ShowStatus.Continuing,
                UserFollowStatus = FollowStatus.Following,
                Year = "1999"
            };
            list.Add(show);
            return list;
        }

        public async Task<bool> MarkEpisodeAsWatch(string user, string series , int season, int episode)
        {
            return true;
        }
    }
}
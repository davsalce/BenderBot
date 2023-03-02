using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace TrackSeries.Core.Client
{
    public partial class FollowedShowWithPendingEpisodesViewModel
    {
        internal Attachment ToAttachment()
        {
            PendingEpisodeDto firstUnwatched = Episodes.FirstOrDefault();
            HeroCard heroCard = new()
            {
                Images = new List<CardImage>()
                {
                    new CardImage() {
                        Url = Images.Poster,
                        Alt = $"{Name} poster"
                    }
                },
                Subtitle = $"Episode {firstUnwatched.SeasonNumber}x{firstUnwatched.Number} ({Unwatched} unwatched episodes)",
                Title = Name
            };
            if (firstUnwatched.Overview is not null)
            {
                heroCard.Text = new string(firstUnwatched.Overview.Take(100).ToArray()) + (firstUnwatched.Overview.Length > 100 ? "..." : string.Empty);
            }

            string seePending = "See pending";
            heroCard.Tap = new CardAction()
            {
                DisplayText = seePending,
                Text = seePending,
                Title = seePending,
                Value = seePending + ' ' + Name,
                Type = ActionTypes.ImBack
            };
            Attachment attachment = heroCard.ToAttachment();
            return attachment;
        }
    }
}

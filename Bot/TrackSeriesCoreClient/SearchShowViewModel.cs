
using Microsoft.Bot.Schema;

namespace TrackSeries.Core.Client
{
    public partial class SearchShowViewModel
    {
        public Attachment ToAttachment(bool followButton = false)
        {
            HeroCard heroCard = new()
            {
                Buttons = new List<CardAction>(),
                Images = new List<CardImage>()
                {
                    new CardImage()
                    {
                        Url = Poster,
                        Alt = $"{Name} poster"
                    }
                },
                //Subtitle = Status.Value.ToString(),
                Tap = new CardAction() { },
                Text = new string(Overview.Take(100).ToArray()) + (Overview.Length > 100 ? "..." : string.Empty),
                Title = Name
            };

            string showDetails = "Show details of";
            heroCard.Buttons.Add(new CardAction()
            {
                DisplayText = showDetails,
                Text = showDetails,
                Title = showDetails,
                Value = showDetails + ' ' + Name,
                Type = ActionTypes.ImBack
            });

            if (followButton && UserFollowStatus.Value == FollowStatus.NotFollowing)
            {
                string followSerie = "Follow show";
                heroCard.Buttons.Add(new CardAction()
                {
                    DisplayText = followSerie,
                    Text = followSerie,
                    Title = followSerie,
                    Value = nameof(followSerie)
                });
            }

            Attachment attachment = heroCard.ToAttachment();
            return attachment;
        }
    }
}

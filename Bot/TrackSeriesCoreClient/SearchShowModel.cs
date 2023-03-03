using Microsoft.Bot.Schema;

namespace TrackSeries.Core.Client
{
    public partial class SearchShowModel
    {
        public Attachment ToAttachment()
        {
            HeroCard heroCard = new()
            {
                Buttons = new List<CardAction>(),
                Images = new List<CardImage>()
                {
                    new CardImage()
                    {
                        Url = Images.Poster,
                        Alt = $"{Name} poster"
                    }
                },
                Subtitle = Status.Value.ToString(),
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

            Attachment attachment = heroCard.ToAttachment();
            return attachment;
        }   
    }
}

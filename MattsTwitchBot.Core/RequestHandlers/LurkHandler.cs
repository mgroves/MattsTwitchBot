using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Requests;
using MediatR;
using Scriban;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class LurkHandler : IRequestHandler<Lurk>
    {
        private readonly ITwitchClient _twitchClient;
        private readonly Random _random;

        public LurkHandler(ITwitchClient twitchClient)
        {
            _twitchClient = twitchClient;
            _random = new Random();
        }

        public async Task<Unit> Handle(Lurk request, CancellationToken cancellationToken)
        {
            var channel = request.ChatMessage.Channel;

            var lurkTemplates = new List<string>
            {
                "Thank you for lurking, @{{username}}",
                "Just lurking today, @{{username}}?",
                "Hey everyone, @{{username}} is lurking!",
                "Just call @{{username}} Lurky McLurkerson",
                "I like the way you lurk it, @{{username}}, no diggity.",
                "@{{username}}'s lurking for the weekend."
            };

            // pick a template at random
            var template = lurkTemplates[_random.Next(lurkTemplates.Count)];

            // parse template with razor
            var username = request.ChatMessage.Username;

            var result = await Template.Parse(template).RenderAsync(new {Username = username});

            _twitchClient.SendMessage(channel, result);

            return default;
        }
    }
}
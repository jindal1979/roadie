﻿using Microsoft.AspNetCore.SignalR;
using Roadie.Library.Models;
using System.Threading.Tasks;

namespace Roadie.Api.Hubs
{
    public class PlayActivityHub : Hub
    {
        public async Task SendActivity(PlayActivityList playActivity)
        {
            await Clients.All.SendAsync("PlayActivity", playActivity);
        }
    }
}
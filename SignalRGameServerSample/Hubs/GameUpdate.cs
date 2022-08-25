using Microsoft.AspNetCore.SignalR;
using SignalRGameServerSample.Models;

namespace SignalRGameServerSample.Hubs
{
    public class GameUpdate
    {   
        //update delay in milisecond
        private const int UPDATE_DELAY_MILISECOND = 200;
        private readonly IHubContext<GameHub> _gameHubContext;
        private readonly Helper _helper;
        public GameUpdate(IHubContext<GameHub> context, Helper helper)
        {
            _gameHubContext = context;
            _helper = helper;
            Start();
        }
        public void Start()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    // send player informations of each game to all game group member
                    foreach (var gameId in _helper.Games)
                    {
                        var update = new Update();
                        update.PlayerInformations = _helper.Players.Values.Where(x => x.GameId == gameId).ToList();
                        _ = _gameHubContext.Clients.Group(gameId).SendAsync("GameUpdate", update);
                    }
                    await Task.Delay(UPDATE_DELAY_MILISECOND);
                }
            });
        }
    }
}

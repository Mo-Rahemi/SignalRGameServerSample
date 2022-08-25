using Microsoft.AspNetCore.SignalR;
using SignalRGameServerSample.Models;
using System.Xml.Linq;

namespace SignalRGameServerSample.Hubs
{
    public class GameHub : Hub
    {
        //update delay in milisecond
        private const int UPDATE_DELAY_MILISECOND = 200;
        private readonly Helper _helper;
        //dependency inject our singletone helper
        public GameHub(Helper helper)
        {
            _helper = helper;
        }
        //create player information on connection start
        public override Task OnConnectedAsync()
        {
            _helper.AddPlayer(Context.ConnectionId, new PlayerInformation() { Id = Context.ConnectionId });
            return base.OnConnectedAsync();
        }
        public async Task<Update> JoinGame(string name, string gameId)
        {
            //check if player information exist
            if (!_helper.Players.TryGetValue(Context.ConnectionId, out var player)) return null;
            //check if player is in a game already
            if (player.GameId == gameId || player.GameId != "") return null;
            //set player name
            player.Name = name;
            //check if game exist or started now
            if (!_helper.Games.Contains(gameId))
                _helper.AddGame(gameId);
            //add player to game group
            await Clients.Group(gameId).SendAsync("PlayerJoinGame", player);
            //notification to all player in game that a new player connect
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            //set player game id
            player.GameId = gameId;
            //return all current player informations in the game to new joined player
            return new Update() { PlayerInformations = _helper.Players.Values.Where(x => x.Id != Context.ConnectionId && x.GameId == gameId).ToList() };
        }
        public async Task LeaveGame()
        {
            //check if player information exist
            if (!_helper.Players.TryGetValue(Context.ConnectionId, out var player)) return;
            //remove player from game group
            await Groups.RemoveFromGroupAsync(player.Id, player.GameId);
            //check if all player on the group left then remove group
            if (_helper.Players.Count(x => x.Value.GameId == player.GameId) == 0)
                _helper.RemoveGame(player.GameId);
            //else if there is player in game say to all this player left game
            else
                await Clients.Group(player.GameId).SendAsync("PlayerLeftGame", player.Id);
            //set player game id to none
            player.GameId = "";
        }
        public async Task SendMessageToAll(string message)
        {
            //check if player information exist
            if (!_helper.Players.TryGetValue(Context.ConnectionId, out var player)) return;
            //send chat message to all player in his/her game group
            await Clients.Group(player.GameId).SendAsync("Notification", new Notification()
            {
                Id = Context.ConnectionId,
                Message = message,
                Name = player.Name
            });
        }
        public void Move(float x, float y, float z, float lookY)
        {
            //check if player information exist
            if (!_helper.Players.TryGetValue(Context.ConnectionId, out var player)) return;
            //move player and change look direction
            player.Position = new vector3(x, y, z);
            player.LookAt = lookY;
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            //remove player from game and server if disconnect
            await LeaveGame();
            _helper.RemovePlayer(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        public void Start()
        {
            //start game update with 200 ms delay between each update
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    // send player informations of each game to all game group member
                    foreach (var gameId in _helper.Games)
                    {
                        var world = new Update();
                        world.PlayerInformations = _helper.Players.Values.Where(x => x.GameId == gameId).ToList();
                        _ = Clients.Group(gameId).SendAsync("GameUpdate", world);
                    }
                    await Task.Delay(UPDATE_DELAY_MILISECOND);
                }
            });
        }
    }
}

namespace SignalRGameServerSample.Models
{
    public class Helper
    {
        public IReadOnlyCollection<string> Games => _games;
        public IReadOnlyDictionary<string, PlayerInformation> Players => _players;

        public void AddGame(string name)
        {
            lock (_games)
            {
                if (!_games.Contains(name))
                    _games.Add(name);
            }
        }
        public void AddPlayer(string id, PlayerInformation information)
        {
            lock (_players)
            {
                if (!_players.ContainsKey(id))
                    _players.Add(id, information);
            }
        }
        public bool RemoveGame(string name)
        {
            lock (_games)
            {
                return _games.Remove(name);
            }
        }
        public PlayerInformation RemovePlayer(string id)
        {
            lock (_players)
            {
                _players.Remove(id, out var player);
                return player;
            }
        }

        private readonly List<string> _games = new();
        private readonly Dictionary<string, PlayerInformation> _players = new();
    }
}

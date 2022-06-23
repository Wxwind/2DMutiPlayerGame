namespace Game
{
    public class PlayerUI
    {
        public string Ip { get; set; }
        public string PlayerName { get; set; }
        public int Hp { get; set; }
        public bool IsLocalPlayer { get; set; }

        public PlayerUI(string ip, string playerName,int hp,bool isLocalPlayer)
        {
            Ip = ip;
            PlayerName = playerName;
            Hp = hp;
            IsLocalPlayer = isLocalPlayer;
        }
        
        public PlayerUI(string ip, string playerName)
        {
            Ip = ip;
            PlayerName = playerName;
        }

        public PlayerUI()
        {
            
        }
    }
}
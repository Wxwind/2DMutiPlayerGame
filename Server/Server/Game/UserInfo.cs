using System.Net;
using System.Security.Authentication.ExtendedProtection;

namespace WX.Game;

public class UserInfo
{
    public string PlayerName { get; set; }
    //public IPEndPoint Ip { get; set; }

    public UserInfo(string playerName)
    {
        PlayerName = playerName;
    }
    
    public bool Equals(string name)
    {
        return PlayerName == name;
    }
}
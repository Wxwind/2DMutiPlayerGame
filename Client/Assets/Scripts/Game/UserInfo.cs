namespace Game
{
    public class UserInfo
    {
        public string UserName { get; set; }

        public UserInfo(string userName)
        {
            UserName = userName;
        }

        public bool Equals(string name)
        {
            return UserName == name;
        }
    }
}
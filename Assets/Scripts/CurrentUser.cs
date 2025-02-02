public class CurrentUser
{
    public static CurrentUser Instance { get; private set; }
    public string Username { get; private set; }
    private CurrentUser() { }

    public static void Initialize(string username)
    {
            Instance = new CurrentUser
            {
                Username = username,
            };
    }
}

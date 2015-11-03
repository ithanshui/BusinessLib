namespace BusinessLib.BasicAuthentication
{
    public interface IToken
    {
        //user data
        System.String Key { get; set; }
        //front clinet
        System.String IP { get; set; }
    }
}

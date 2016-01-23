namespace Business.Authentication
{
    public interface IInterception
    {
        System.Collections.Concurrent.ConcurrentDictionary<string, Business.Extensions.InterceptorCommand> Command { get; set; }

        Business.IBusiness Business { get; set; }
    }
}

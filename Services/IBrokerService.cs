namespace AuthorizationServer.Services
{
    public interface IBrokerService
    {
        object GetMessage();
        object SetMessage(object obj);
        byte[] GetBody();
        object SetBody(object obj);
        void PublishMessageNewUserCreated();
    }
}
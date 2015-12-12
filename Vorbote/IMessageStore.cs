namespace Vorbote
{
    using Vorbote.Models;

    public interface IMessageStore
    {
        void SaveMessage(IMailMessage message);
    }
}

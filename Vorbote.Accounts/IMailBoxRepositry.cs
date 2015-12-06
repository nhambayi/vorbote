namespace Vorbote.Accounts
{
    public interface IMailBoxRepositry
    {
        MailBox CreateMailBox(string username, string password);
        void DeleteMailbox(string username);
        MailBox GetMailBox(string username, string password);
        MailBox UpdateMailBox(string username);
    }
}
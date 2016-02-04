namespace Vorbote.Mailboxes.SqlServer
{
    using System;
    using Dapper;
    using Dapper.Extensions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.SqlClient;
    using System.Configuration;
    using System.Data;

    public class SqlServerMailboxRepositry 
    {

        public static void AddNewMailbox(IMailbox mailbox)
        {
            using (var connection = GetDbConnection())
            {
                
            }
        }

        public static MailBox GetMailbox(string username)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["mailboxesDatabase"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    var query = "select * from mailboxes where username = @Username";
                    var mailBoxes = connection.Query<MailBox>(query, new { Username = username });
                    return mailBoxes.FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static IDbConnection GetDbConnection()
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["mailboxesDatabase"].ConnectionString;
                return new SqlConnection(connectionString);

            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public static string VerifyUser(string username, string password)
        {
            var mailbox = GetMailbox(username);
            if (mailbox != null)
            {
                if (password == mailbox.Password)
                    return username;
                else
                    return null;
            }
            else
            {
                return null;
            }
        }


        public static void AddMailMessage(MailBox mailbox, MailboxMessage message)
        {
            using (var connection = GetDbConnection())
            {
                var query = "INSERT INTO [dbo].[MailboxMessages] "
                    + "([MessageId] ,[MailboxId] ,[RawHeader] ,[Body] ,[MessageUrl])"
                    + " VALUES ( @MessageId, @MailboxId , @RawHeader, @Body , @MessageUrl)";

                connection.Query(query, new
                {
                    message.MessageId,
                    message.MailboxId,
                    message.RawHeader,
                    message.Body,
                    message.MessageUrl
                });
            }
        }
    }
}

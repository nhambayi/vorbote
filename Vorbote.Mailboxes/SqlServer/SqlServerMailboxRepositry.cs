namespace Vorbote.Mailboxes.SqlServer
{
    using System;
    using Dapper;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
                var connectionString = ConfigurationManager.ConnectionStrings["accountsDatabase"].ConnectionString;
                return new SqlConnection(connectionString);

            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

    }
}

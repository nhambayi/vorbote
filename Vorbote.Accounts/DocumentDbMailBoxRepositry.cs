namespace Vorbote.Accounts
{
    using Microsoft.Azure.Documents.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Linq;
    using Newtonsoft.Json;
    using System.IO;

    public class DocumentDbMailBoxRepositry
    {
        private DocumentClient _documentDbClient;
        DocumentCollection _documentCollection;
        private Database _userDatabase;
        private readonly string DB_ENDPOINT_URL = "https://vorbote-dev.documents.azure.com:443/";
        private readonly string DB_AUTHORIZATON_KEY = "Cyzzth/h+2qA/uGvpa9NWuS8PiHoxQDrWMIJSehSg54vwA366VkitSBc6EsfEby2FIStd+mpbDg6pu+Lxv8esQ==";
        private readonly string USER_DATABASE_NAME = "Users";
        private readonly string USER_COLLECTION_NAME = "UsersCollection";

        public DocumentDbMailBoxRepositry()
        {
            _documentDbClient = new DocumentClient(new Uri(DB_ENDPOINT_URL), DB_AUTHORIZATON_KEY);
        }

        private string UserCollectionUrl
        {
            get
            {
                return string.Format("dbs/{0}/colls/{1}", _userDatabase.Id, _documentCollection.Id);
            }
        }

        public async Task Initialize()
        {
            await CreateDb();
        }

        private async Task CreateDb()
        {
            _userDatabase = _documentDbClient.CreateDatabaseQuery()
                            .Where(db => db.Id == USER_DATABASE_NAME)
                            .AsEnumerable()
                            .FirstOrDefault();

            if (_userDatabase == null)
            {
                _userDatabase = await _documentDbClient.CreateDatabaseAsync(
                    new Database
                    {
                        Id = USER_DATABASE_NAME
                    });
            }

            _documentCollection = _documentDbClient
                .CreateDocumentCollectionQuery("dbs/" + _userDatabase.Id)
                .Where(c => c.Id == USER_COLLECTION_NAME)
                .AsEnumerable()
                .FirstOrDefault();

            if (_documentCollection == null)
            {
                _documentCollection = await _documentDbClient
                    .CreateDocumentCollectionAsync("dbs/" + _userDatabase.Id,
                    new DocumentCollection
                    {
                        Id = USER_COLLECTION_NAME
                    });
            }
        }

        public async void CreateMailBox(string username, string password)
        {
            MailBox mailBox = new MailBox()
            {
                id = username,
                Username = username,
                Password = password
            };

            await _documentDbClient.CreateDocumentAsync(UserCollectionUrl, mailBox);
        }

        public async void DeleteMailbox(string username)
        {

        }

        public MailBox GetMailBox(string username)
        { 
            var document = _documentDbClient.CreateDocumentQuery(UserCollectionUrl)
                .Where(d => d.Id == username)
                .AsEnumerable()
                .FirstOrDefault();

            var deserializer = new JsonSerializer();
            if (document != null)
            {
                MailBox mb = deserializer.Deserialize<MailBox>(new JsonTextReader(new StringReader(document.ToString())));
                return mb;
            }
            else
            {
                return null;
            }
        }

        public MailBox UpdateMailBox(string username)
        {
            return null;
        }

        public void AddMailItem()
        {

        }

        public void DeleteMailItem()
        {

        }
    }
}
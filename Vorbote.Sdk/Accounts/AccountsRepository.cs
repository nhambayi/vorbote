namespace Vorbote.Accounts
{
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AccountsRepository
    {
        private string _endpointUrl;
        private string _authkey;
        private DocumentClient _client;
        private Database _database;
        private DocumentCollection _documentCollection;
        private static AccountsRepository _instance;

        private const string DATABASE_NAME = "moqmail";
        private const string COLLECTION_NAME = "moqmail";

        public static AccountsRepository Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new AccountsRepository();
                }

                return _instance;
            }
        }

        public AccountsRepository()
        {
            Init();
        }

        private void Init()
        {
            InitializeClient();
            InitializeDatabase();
            InitializeCollection();
        }

        public MoqMailAccount GetAccount(string accountId)
        {
            var result =
                from f in _client.CreateDocumentQuery<MoqMailAccount>("dbs/" + _database.Id + "/colls/" + _documentCollection.Id)
                where f.AccountId == accountId
                select f;

           List< MoqMailAccount> account = null;

            if(result != null)
            {
                account = result.ToList();
            }

            return account.FirstOrDefault();
        }

        private void InitializeClient()
        {
            _endpointUrl = @"https://vorbote.documents.azure.com:443/";
            _authkey = "U3iYkOkwYT24V74NYh/63BWUx7dikhGlU5GELADO7SKkHseF76taY3RcCJN0IQGEzwvXsABn8myMNnr2bPNdWg==";
            _client = new DocumentClient(new Uri(_endpointUrl), _authkey);
        }

        private async void InitializeDatabase()
        {
            // Check to verify a database with the id=FamilyRegistry does not exist
            _database = _client.CreateDatabaseQuery().Where(db => db.Id == DATABASE_NAME)
                .AsEnumerable().FirstOrDefault();

            // If the database does not exist, create a new database
            if (_database == null)
            {
                _database = await _client.CreateDatabaseAsync(
                    new Database
                    {
                        Id = DATABASE_NAME
                    });
            }
        }

        private async void InitializeCollection()
        {
             _documentCollection = _client.CreateDocumentCollectionQuery("dbs/" + _database.Id)
                .Where(c => c.Id == COLLECTION_NAME).AsEnumerable().FirstOrDefault();

            // If the document collection does not exist, create a new collection
            if (_documentCollection == null)
            {
                _documentCollection = await _client.CreateDocumentCollectionAsync("dbs/" + _database.Id,
                    new DocumentCollection
                    {
                        Id = COLLECTION_NAME
                    });
            }
        }


    }
}

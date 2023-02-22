using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Simple.Models;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net;
using System.Text;

namespace Simple.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        protected MongoClient Client { get; set; }
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            /*
            DateTime dt = DateTime.Now;
            StringBuilder sb = new StringBuilder();

            sb.Append("Index ");
            sb.Append(dt.ToString());

            _logger.Log(LogLevel.Trace, sb.ToString());

            System.Net.IPAddress[] ipAddresses = Dns.GetHostAddresses("mongo");
            string? connectionString;
            foreach (IPAddress ip in ipAddresses)
            {
                if (connectionString.DefaultIfEmpty = "")
                    connectionString = "mongodb://";
                else
                    connectionString += ",";
                connectionString += $"{ip.ToString()}:27017";
            }
            connectionString += "/database";
            */

            // HttpContext hContext = ControllerContext.HttpContext;
            return View();
        }

            
            
         


        /*
         admin
        {
          "connectionString.standard": "mongodb://my-user:Frazz1234@mdb0-0.mdb0-svc.mongodb.svc.cluster.local:27017,mdb0-1.mdb0-svc.mongodb.svc.cluster.local:27017/admin?replicaSet=mdb0&ssl=false",
          "connectionString.standardSrv": "mongodb+srv://my-user:Frazz1234@mdb0-svc.mongodb.svc.cluster.local/admin?replicaSet=mdb0&ssl=false",
          "password": "Frazz1234",
          "username": "my-user"
        }

        {
          "connectionString.standard": "mongodb://appuser:Frazz1234@mdb0-0.mdb0-svc.mongodb.svc.cluster.local:27017,mdb0-1.mdb0-svc.mongodb.svc.cluster.local:27017/admin?replicaSet=mdb0&ssl=false",
          "connectionString.standardSrv": "mongodb+srv://appuser:Frazz1234@mdb0-svc.mongodb.svc.cluster.local/admin?replicaSet=mdb0&ssl=false",
          "password": "Frazz1234",
          "username": "appuser"
        }

        */

        public async Task<IActionResult> Privacy()
        {

            _logger.Log(LogLevel.Information, "be4 the client");
            _logger.Log(LogLevel.Information, "be4 listDatabase");
            // Client = new MongoClient("mongodb://root:rootpassword@mongodb-container.default.svc.cluster.local:27017");
            // can be run in cluster  --- deployment - service
            Client = new MongoClient("mongodb://mongodb-replica-0.mongo.default.svc.cluster.local:27017," +
                "mongodb-replica-1.mongo.default.svc.cluster.local:27017," +
                "mongodb-replica-2.mongo.default.svc.cluster.local:27017");  // has to be run on cluster

           
            //Client = new MongoClient("mongodb://root:rootpassword@192.168.1.100:27017");

            /*

            _logger.Log(LogLevel.Information, "Get Database");
            Client.DropDatabase("hibigglus");
 
            var database = Client.GetDatabase("hibigglus");


            _logger.Log(LogLevel.Information, "After Get Database");
            #region Typed classes commands
            _logger.Log(LogLevel.Information, "Create Collection");
            // Will create the users collection on the fly if it doesn't exists
            var personsCollection = database.GetCollection<User>("subblistic"); // Constants.UsersCollection);
            _logger.Log(LogLevel.Information, "After Create Collection");
             
            var typedUser = RandomData.GenerateUsers(10);
          

            _logger.Log(LogLevel.Information, "Be4 Insert Collection");
            await personsCollection.InsertManyAsync(typedUser);
            _logger.Log(LogLevel.Information, "After insert Collection");
 
            // sb.ToString());
            */


            


            var typedUser = RandomData.GenerateUsers(10);
            //Will create the users collection on the fly if it doesn't exists
            var database = Client.GetDatabase("hibigglus");
            var userCollection = database.GetCollection<User>("subblistic"); // Constants.UsersCollection);
            _logger.Log(LogLevel.Information, "After Create Collection");
            _logger.Log(LogLevel.Information, "Be4 Insert Collection");
            await userCollection.InsertManyAsync(typedUser);
            _logger.Log(LogLevel.Information, "After insert Collection");


            _logger.Log(LogLevel.Information, "be4 listDatabase");
            var databases = await Client.ListDatabasesAsync();
            _logger.Log(LogLevel.Information, "After ListDatabase");

            StringBuilder sb111 = new StringBuilder();
            bool isOKK = false;
            while (databases.MoveNext())
            {
                var currentBatch = databases.Current;
                //Utils.Log(currentBatch.AsEnumerable(), "List databases");
                foreach (BsonDocument s in currentBatch)
                {
                    isOKK = true;
                    sb111.Append(s.ToString());
                    _logger.Log(LogLevel.Information, sb111.ToString());
                }
            }
            ViewData["goodcall"] = sb111.ToString();

            if (isOKK)
            {
                _logger.Log(LogLevel.Information, "DID   FIND DB");
                ViewData["goodcall"] = "found   Data";
                ViewData["rsname"] = sb111.ToString();
            }
            else
            {
                _logger.Log(LogLevel.Information, "DID NO FIND DB");
                ViewData["goodcall"] = "found NO Data";
                ViewData["rsname"] = "squat";
            }



            return View(typedUser);
        }



         
        public async Task<IActionResult> GetAll()
        {

            Client = new MongoClient("mongodb://root:rootpassword@192.168.1.100:27017");
            var database = Client.GetDatabase("hibigglus");
            // Will create the users collection on the fly if it doesn't exists
            database.DropCollection("users");

            var personsCollection = database.GetCollection<User>("users"); //.UsersCollection);

            User usr = new User();
            //usr.Id = "000000000000000000000000";

            // Insert multiple documents
            var persons = RandomData.GenerateUsers(30);

            await personsCollection.InsertManyAsync(persons);

          //  var personFilter = Builders<User>.Filter.All(persons);
           
        //    var personFindResult = await personsCollection.Find(personFilter); //.Find();

            return  View( );
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
    
}
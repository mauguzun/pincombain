using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace PinCombain
{
    class Model
    {

        private static Model instance;

        private Model()
        { LoadCollection(); }

        public static Model GetInstance()
        {
            if (instance == null)
            {
                instance = new Model();

            }

            return instance;
        }

        IMongoCollection<User> users;
        public void LoadCollection()
        {

            string connectionString = "mongodb+srv://denis:penis@denispenis-gv66s.mongodb.net/test?retryWrites=true";

            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase database = client.GetDatabase("user");
            this.users = database.GetCollection<User>("users");

        }
        public User FindUser(string name)
        {
            return this.users.Find(x => x.Name == name).FirstOrDefault();


        }
        public bool Add(User cap)
        {
            try
            {
                this.users.InsertOne(cap);
                this.LoadCollection();
                return true;
            }
            catch
            {
                return false;
            }

        }
    }

}

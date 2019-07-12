using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            /* Esempio utilizzo DBManager
            DBManager dbManager = new DBManager();

            DataTable result = dbManager.Select(TableNames.usersTable);

            foreach (DataRow row in result.Rows) {
                Console.WriteLine($"ID: {row["user_id"]}\nUsername: {row["username"]}\nPassword: {row["password"]}");
            }
            */

            Server myServer = new Server();
  
        }
    }
}

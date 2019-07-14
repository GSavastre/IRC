using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections;
using System.Runtime.Serialization;

namespace Server {
    /// <summary>
    /// Classe per la manipolazione dei dati su un DB.
    /// </summary>
    public class DBManager {

        /// <summary>
        /// Stringa di connessione visualizzabile nel file <c>App.config</c>
        /// </summary>
        public string ConnectionStringVal{ get;}

        
        private readonly MySqlConnection connection;

        /// <summary>
        ///  Genera un oggetto per la connessione al DB
        /// </summary>
        /// <param name="connectionStringName">
        ///     Nome del database a cui connettersi
        /// </param>
        public DBManager(string connectionStringName = "IRC") {
            ConnectionStringVal = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            connection = new MySqlConnection(ConnectionStringVal);
        }

        /// <summary>
        ///  Tenta di aprire una connessione al DB
        /// </summary>
        /// <returns>
        ///     <see cref="bool"/> esito della connessione
        /// </returns>
        private bool OpenConnection() {
            try {
                this.CloseConnection();
                connection.Open();
                return true;
            } catch (MySqlException ex){

                switch (ex.Number) {
                    case 0:
                        Console.WriteLine("Can't connect to server.");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid credentials");
                        break;

                    default:
                        Console.WriteLine(ex.Message);
                        break;
                }

                return false;
            }
        }

        /// <summary>
        /// Chiude la connessione dell'ogetto al DB
        /// </summary>
        /// <returns>
        ///    <see cref="bool"/> esito della chiusura della connessione.
        /// </returns>
        private bool CloseConnection() {
            try {
                connection.Close();
                return true;
            } catch (MySqlException ex) {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        ///  Cerca l'esistenza della tabella nel DB
        /// </summary>
        /// <param name="table">
        ///     Nome della tabella da ricercare
        /// </param>
        /// <returns>
        ///     <see cref="bool"/> esito ricerca.
        /// </returns>
        private bool TableExists(string table) {
            string tableProbeQuery = $"SELECT count(*) FROM information_schema.tables WHERE table_name = '{table}';";

            //Test if the table exists
            if (this.OpenConnection()) {

                MySqlCommand command = new MySqlCommand(tableProbeQuery, connection);

                //Check table
                if (int.Parse(command.ExecuteScalar()+"") > 0){
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///  Cerca se una lista di colonne esistono in una data tabella.
        /// </summary>
        /// <param name="table">
        ///     Nome della tabella in cui bisogna ricercare le colonne.
        /// </param>
        /// <param name="columnNames">
        ///     <see cref="List{string}"/> nome colonne da ricercare.
        /// </param>
        /// <returns>
        ///     <see cref="bool"/> esito ricerca.
        /// </returns>
        /// <remarks>
        ///     Viene chiamata anche <see cref="TableExists(string)"/>.
        /// </remarks>
        private bool ColumnsExists(string table, List<string> columnNames) {

            if (this.OpenConnection() && this.TableExists(table)) {
                MySqlCommand command = new MySqlCommand();
                bool nameIsValid = false;
                command.Connection = connection;

                foreach (string columnName in columnNames) {
                    string columnProbeQuery = $"SHOW COLUMNS FROM `{table}` LIKE '{columnName}';";
                    command.CommandText = columnProbeQuery;
                    string commandResult = command.ExecuteScalar() + "";
                    if ( commandResult != "") {
                        nameIsValid = true;
                    } else {
                        nameIsValid = false;
                    }
                }

                return nameIsValid;
            }

            return false;
        }

        /// <summary>
        ///  Inserisce all'interno della tabella una nuova riga.
        /// </summary>
        /// <param name="table">
        ///     Tabella da popolare.
        /// </param>
        /// <param name="parameters">
        /// <see cref="Dictionary{string, string}"/> con associazione nome attributo e valore attributo.
        /// <para>
        ///     Esempio <c>parameters</c>
        ///     <code>
        ///         Insert("user",Dictionary<string, string> = {{"username","NuovoUsername"}}).
        ///     </code>
        ///     La query diventerà...
        ///     <code>
        ///         query = "INSERT INTO user (username) VALUES (NuovoUsername)".
        ///     </code>
        /// </para>
        /// </param>
        public void Insert(string table, Dictionary<string, string> parameters) {

            List<string> parameterNames = new List<string>();
            List<string> parameterValues = new List<string>();

            foreach (KeyValuePair<string,string> parameter in parameters) {
                parameterNames.Add(parameter.Key);
                parameterValues.Add("'"+parameter.Value+"'");
            }

            if (this.OpenConnection() && this.ColumnsExists(table, parameterNames)) {
                //MySQL does the casting from string to int when necessary
                string query = $"INSERT INTO {table} ({string.Join(", ", parameterNames)}) VALUES({string.Join(", ", parameterValues)});";
                MySqlTransaction transaction = connection.BeginTransaction();
                MySqlCommand cmd = new MySqlCommand(query, connection, transaction);

                try {
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                } catch (Exception) {

                    try {
                        transaction.Rollback();
                    } finally {
                        this.CloseConnection();
                    }

                }
                this.CloseConnection();
            }

            this.CloseConnection();
        }
        /// <summary>
        ///  Aggiorna i valori di una riga della tabella in base al parametro passato.
        /// </summary>
        /// <param name="table">
        ///     Tabella su cui bisogna aggiornare i dati.
        /// </param>
        /// <param name="attributes">
        ///     Gli attributi da aggiornare.
        /// </param>
        /// <param name="parameter">
        ///     Il parametro usato per la ricerca della riga corretta.
        /// </param>
        /// <remarks>
        /// <para>
        ///     Esempio <c>attributes</c> e <c>parameter</c>
        ///     <code>
        ///         Update("user",Dictionary<string, string> = {{"username","NuovoUsername"}}, Dictionary<string, string> = {{"username","VecchioUsername"}}).
        ///     </code>
        ///     La query diventerà...
        ///     <code>
        ///         query = "UPDATE user SET username = 'NuovoUsernamae' WHERE username = 'VecchioUsername'".
        ///     </code>
        /// </para>
        /// </remarks>
        public void Update(string table, Dictionary<string, string> attributes, Dictionary<string,string> parameter) {
            //string query = "UPDATE tableinfo SET name='Joe', age='22' WHERE name='John Smith'";
            string queryString = "UPDATE ";
            List<string> attributeNames = new List<string>();
            List<string> attributeValues = new List<string>();
            string parameterName = parameter.Keys.First();
            string parameterValue = parameter.Values.First();

            List<string> columnParameterCheck = new List<string>();
            columnParameterCheck.Add(parameterName);


            foreach (KeyValuePair<string, string> attribute in attributes) {
                attributeNames.Add(attribute.Key);

                //AttributeValues {Key = "name"}{Value = "Joe"} => "name='Joe'"
                attributeValues.Add(attribute.Key+"='"+attribute.Value+"'");
            }


            if (this.OpenConnection() && this.ColumnsExists(table,columnParameterCheck) && this.ColumnsExists(table, attributeNames)) {

                queryString += $"{table} SET {string.Join(", ", attributeValues)} WHERE {parameterName+"='"+parameterValue+"';"}";

                MySqlTransaction transaction = connection.BeginTransaction();
                MySqlCommand command = new MySqlCommand(queryString, connection, transaction);

                try {
                    command.ExecuteNonQuery();
                    transaction.Commit();
                } catch (Exception) {
                    try {
                        transaction.Rollback();
                    } finally {
                        this.CloseConnection();
                    }
                }
            }

            this.CloseConnection();
        }

        /// <summary>
        ///  Cancella definitivamente una riga da una tabella.
        /// </summary>
        /// <param name="table">
        ///     La tabella da cui cancellare la riga.
        /// </param>
        /// <param name="parameter">
        ///     Il parametro usato per la ricerca della riga
        /// </param>
        /// <remarks>
        ///     Questo metodo non implementa SoftDeletes e cancellerà definitivamente la riga all'interno della tabella, questo metodo può essere prono ad errori date le relazioni delle tabelle tra loro.
        /// </remarks>
        public void Delete(string table, Dictionary<string, string> parameter) {

            if (this.OpenConnection()) {
                string queryString = "DELETE FROM ";

                string parameterName = parameter.Keys.First();
                string parameterValue = parameter.Values.First();
                List<string> columnParameterCheck = new List<string>();

                columnParameterCheck.Add(parameterName);
                    
                //Check if the column name exists... it also checks for the existance of the table
                if (this.ColumnsExists(table,columnParameterCheck)) {

                    queryString += $"{table} WHERE {parameterName}='{parameterValue}'";

                    MySqlTransaction transaction = connection.BeginTransaction();
                    MySqlCommand command = new MySqlCommand(queryString, connection, transaction);

                    try {
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    } catch (Exception) {

                        try {
                            transaction.Rollback();
                        } finally {
                            this.CloseConnection();
                        }
                    }
                }

                this.CloseConnection();
            }
        }

        /// <summary>
        ///  Seleziona tutti i dati di tutte le righe di una tabella
        /// </summary>
        /// <param name="table">
        ///     Tabella su cui effettuare la selezione.
        /// </param>
        /// <returns>
        ///     <see cref="DataTable"/> contenente molteplici <see cref="DataRow"/> con il risultato della selezione.
        /// </returns>
        public DataTable Select(string table) {

            if (this.OpenConnection() && this.TableExists(table)) {

                string query = $"SELECT * FROM {table}";

                MySqlCommand command = new MySqlCommand(query, connection);

                MySqlDataAdapter da = new MySqlDataAdapter();
                da.SelectCommand = command;

                //Continue with storing the result in a table and returning it to the Login() method

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                da.Fill(ds, table);
                dt = ds.Tables[table];

                this.CloseConnection();

                return dt;
            }

            return null;
        }

        /// <summary>
        ///  Esegui una query personale sulla tabella
        /// </summary>
        /// <param name="table">
        ///     Tabella su cui effettuare la query.
        /// </param>
        /// <param name="queryString">
        ///  Query <see cref="string"/>.
        /// </param>
        /// <returns>
        ///     <see cref="DataTable"/> contenente molteplici <see cref="DataRow"/> con il risultato della query.
        /// </returns>
        public DataTable Query(string table, string queryString) {
            if (this.OpenConnection()) {
                MySqlCommand command = new MySqlCommand(queryString, connection);
                MySqlDataAdapter da = new MySqlDataAdapter();

                da.SelectCommand = command;

                //Continue with storing the result in a table and returning it to the Login() method

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                da.Fill(ds, table);
                dt = ds.Tables[table];

                this.CloseConnection();

                return dt;
            }

            return null;
        }

        /*
         * Come usare un oggetto di tipo DataTable:
         * DataTable è un oggetto simile ad una matrice che però ha la struttura della tabella SQL interrogata
         * 
         * Come in una matrice i dati possono essere accessi usando -> DataTable nomeTabella[0][0] etc...
         * Dove viene accessa la prima riga e la prima colonna
         * 
         * Oppure si può usare -> DataTable nomeTabella[0]["id"] etc...
         * Dove viene accessa la prima riga e la colonna chiamata "id"
         * 
         * Questo però presuppone che si sappia il numero di righe del risultato, perché se dalla query non escono fuori risultati
         * usare nomeTabella[0][etc...] da errore OutOfIndexException.
         * 
         * Per sapere il numero di righe del risultato si può usare nomeTabella.Rows.Count
         * 
         * Oppure si può usare un foreach che automatizza un po il tutto
         * 
         * foreach(DataRow row in tabella.Rows){
         *      int idUtente = row["id"];
         * }
         * 
         * Così viene ciclata ogni riga della tabella e si prende la colonna "id"
         */
    }
}

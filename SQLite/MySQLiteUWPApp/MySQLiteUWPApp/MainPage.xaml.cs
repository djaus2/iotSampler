using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SQLite;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MySQLiteUWPApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Supposed to check if db file exists
        /// </summary>
        /// <param name="DatabaseName"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> DoesDbExist(string DatabaseName)
        {
            bool dbexist = true;
            try
            {
                Windows.Storage.StorageFile storageFile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(DatabaseName);

                if (storageFile == null)
                    dbexist = false;
            }
            catch
            {
                dbexist = false;
            }

            return dbexist;
        }
        

        public async System.Threading.Tasks.Task<bool> CreateDB()
        {
            SQLite.SQLiteAsyncConnection connection;
            bool res = await DoesDbExist(textBox.Text);
            res = true;
            if (!res)
            {
                SQLite.SQLiteOpenFlags sqlflg = SQLite.SQLiteOpenFlags.Create;
                connection = new SQLite.SQLiteAsyncConnection(textBox.Text, sqlflg, false);
            }
            else
            {
                connection = new SQLite.SQLiteAsyncConnection(textBox.Text);
            }
            await connection.CreateTableAsync<Sensor>();


            return res;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.IsEnabled = false;
            string content = button.Content.ToString();
            string[] buttonContents = content.Split(new char[] { ' ' });
            switch (buttonContents[1])
            {
                //For these the verb is last
                case "Insert":
                    Insert(buttonContents[0]);
                    break;
                case "Update":
                    Update(buttonContents[0]);
                    break;
                case "Delete":
                    Delete(buttonContents[0]);
                    break;
                default:
                    // For these the verb is first
                    switch (content)
                    {
                        case "Query Database":
                            QueryDB();
                            break;
                        case "Add Table":
                            AddTable();
                            break;
                        case "Drop Table":
                            DropTable();
                            break;
                        case "New Database":
                            CreateDB();
                            break;
                        case "Open Database":
                            OpenDB();
                            break;
                        case "Drop Database":
                            DropDatabase();
                            break;
                    }
                    break;

            }
            button.IsEnabled = true;
        }



        /// <summary>
        /// Add table
        /// Will also create databse
        /// </summary>
        public async void AddTable()
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection(textBox.Text);
            await connection.CreateTableAsync<Sensor>();
        }


        /// <summary>
        /// Drop the database
        /// </summary>
        public async void DropTable()
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection(textBox.Text);
            await connection.DropTableAsync<Sensor>();
        }

        /// <summary>
        /// Create blank databse
        /// </summary>
        public async void OpenDB()
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection(textBox.Text);
        }

        /// <summary>
        /// Delete the database
        /// </summary>
        public async void DropDatabase()
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection(textBox.Text);
            await connection.DropTableAsync<Sensor>();
        }

        /// <summary>
        /// Add one record to table
        /// </summary>
        public async void Insert(string sensor)
        {
            
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection(textBox.Text);
            var Sensor = new Sensor()
            {
                dateTime = DateTime.Now,
                Name = sensor,
                Value = GetRandomValue(sensor)
             };
            await connection.InsertAsync(Sensor);
        }

        /// <summary>
        /// Supposed to add 3 records to table
        /// Upon query only get first returned
        /// BUT id is incremented for 3 records added ???
        /// </summary>
        public async void InsertList()
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection(textBox.Text);

            var SensorList = new List<Sensor>()
            {
                new Sensor()
                {
                    dateTime = DateTime.Now,
                    Name="Temperature1",
                    Value = GetRandomValue("Temperature1")
        },
                new Sensor()
                {
                    dateTime = DateTime.Now,
                    Name="Temperature2",
                    Value = GetRandomValue("Temperature2")
        },
                new Sensor()
                {
                    dateTime = DateTime.Now,
                    Name="Humidity1",
                    Value = GetRandomValue("Humidity1")
        }
            };

            int n = await connection.InsertAllAsync(SensorList);
        }

        /// <summary>
        /// List records in Debug Output
        /// Second version doesn't work properly
        /// Note: Source had await included that is commented out here
        /// </summary>
        public async void QueryDB()
        {
            string qry = textBoxQueryBeginsWith.Text;
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection(textBox.Text);
            var SensorQry = /*await*/ connection.Table<Sensor>().Where(x => x.Name.StartsWith(qry));

            var SensorQryLst = await SensorQry.ToListAsync();

            System.Diagnostics.Debug.WriteLine("Number of {0} records found: {1}", qry, SensorQryLst.Count);
            
            foreach (var item in SensorQryLst)
            {
                int id = item.id;
                string name = item.Name;
                int value = item.Value;
                DateTime dateTime = item.dateTime;
                if (item.Name == null)
                    name = "Unknown";
                //Do your stuff
                System.Diagnostics.Debug.WriteLine("{0} {1} {2} {3}", id,dateTime, name, value); ;
            }

            //SQLiteAsyncConnection connection = new SQLiteAsyncConnection(textBox.Text);
            //var result = await connection.QueryAsync<Sensor>("Select Name FROM Sensors WHERE EnrolledCourse = ?", new object[] { "CSE 4203" });
            //foreach (var Item in result)
            //{
            //    Sensor item = (Sensor)Item;
            //    int id = item.id;
            //    string name = item.Name;
            //    string enrolledCourse = item.EnrolledCourse;
            //    if (item.Name == null)
            //        name = "Joe Doe";
            //    if (item.EnrolledCourse == null)
            //        enrolledCourse = "None";
                
            //    System.Diagnostics.Debug.WriteLine("{0} {1} {2}", id, name, enrolledCourse); ;
            //}
        }

        /// <summary>
        /// Returns random value 0 to 50 for sensors
        /// </summary>
        Random ran = new Random();
        int GetRandomValue(string sensor)
        {
            int val = ran.Next();
            val = val % 50;
            switch (sensor.Substring(0,4))
            {
                case "Temp":
                    val += 25;
                    break;
                case "Humi":
                    val += 50;
                    break;
                case "Pres":
                    val += 975;
                    break;
            }
            return val;

        }

        /// <summary>
        /// Update some records
        /// </summary>
        public async void Update(string sensor)
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection(textBox.Text);
            var SensorQry = await connection.Table<Sensor>().Where(x => x.Name.StartsWith(sensor)).FirstOrDefaultAsync();

            if (SensorQry != null)
            {
                SensorQry.dateTime = DateTime.Now;
                SensorQry.Value = GetRandomValue(SensorQry.Name);
                await connection.UpdateAsync(SensorQry);
            }
        }

        /// <summary>
        /// Delete a record
        /// </summary>
        public async void Delete(string sensor)
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection(textBox.Text);

            var SensorQry = await connection.Table<Sensor>().Where(x => x.Name.StartsWith(sensor)).FirstOrDefaultAsync();

            if (SensorQry != null)
            {
                await connection.DeleteAsync(SensorQry);
            }

        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            
        }
    }
}

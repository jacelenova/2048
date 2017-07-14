using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Data;

namespace _2048CSharp
{
    public class LocalDB
    {
        private SqlCeConnection _Connection = new SqlCeConnection();
        private string _ConnectionString;

        private DataTable _ScoreDT;

        public LocalDB()
        {
            //string DBPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            //DBPath = DBPath + "\\DB.sdf";
            //string DBPath = "C:\\Users\\jacel.enova\\Documents\\Visual Studio 2010\\Projects\\2048CSharp\\2048CSharp\\bin\\Debug\\DB.sdf";
            string DBPath = "C:\\Users\\jacel.enova\\Documents\\Visual Studio 2010\\Projects\\2048CSharp\\2048CSharp\\DB.sdf";

            _ConnectionString = "Data Source=" + DBPath + ";Persist Security Info=False;";
            Debug.WriteLine(_ConnectionString);
            _Connection.ConnectionString = _ConnectionString;
            
        }

        public void AddScore(string Name, int Score, int SquareCreated)
        {
            if (_Connection.State == System.Data.ConnectionState.Closed)
            {
                _Connection.Open();
            }

            SqlCeCommand cmd = new SqlCeCommand();
            cmd.Connection = _Connection;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "INSERT INTO tHighScore (Name, Score, SquareCreated) VALUES (@Name, @Score, @SquareCreated);";
            cmd.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@Score", System.Data.SqlDbType.Int, 4);
            cmd.Parameters.Add("@SquareCreated", System.Data.SqlDbType.Int, 4);

            cmd.Parameters["@Name"].Value = Name;
            cmd.Parameters["@Score"].Value = Score;
            cmd.Parameters["@SquareCreated"].Value = SquareCreated;

            cmd.ExecuteNonQuery();

            _Connection.Close();
        }
        
    }
}

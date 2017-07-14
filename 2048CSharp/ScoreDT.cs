using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;

namespace _2048CSharp
{
    public class ScoreDT : DataTable
    {
        private SqlCeConnection _Connection;
        private SqlCeDataAdapter _DataAdapter;
        private SqlCeCommandBuilder _Builder;

        public ScoreDT() 
        {
            _Connection = new SqlCeConnection();
            _Connection.ConnectionString = Global.ConnectionString;

            _DataAdapter = new SqlCeDataAdapter("SELECT TOP (20) * FROM tHighScore", _Connection);
            _Builder = new SqlCeCommandBuilder(_DataAdapter);

            _DataAdapter.Fill(this);
        }

        public void Update()
        {
            _DataAdapter.Update(this);
        }

        public DataRow getRow(int ID)
        {
            DataRow[] r = this.Select("ID = " + ID);
            if (r.Count() > 0)
            {
                return r[0];
            }
            else
            {
                return null;
            }
        }

        public void AddRow(string Name, int Score, int SquareCreated)
        {
            DataRow r = this.NewRow();
            r["Name"] = Name;
            r["Score"] = Score;
            r["SquareCreated"] = SquareCreated;

            this.Rows.Add(r);

            _DataAdapter.Update(this);
        }
    }
}

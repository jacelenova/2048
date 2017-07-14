using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2048CSharp
{
    public static class Global
    {
        private static string DBPath = "C:\\Users\\jacel.enova\\Documents\\Visual Studio 2010\\Projects\\2048CSharp\\2048CSharp\\DB.sdf";

        private static string _ConnectionString = "Data Source=" + DBPath + ";Persist Security Info=False;";
                
        public static string ConnectionString
        {
            get { return _ConnectionString; }
        }
        
    }
}

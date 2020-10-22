using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialTrackApp.DB
{
    public class MasterEntity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
    }
}

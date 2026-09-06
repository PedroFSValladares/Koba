using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySqlApi.Models
{
    public class Log
    {
        public int Id {get; set;}
        public DateTime Date {get; set;}
        public string LogLevel {get; set;}
        public string CategoryName {get; set;}
        public string Message {get; set;}
    }
}
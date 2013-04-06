using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fitbit.Models
{
    /*
    <heartLog>
        <heartRate>157</heartRate>
        <logId>1433</logId>
        <time>12:22</time>
        <tracker>Running</tracker>
    </heartLog>
    */

    public class HeartLog
    {
        public int HeartRate { get; set; }
        public string Tracker { get; set; }
        public string LogId { get; set; }
        public string Date { get; set; }
        public string Time { get; set; } //?
    }
}

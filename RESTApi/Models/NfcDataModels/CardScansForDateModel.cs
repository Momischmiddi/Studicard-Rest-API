using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NFCDataRESTApi.Models
{
    public class CardScansForDateModel
    {
        public DateTime RequestedDate { get; set; }

        public int ScanAmount { get; set; }
    }
}

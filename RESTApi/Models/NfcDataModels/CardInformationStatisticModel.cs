using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NFCDataRESTApi.Models
{
    public class CardInformationStatisticModel
    {
        public double TotalCardValue { get; set; }

        public double TotalLastTransaction { get; set; }

        public double AverageCardValue { get; set; }

        public double AverageLastTransaction { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTApi.Models.NfcDataModels
{
    public class CardInformationModelForID
    {
        public double CurrentValue { get; set; }

        public double LastTransaction { get; set; }

        public double LastTransactionAverage { get; set; }

        public double CurrentValueAverage { get; set; }

        public List<double> AllCurrentValues { get; set; }

        public List<double> AllLastTransactions { get; set; }

        public List<string> ScanDates { get; set; }

        public List<double> SpentMoney { get; set; }

        public List<double> AverageLastTransactions { get; set; }

        public List<int> ScanAmounts { get; set; }
    }
}

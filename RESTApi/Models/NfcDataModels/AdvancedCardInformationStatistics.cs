using System;

namespace RESTApi.Models.NfcDataModels
{
    public class AdvancedCardInformationStatistics
    {
        public string Date { get; set; }

        public double SpentMoney { get; set; }

        public double AverageLastTransaction { get; set; }

        public int ScanAmount { get; set; }
    }
}

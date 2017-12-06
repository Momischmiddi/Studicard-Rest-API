namespace RESTApi.Models.NfcDataModels
{
    public class CardInformationModelWithPassword
    {
        public double CardValue { get; set; }

        public double LastTransaction { get; set; }

        public string ID { get; set; }

        public string Password { get; set; }
    }
}

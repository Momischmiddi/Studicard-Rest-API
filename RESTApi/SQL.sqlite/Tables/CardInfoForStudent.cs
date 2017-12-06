using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NFCDataRESTApi.Models
{
    [Table("CardInfoForStudent")]
    public class CardInfoForStudent
    {
        public double CardValue { get; set; }

        public double LastTransaction { get; set; }

        public string ID { get; set; }

        [Key]
        public string ScanDate { get; set; }
    }
}

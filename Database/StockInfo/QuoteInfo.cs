using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ZERO.Database.StockInfo
{
    [Table("quote_info")]
    public class QuoteInfo
    {
        [Required]
        public string id { get; set; }

        public float open { get; set; }

        public float high { get; set; }

        public float low { get; set; }

        public float close { get; set; }

        public int volume { get; set; }

        public float millionAmount { get; set; }
        [Required]
        public string date { get; set; }
        public DateTime? createAt { get; set; }
    }
}

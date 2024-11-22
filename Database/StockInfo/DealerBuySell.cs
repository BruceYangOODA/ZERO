using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ZERO.Database.StockInfo
{
    [Table("dealer_buy_sell")]
    public class DealerBuySell
    {
        [Required]
        public string investrueId { get; set; }
        [Required]
        public string date { get; set; }
        public long buy { get; set; }
        public long sell { get; set; }
        public long? unixTimestamp { get; set; }
        public DateTime? createAt { get; set; }
    }
}

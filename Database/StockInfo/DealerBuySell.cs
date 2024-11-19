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
        public int buy { get; set; }
        public int sell { get; set; }
        public long? unixTimestamp { get; set; }
        public DateTime? createAt { get; set; }
    }
}

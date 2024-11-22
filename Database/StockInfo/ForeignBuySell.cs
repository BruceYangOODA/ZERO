using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZERO.Database.StockInfo
{
    [Table("foreign_buy_sell")]
    public class ForeignBuySell
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

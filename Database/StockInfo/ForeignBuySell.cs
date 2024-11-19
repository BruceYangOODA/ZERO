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
        public int buy { get; set; }
        public int sell { get; set; }
        public long? unixTimestamp { get; set; }
        public DateTime? createAt { get; set; }
    }
}

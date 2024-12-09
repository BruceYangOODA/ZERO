using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZERO.Database.StockInfo
{
    [Table("tw_trade_day")]
    public class TwTradeDay
    {
        [Required]
        public string date { get; set; }
        public long unixTimestamp { get; set; }
    }
}

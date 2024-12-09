using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ZERO.Database.StockInfo
{
    [Table("quote_info")]
    public class QuoteInfo
    {
        [Required]
        public string id { get; set; }
        [Required]
        public string date { get; set; }

        public float open { get; set; }

        public float high { get; set; }

        public float low { get; set; }

        public float close { get; set; }

        public long volume { get; set; } // 總成交量

        public long millionAmount { get; set; }
        public long? buyAmount { get; set; } 
        public long? sellAmount { get; set; }
        public long? sharesVolume { get; set; } // 當沖量
        public float? amplitude { get; set; } // 振幅
        public float? fluctuation { get; set; } // 漲幅 fluctuation
        public float? sharesRate { get; set; } // 當沖率
        public long? unixTimestamp { get; set; }
        public DateTime? createAt { get; set; }
    }
}

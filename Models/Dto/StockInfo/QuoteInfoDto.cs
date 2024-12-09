namespace ZERO.Models.Dto.StockInfo
{
    public class QuoteInfoDto
    {
        public QuoteInfoDto() { }

        public string? id { get; set; }

        public string? date { get; set; }
        public float? open { get; set; }

        public float? high { get; set; }

        public float? low { get; set; }

        public float? close { get; set; }
        public float? amplitude { get; set; } // 振幅
        public float? fluctuation { get; set; } // 漲幅 fluctuation

        public float? sharesRate { get; set; } // 當沖率

        public long? volume { get; set; }

        public float? millionAmount { get; set; }        
        public long? buyAmount { get; set; }
        public long? sellAmount { get; set; }
        public long? sharesVolume { get; set; } // 當沖量
        public long unixTimestamp { get; set; }
        public DateTime? createAt { get; set; }
        public long? foreignBuy { get; set; }
        public long? foreignSell { get; set; }
        public long? trustBuy { get; set; }
        public long? trustSell { get; set; }
        public long? dealerBuy { get; set; }
        public long? dealerSell { get; set; }

    }
}

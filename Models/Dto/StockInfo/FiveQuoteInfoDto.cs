namespace ZERO.Models.Dto.StockInfo
{
    public class FiveQuoteInfoDto
    {
        public FiveQuoteInfoDto() { }

        public string? id { get; set; }

        public float? open { get; set; }

        public float? high { get; set; }

        public float? low { get; set; }

        public float? close { get; set; }
        //public float? rate { get; set; }
        public float? fluctuation { get; set; } // 漲幅 fluctuation
        public float? amplitude { get; set; }

        public long? fiveDayVolume { get; set; }
        public bool isBurstVolume { get; set; } // 是否爆量 一日大於1.5  5日平均
        public bool isDayTrading { get; set; } // 是否當沖大於40%
        public bool isAmplify { get; set; } // 是否振幅大於 5%

        public List<QuoteInfoDto>? fiveDayList { get; set; }

    }
}

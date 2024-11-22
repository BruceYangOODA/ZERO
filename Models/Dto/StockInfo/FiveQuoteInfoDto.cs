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

        public long? fiveDayVolume { get; set; }
        public bool isBurstVolume { get; set; } // 是否爆量

        public List<QuoteInfoDto>? fiveDayList { get; set; }

    }
}

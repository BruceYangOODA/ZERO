﻿namespace ZERO.Models.Dto.StockInfo
{
    public class TrustBuySellDto
    {
        public TrustBuySellDto() { }

        public string investrueId { get; set; }
        public string date { get; set; }
        public long buy { get; set; }
        public long sell { get; set; }
        public long? unixTimestamp { get; set; }
        public DateTime? createAt { get; set; }


    }
}

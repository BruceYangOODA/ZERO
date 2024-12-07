﻿using ZERO.Database.StockInfo;

namespace ZERO.Models.Dto.StockInfo
{
    public class TrustBuySellDto
    {
        public string investrueId { get; set; }
        public string date { get; set; }
        public long buy { get; set; }
        public long sell { get; set; }
        public long? unixTimestamp { get; set; }
        public DateTime? createAt { get; set; }


        public TrustBuySellDto() { }
        public TrustBuySellDto(TrustBuySell t)
        {
            investrueId = t.investrueId;
            date = t.date;
            buy = t.buy;
            sell = t.sell;
            unixTimestamp = t.unixTimestamp;
        }
    }
}

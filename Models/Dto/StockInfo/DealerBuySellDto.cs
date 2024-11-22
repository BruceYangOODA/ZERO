using Microsoft.IdentityModel.Tokens;
using ZERO.Database.StockInfo;

namespace ZERO.Models.Dto.StockInfo
{
    public class DealerBuySellDto
    {
        public string investrueId { get; set; }
        public string date { get; set; }
        public long buy { get; set; }
        public long sell { get; set; }
        public long? unixTimestamp { get; set; }
        public DateTime? createAt { get; set; }

        public DealerBuySellDto() { }
        public DealerBuySellDto(DealerBuySell t)
        {
            investrueId = t.investrueId;
            date = t.date;
            buy = t.buy;
            sell = t.sell;            
            unixTimestamp = t.unixTimestamp;
        }
    }
}

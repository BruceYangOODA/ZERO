using System.ComponentModel.DataAnnotations;
using ZERO.Database.StockInfo;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ZERO.Models.Dto.StockInfo
{
    public class ForeignBuySellDto
    {
        public string investrueId { get; set; }        
        public string date { get; set; }
        public int buy { get; set; }
        public int sell { get; set; }
        public long? unixTimestamp { get; set; }
        public DateTime? createAt { get; set; }

        public ForeignBuySellDto() { }
        public ForeignBuySellDto(ForeignBuySell t) 
        {
            investrueId = t.investrueId;
            date = t.date;
            buy = t.buy;
            sell = t.sell;
            unixTimestamp = t.unixTimestamp;
        }

    }
}

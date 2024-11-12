using System.ComponentModel.DataAnnotations;
using ZERO.Database.StockInfo;

namespace ZERO.Models.Dto.StockInfo
{
    public class QuoteInfoDto
    {
        
        public string id { get; set; }

        public float open { get; set; }

        public float high { get; set; }

        public float low { get; set; }

        public float close { get; set; }

        public int volume { get; set; }

        public float millionAmount { get; set; }        
        public string date { get; set; }
        public DateTime? createAt { get; set; }
        public QuoteInfoDto(QuoteInfo t) 
        {
            id = t.id;
            open = t.open;
            high = t.high;
            low = t.low;
            close = t.close;
            volume = t.volume;
            millionAmount = t.millionAmount;
            date = t.date;
            createAt = t.createAt;
        }
        public QuoteInfoDto() { }


    }
}

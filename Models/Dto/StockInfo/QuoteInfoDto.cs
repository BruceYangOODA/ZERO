using System.ComponentModel.DataAnnotations;
using ZERO.Database.StockInfo;

namespace ZERO.Models.Dto.StockInfo
{
    public class QuoteInfoDto: QuoteInfo
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
        public QuoteInfoDto(QuoteInfo q) 
        {
            id = q.id;
            open = q.open;
            high = q.high;
            low = q.low;
            close = q.close;
            volume = q.volume;
            millionAmount = q.millionAmount;
            date = q.date;
            createAt = q.createAt;
        }


    }
}

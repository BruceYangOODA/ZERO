namespace ZERO.Models.Dto.StockInfo
{
    public class VolumeDataDto
    {
        public string stockNo { get; set; }

        public string? theDate { get; set; }
        //public DateTime date { get; set; }
        
        public long buyAmount { get; set; }
        public long sellAmount { get; set; }
        public long sharesVolume { get; set; } // 當沖量
    }
}

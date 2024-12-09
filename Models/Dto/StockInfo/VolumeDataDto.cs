namespace ZERO.Models.Dto.StockInfo
{
    public class VolumeDataDto
    {
        public VolumeDataDto() { }
        public string stockNo { get; set; }

        public string? theDate { get; set; }
        //public DateTime date { get; set; }
        
        public long buyAmount { get; set; }
        public long sellAmount { get; set; }
        public long sharesVolume { get; set; } // 當沖量
        public float? amplitude { get; set; } // 振幅
        public float? fluctuation { get; set; } // 漲幅 fluctuation
        public float? sharesRate { get; set; } // 當沖率
    }
}

namespace ZERO.Models.Dto.StockInfo
{
    public class VolumeDataDto
    {
        public string stockNo { get; set; }

        public string? theDate { get; set; }
        //public DateTime date { get; set; }
        
        public float buyAmount { get; set; }
        public float sellAmount { get; set; }
        public float sharesVolume { get; set; } // 當沖量
    }
}

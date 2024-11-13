namespace ZERO.Models.JWT
{
    /// <summary> 存放Token跟過期時間的類 </summary>
    public class TnToken
    {
        /// <summary> token </summary>
        public string? tokenStr { get; set; }

        /// <summary> 過期時間 </summary>
        public DateTime expires { get; set; }
    }
}

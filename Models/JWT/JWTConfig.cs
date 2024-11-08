namespace ZERO.Models.JWT
{
    /// <summary> 配置token生成資訊 </summary>
    public class JWTConfig
    {
        /// <summary> Token釋出者 </summary>
        public string? Issuer { get; set; }

        /// <summary> Token接受者 </summary>
        public string? Audience { get; set; }

        /// <summary> 密鑰 </summary>
        public string? IssuerSigningKey { get; set; }

        /// <summary> 過期時間 </summary>
        public int AccessTokenExpiresMinutes { get; set; }
        /// <summary> token 過期後，可重刷時效 </summary>
        public int AccessRefershTokenExpiresMinutes { get; set; }
    }
}

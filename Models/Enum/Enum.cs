namespace ZERO.Models.Enum
{


    public enum RequestResultCode
    {
        Success,
        NotFound,
        /// <summary> BadRequest </summary>
        Failed,
        /// <summary> 新增成功</summary>
        Created,
        /// <summary> 刪除成功 </summary>
        NoContent,
        /// <summary> DB衝突  </summary>
        Conflict,
        /// <summary> Exception </summary>
        InternalServerError
    }

}

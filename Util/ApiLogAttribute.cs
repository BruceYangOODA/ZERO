using System;
/// <summary>
/// 註記是否要記錄 API Loc 到 DB 中
/// </summary>
public class ApiLogAttribute : Attribute
{
    public ApiLogEvent Event { get; set; }

    public ApiLogAttribute(ApiLogEvent ev = ApiLogEvent.None)
    {
        Event = ev;
    }
}
/// <summary>
/// 註記特殊事件定義
/// </summary>
public enum ApiLogEvent
{
    None,
    Transaction
}


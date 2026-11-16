namespace QuotePipeline.Core;

/// <summary>
/// 一筆行情。用 readonly struct 避免每筆都在 heap 配置物件(之後 GC 實驗會用到)。
/// Timestamp 是 Stopwatch.GetTimestamp(),之後量測端到端延遲用。
/// </summary>
public readonly record struct Quote(long Seq, long Timestamp, int SymbolId, int Price, int Qty);

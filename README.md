# QuotePipeline — 高併發練習專案

目標:用「先寫會壞的版本 → 量測 → 修好 → 再量測」的方式,練習 C# 高併發與效能分析。
資料來源做成介面(`IQuoteSource`),模擬器、重播器、元大 Live 可以互換。

## Mac 環境設定

```bash
brew install --cask dotnet-sdk@10     # 安裝 .NET SDK(需 10.0)
dotnet --version
dotnet tool install --global dotnet-counters
dotnet tool install --global dotnet-trace
```

VS Code 安裝擴充:**C# Dev Kit**。用 VS Code 開啟這個資料夾即可。

## 執行(一定要 Release,Debug 模式的效能數據沒有意義)

```bash
cd src/QuotePipeline
dotnet build -c Release
dotnet run -c Release -- sim --rate 200000 --seconds 5   # Day 1
dotnet run -c Release -- race                            # Day 2
```

## 第一週進度

| 天 | 任務 | 狀態 |
|---|---|---|
| 1 | 跑 `sim`,調高 `--rate`(50萬、100萬、200萬),看吞吐量上限在哪、最大延遲怎麼變 | ☐ |
| 2 | 跑 `race`,回答程式裡的三個思考題;把 Threads 改成 1/2/8 比較 | ☐ |
| 3 | 新增 `ChannelSink`:接收端寫入**無上限**佇列,消費端故意加 `Thread.Sleep` 變慢,觀察記憶體 → 改成**有界** Channel + 背壓 | ☐ |
| 4 | 在消費端故意用 `.Result` 阻塞非同步呼叫,用 `dotnet-counters monitor` 看 thread pool 的 queue length → 改正 | ☐ |
| 5 | 新增 `SocketSource`/`SocketSink`:用真的 TCP 傳 Quote,自己處理訊息邊界(長度前綴),處理半包黏包 | ☐ |
| 6-7 | 用 `dotnet-counters` 看 GC 次數,減少配置;用 BenchmarkDotNet 做微基準測試 | ☐ |

## 量測紀錄(每次改動都填,這就是面試時的證據)

| 日期 | 改動 | 吞吐量 | p99 延遲 | GC Gen0/1/2 | 備註 |
|---|---|---|---|---|---|
| | | | | | |

## 常用量測指令

```bash
# 即時看 GC、thread pool、鎖競爭
dotnet-counters monitor --name QuotePipeline --counters System.Runtime
# 取 trace,用 PerfView / speedscope 分析
dotnet-trace collect --name QuotePipeline
```

## 結構

```
src/QuotePipeline/
  Core/         Quote、IQuoteSource/IQuoteSink、CountingSink
  Sources/      SimulatorSource(之後加 ReplaySource、YuantaLiveSource[Windows])
  Experiments/  RaceExperiment(Day 2)
  Program.cs    指令分派
```

## 之後的方向

- Web 儀表板:ASP.NET Core + SignalR,畫面以 10–30 fps 取樣指標快照(UI 不放在熱路徑)
- ReplaySource:讀取 Quotes.db 以 1×/10×/100× 重播
- 容錯:斷線重連、序號補漏、重啟後還原
- 在 Windows 上同樣跑一次,比較結果

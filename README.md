# SpecPaste

A one-page PC summary you can paste into a forum — a lightweight replacement for Speccy's system summary view, without the installer, tray service or telemetry.

## Features

- Collects via WMI: OS (caption, version, build + UBR, architecture), CPU (name, cores, logical processors, max clock), memory (total, per-module capacity/speed/manufacturer/part number, slots used/total), motherboard (manufacturer, product, BIOS vendor/version/date), GPU (name, driver version, adapter RAM, resolution, refresh rate), disks (model, size, SSD/HDD, bus type) + volumes (letter, filesystem, size, free), and up network adapters (name, link speed).
- Output as plain text, Markdown or BBCode (for forums like HKEPC).
- Single-window GUI with 重新整理 / 複製純文字 / 複製 Markdown / 複製 BBCode / 儲存為 .txt.
- CLI: `SpecPaste.exe --text|--markdown|--bbcode` prints to stdout and exits. In PowerShell pipe it (`SpecPaste.exe --markdown | Out-File specs.md`); `>` works in cmd.

## Install

Unzip the release and run `SpecPaste.exe`. Requires the .NET 10 Desktop Runtime.

## Build

```
./build.ps1
```

Produces `dist/SpecPaste-v1.0.0-win-x64.zip`.

## Privacy

SpecPaste reads only the hardware/OS fields listed above. It never collects serial numbers, MAC/IP addresses, your user name, computer name or product keys, and it sends nothing over the network.

## Licence

MIT — see LICENSE.

---

# SpecPaste

一頁式電腦資訊摘要，可貼到討論區使用 — Speccy 系統摘要畫面的輕量替代品，沒有安裝程式、常駐服務或遙測。

## 功能

- 透過 WMI 收集：作業系統（名稱、版本、組建＋UBR、架構）、處理器（名稱、核心、邏輯處理器、最大時脈）、記憶體（總容量、每條模組容量／時脈／廠商／料號、已用／總插槽）、主機板（製造商、型號、BIOS 廠商／版本／日期）、顯示卡（名稱、驅動程式版本、記憶體、解析度、更新頻率）、磁碟（型號、容量、SSD/HDD、匯流排類型）＋磁碟區（代號、檔案系統、容量、可用）、以及已連線的網絡介面卡（名稱、連線速度）。
- 輸出純文字、Markdown 或 BBCode（適用 HKEPC 等討論區）。
- 單視窗圖形介面，含「重新整理／複製純文字／複製 Markdown／複製 BBCode／儲存為 .txt」。
- 命令列：`SpecPaste.exe --text|--markdown|--bbcode` 輸出到 stdout 後結束。PowerShell 請用管線（`SpecPaste.exe --markdown | Out-File specs.md`）；cmd 可直接用 `>`。

## 安裝

解壓縮後執行 `SpecPaste.exe`。需要 .NET 10 Desktop Runtime。

## 建置

```
./build.ps1
```

產出 `dist/SpecPaste-v1.0.0-win-x64.zip`。

## 私隱

SpecPaste 只讀取上述硬體／系統欄位。它絕不收集序號、MAC/IP 位址、使用者名稱、電腦名稱或產品金鑰，也不會透過網絡傳送任何資料。

## 授權

MIT — 見 LICENSE。

---

## Sponsor / 贊助

If this tool helps you, you can buy me a coffee. 如果這個工具對你有幫助，歡迎請我喝杯咖啡。

- Buy Me a Coffee: https://buymeacoffee.com/kinfxhk

# HANDOFF — SpecPaste v1.0.0

## What was built

.NET 10 WPF app (`src/SpecPaste`, net10.0-windows) + xUnit tests (`tests/SpecPaste.Tests`).

- `SystemInfo` record model + per-section records (SystemInfo.cs).
- `Collector` (WMI via System.Management): OS (caption/version/build+UBR from registry/arch), CPU, memory (total + per-module + slots), motherboard + BIOS, GPUs (with registry `HardwareInformation.qwMemorySize` override when AdapterRAM ≤ 4 GB), disks (MSFT_PhysicalDisk) + volumes, up network adapters. Each section is wrapped in its own try/catch — a failed section yields `null`/empty and the formatter prints 無法讀取, never crashes.
- `Formatter` (pure): Text / Markdown / BBCode, sectioned (系統、處理器、記憶體、主機板、顯示卡、儲存裝置、網絡).
- GUI: one window, read-only TextBox, buttons 重新整理 / 複製純文字 / 複製 Markdown / 複製 BBCode / 儲存為 .txt. Default light theme (dark mode skipped — not trivial, and spec allows default light).
- CLI: `SpecPaste.exe --text|--markdown|--bbcode` writes UTF-8 to stdout via `AttachConsole(-1)`/`GetStdHandle` and exits.
- Repo scaffolding: README (EN + 繁中 + sponsor block), LICENSE, THIRD-PARTY-NOTICES, FUNDING.yml, build.yml (setup-dotnet 10.0.x), build.ps1, CHANGELOG, .gitignore.

## Commands run (real output)

```
pwsh -File build.ps1
  Build:   0 warnings, 0 errors
  Test:    Passed! Failed: 0, Passed: 6, Skipped: 0, Total: 6
  Publish: dist/publish/SpecPaste.exe (+ SpecPaste.pdb)
  Zip:     dist/SpecPaste-v1.0.0-win-x64.zip  (221,845 bytes)

./dist/publish/SpecPaste.exe --markdown > evidence/this-pc.md   (exit 0)
./dist/publish/SpecPaste.exe            (GUI launched, screenshot via PowerShell CopyFromScreen)
```

Evidence: `evidence/this-pc.md` (Markdown output for this PC) and `evidence/gui.png` (1140×1080, 110 KB, full window). Verified this-pc.md contains no serial / MAC / IP / computer name / user name (the only dotted-quad matches are GPU driver version strings, which is a required field).

## Verified vs not verified

✅ `build.ps1` exits 0 locally; 6/6 tests pass; zip exists in dist/.
✅ Published exe runs `--markdown` on this PC and the output is privacy-clean.
✅ GUI launches and renders all sections + 5 buttons (screenshot).
⬜ GitHub Actions CI — not run (no push; `gh` and pushes are forbidden by spec).
⬜ GUI copy-to-clipboard buttons and Save-as-.txt dialog — code is standard `Clipboard.SetText` / `SaveFileDialog`, but not clicked during the real check.
⬜ BBCode/Markdown rendering on an actual forum (HKEPC) — format strings look valid but not pasted anywhere.

## Decisions / limits

- Single-file publish `-p:PublishSingleFile=true` WORKED on .NET 10 (one SpecPaste.exe, framework-dependent). Zip also contains the .pdb.
- Dark theme: skipped (default light) per spec ("only if trivial").
- Requires .NET 10 Desktop Runtime on the target machine (framework-dependent).
- Integrated GPU with no attached display reports 解析度 0 × 0 @ 0 Hz — correct WMI result, not a bug.
- CLI stdout is UTF-8; on a legacy cmd with a non-UTF8 codepage the Chinese text may render garbled (file redirect is always UTF-8).


## Reviewer verification (it-dog)

- KinTest VM (release zip only): GUI showed every section populated (VMware hardware); `--markdown` to a file works from cmd and via `| Out-File` in PowerShell (PowerShell `>` from a GUI-subsystem exe gives an empty file; README says so). Memory speed 0 MHz is now hidden. Evidence: `evidence/vm/`.

using System.Text;

namespace SpecPaste;

public static class Formatter
{
    public static string Text(SystemInfo i) => Render(i, "text");
    public static string Markdown(SystemInfo i) => Render(i, "markdown");
    public static string BbCode(SystemInfo i) => Render(i, "bbcode");

    private static string Render(SystemInfo i, string fmt)
    {
        var sb = new StringBuilder();
        foreach (var l in Build(i))
        {
            if (l.IsHeader)
                sb.AppendLine(fmt switch { "text" => $"=== {l.Label} ===", "markdown" => $"## {l.Label}", _ => $"[b]{l.Label}[/b]" });
            else if (l.Value is null)
                sb.AppendLine(l.Label);
            else
                sb.AppendLine(fmt switch
                {
                    "text" => $"{l.Label}: {l.Value}",
                    "markdown" => $"- **{l.Label}**: {l.Value}",
                    _ => $"[b]{l.Label}[/b]: {l.Value}",
                });
        }
        return sb.ToString().TrimEnd();
    }

    private static List<Line> Build(SystemInfo i)
    {
        var lines = new List<Line>();
        void Header(string t) => lines.Add(new Line(true, t, null));
        void Row(string l, string v) => lines.Add(new Line(false, l, v));
        void Plain(string t) => lines.Add(new Line(false, t, null));
        void Unreadable() => Plain("無法讀取");

        Header("系統");
        if (i.Os is { } os)
        {
            Row("作業系統", os.Caption);
            Row("版本", os.Version);
            Row("組建", os.Build);
            Row("架構", os.Architecture);
        }
        else Unreadable();

        Header("處理器");
        if (i.Cpu is { } cpu)
        {
            Row("名稱", cpu.Name);
            Row("核心", cpu.Cores.ToString());
            Row("邏輯處理器", cpu.LogicalProcessors.ToString());
            Row("最大時脈", $"{cpu.MaxClockMhz} MHz");
        }
        else Unreadable();

        Header("記憶體");
        if (i.Memory is { } mem)
        {
            Row("總容量", FormatBytes(mem.TotalBytes));
            Row("插槽", $"{mem.SlotsUsed} / {mem.SlotsTotal}");
            for (var n = 0; n < mem.Modules.Count; n++)
            {
                var m = mem.Modules[n];
                var parts = new[] { FormatBytes(m.CapacityBytes), $"{m.SpeedMhz} MHz", m.Manufacturer, m.PartNumber }
                    .Where(p => p.Length > 0);
                Row($"模組 {n + 1}", string.Join(", ", parts));
            }
        }
        else Unreadable();

        Header("主機板");
        if (i.Motherboard is { } mb)
        {
            Row("製造商", mb.Manufacturer);
            Row("型號", mb.Product);
            Row("BIOS 廠商", mb.BiosVendor);
            Row("BIOS 版本", mb.BiosVersion);
            Row("BIOS 日期", mb.BiosDate);
        }
        else Unreadable();

        Header("顯示卡");
        if (Empty(i.Gpus)) Unreadable();
        else foreach (var g in i.Gpus!)
        {
            Row("名稱", g.Name);
            Row("驅動程式", g.DriverVersion);
            Row("記憶體", FormatBytes(g.MemoryBytes));
            if (g.ResolutionWidth > 0) Row("解析度", $"{g.ResolutionWidth} × {g.ResolutionHeight} @ {g.RefreshRateHz} Hz");
            Plain("");
        }

        Header("儲存裝置");
        Plain("磁碟");
        if (Empty(i.Disks)) Unreadable();
        else foreach (var d in i.Disks!) Row(d.Model, $"{FormatBytes(d.SizeBytes)} · {d.MediaType} · {d.BusType}");
        Plain("磁碟區");
        if (Empty(i.Volumes)) Unreadable();
        else foreach (var v in i.Volumes!) Row($"{v.Letter} ({v.FileSystem})", $"{FormatBytes(v.SizeBytes)}（可用 {FormatBytes(v.FreeBytes)}）");

        Header("網絡");
        if (Empty(i.NetworkAdapters)) Unreadable();
        else foreach (var n in i.NetworkAdapters!) Row(n.Name, n.LinkSpeedBps is null or 0 ? "未知" : FormatSpeed(n.LinkSpeedBps.Value));

        return lines;
    }

    private static bool Empty<T>(IReadOnlyList<T>? l) => l is null || l.Count == 0;

    private static string FormatBytes(ulong bytes)
    {
        string[] units = { "B", "KB", "MB", "GB", "TB", "PB" };
        double v = bytes;
        var u = 0;
        while (v >= 1024 && u < units.Length - 1) { v /= 1024; u++; }
        return u == 0 ? $"{bytes} B" : $"{v:0.##} {units[u]}";
    }

    private static string FormatSpeed(ulong bps) =>
        bps >= 1_000_000_000 ? $"{bps / 1_000_000_000.0:0.#} Gbps" :
        bps >= 1_000_000 ? $"{bps / 1_000_000.0:0.#} Mbps" :
        $"{bps} bps";

    private readonly record struct Line(bool IsHeader, string Label, string? Value);
}

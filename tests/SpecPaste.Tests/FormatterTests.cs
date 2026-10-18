using Xunit;

namespace SpecPaste.Tests;

public class FormatterTests
{
    private static readonly SystemInfo Sample = new(
        new OsInfo("Microsoft Windows 11 Pro", "10.0.26100", "26100.2454", "64 位元"),
        new CpuInfo("AMD Ryzen 7 5800X 8-Core Processor", 8, 16, 3800),
        new MemoryInfo(34359738368UL, new List<MemoryModuleInfo>
        {
            new(17179869184UL, 3200, "Kingston", "KF3200C16D4/16GX"),
            new(17179869184UL, 3200, "Kingston", "KF3200C16D4/16GX"),
        }, 2, 4),
        new MotherboardInfo("ASUS", "ROG STRIX B550-F GAMING", "American Megatrends", "3604", "2024-03-15"),
        new List<GpuInfo>
        {
            new("NVIDIA GeForce RTX 4070", "32.0.15.6603", 12884901888UL, 2560, 1440, 165),
        },
        new List<DiskInfo>
        {
            new("Samsung SSD 980 PRO 1TB", 1000204886016UL, "SSD", "NVMe"),
        },
        new List<VolumeInfo>
        {
            new("C:", "NTFS", 1000204886016UL, 500107862016UL),
        },
        new List<NetworkAdapterInfo>
        {
            new("乙太網路", 1000000000UL),
        });

    [Fact]
    public void Text_ContainsAllSevenSections()
    {
        var t = Formatter.Text(Sample);
        Assert.Contains("系統", t);
        Assert.Contains("處理器", t);
        Assert.Contains("記憶體", t);
        Assert.Contains("主機板", t);
        Assert.Contains("顯示卡", t);
        Assert.Contains("儲存裝置", t);
        Assert.Contains("網絡", t);
    }

    [Fact]
    public void Text_FormatsValues()
    {
        var t = Formatter.Text(Sample);
        Assert.Contains("總容量: 32 GB", t);
        Assert.Contains("NVMe", t);
        Assert.Contains("1 Gbps", t);
        Assert.Contains("64 位元", t);
    }

    [Fact]
    public void Markdown_UsesMarkdownSyntax()
    {
        var m = Formatter.Markdown(Sample);
        Assert.Contains("## 系統", m);
        Assert.Contains("- **作業系統**: Microsoft Windows 11 Pro", m);
    }

    [Fact]
    public void BbCode_UsesBbCodeSyntax()
    {
        var b = Formatter.BbCode(Sample);
        Assert.Contains("[b]系統[/b]", b);
        Assert.Contains("[b]作業系統[/b]: Microsoft Windows 11 Pro", b);
    }

    [Fact]
    public void Privacy_NoSerialMacOrIp()
    {
        foreach (var output in new[] { Formatter.Text(Sample), Formatter.Markdown(Sample), Formatter.BbCode(Sample) })
        {
            var lower = output.ToLowerInvariant();
            Assert.DoesNotContain("serial", lower);
            Assert.DoesNotContain("mac", lower);
            Assert.DoesNotContain("ip address", lower);
        }
    }

    [Fact]
    public void NullSections_ShowUnreadable()
    {
        var empty = new SystemInfo(null, null, null, null, null, null, null, null);
        Assert.Contains("無法讀取", Formatter.Text(empty));
        Assert.Contains("無法讀取", Formatter.Markdown(empty));
        Assert.Contains("無法讀取", Formatter.BbCode(empty));
    }
}

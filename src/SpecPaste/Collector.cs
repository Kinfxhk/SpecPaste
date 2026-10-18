using System.Management;
using Microsoft.Win32;

namespace SpecPaste;

public sealed class Collector
{
    public SystemInfo Collect() => new(
        Os(), Cpu(), Memory(), Motherboard(), Gpus(), Disks(), Volumes(), NetworkAdapters());

    private OsInfo? Os()
    {
        try
        {
            var o = Query("SELECT Caption, Version, BuildNumber, OSArchitecture FROM Win32_OperatingSystem").FirstOrDefault();
            if (o is null) return null;
            var build = S(o, "BuildNumber");
            var ubr = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "UBR", null);
            if (ubr is not null) build += "." + ubr;
            return new OsInfo(S(o, "Caption"), S(o, "Version"), build, S(o, "OSArchitecture"));
        }
        catch { return null; }
    }

    private CpuInfo? Cpu()
    {
        try
        {
            var o = Query("SELECT Name, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeed FROM Win32_Processor").FirstOrDefault();
            if (o is null) return null;
            return new CpuInfo(S(o, "Name").Trim(), I(o, "NumberOfCores"), I(o, "NumberOfLogicalProcessors"), I(o, "MaxClockSpeed"));
        }
        catch { return null; }
    }

    private MemoryInfo? Memory()
    {
        try
        {
            var total = U(Query("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem").FirstOrDefault(), "TotalPhysicalMemory");
            var modules = new List<MemoryModuleInfo>();
            foreach (var o in Query("SELECT Capacity, Speed, Manufacturer, PartNumber FROM Win32_PhysicalMemory"))
                modules.Add(new MemoryModuleInfo(U(o, "Capacity"), I(o, "Speed"), S(o, "Manufacturer").Trim(), S(o, "PartNumber").Trim()));

            var slots = 0;
            foreach (var o in Query("SELECT MemoryDevices FROM Win32_PhysicalMemoryArray"))
                slots += I(o, "MemoryDevices");

            if (total == 0 && modules.Count == 0) return null;
            return new MemoryInfo(total, modules, modules.Count, slots);
        }
        catch { return null; }
    }

    private MotherboardInfo? Motherboard()
    {
        try
        {
            var bb = Query("SELECT Manufacturer, Product FROM Win32_BaseBoard").FirstOrDefault();
            var bios = Query("SELECT Manufacturer, SMBIOSBIOSVersion, ReleaseDate FROM Win32_BIOS").FirstOrDefault();
            return new MotherboardInfo(
                S(bb, "Manufacturer"), S(bb, "Product"),
                S(bios, "Manufacturer"), S(bios, "SMBIOSBIOSVersion"), ToDate(S(bios, "ReleaseDate")));
        }
        catch { return null; }
    }

    private IReadOnlyList<GpuInfo>? Gpus()
    {
        try
        {
            var memByDesc = GpuMemoryByDesc();
            var list = new List<GpuInfo>();
            foreach (var o in Query("SELECT Name, DriverVersion, AdapterRAM, CurrentHorizontalResolution, CurrentVerticalResolution, CurrentRefreshRate FROM Win32_VideoController"))
            {
                var name = S(o, "Name");
                var ram = U(o, "AdapterRAM");
                if (ram <= 4294967296UL && memByDesc.TryGetValue(name, out var reg) && reg > ram)
                    ram = reg;
                list.Add(new GpuInfo(name, S(o, "DriverVersion"), ram,
                    I(o, "CurrentHorizontalResolution"), I(o, "CurrentVerticalResolution"), I(o, "CurrentRefreshRate")));
            }
            return list;
        }
        catch { return null; }
    }

    private IReadOnlyList<DiskInfo>? Disks()
    {
        try
        {
            var list = new List<DiskInfo>();
            foreach (var o in Query("SELECT Model, Size, MediaType, BusType FROM MSFT_PhysicalDisk", @"root\Microsoft\Windows\Storage"))
                list.Add(new DiskInfo(S(o, "Model"), U(o, "Size"), MediaType(I(o, "MediaType")), BusType(I(o, "BusType"))));
            return list;
        }
        catch { return null; }
    }

    private IReadOnlyList<VolumeInfo>? Volumes()
    {
        try
        {
            var list = new List<VolumeInfo>();
            foreach (var o in Query("SELECT DeviceID, FileSystem, Size, FreeSpace FROM Win32_LogicalDisk WHERE DriveType = 3"))
                list.Add(new VolumeInfo(S(o, "DeviceID"), S(o, "FileSystem"), U(o, "Size"), U(o, "FreeSpace")));
            return list;
        }
        catch { return null; }
    }

    private IReadOnlyList<NetworkAdapterInfo>? NetworkAdapters()
    {
        try
        {
            var list = new List<NetworkAdapterInfo>();
            foreach (var o in Query("SELECT Name, NetConnectionID, Speed, NetConnectionStatus FROM Win32_NetworkAdapter WHERE NetConnectionStatus = 2"))
            {
                var name = S(o, "NetConnectionID");
                if (name.Length == 0) name = S(o, "Name");
                var speed = U(o, "Speed");
                list.Add(new NetworkAdapterInfo(name, speed == 0 ? null : speed));
            }
            return list;
        }
        catch { return null; }
    }

    private static Dictionary<string, ulong> GpuMemoryByDesc()
    {
        var map = new Dictionary<string, ulong>(StringComparer.OrdinalIgnoreCase);
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}");
            if (key is null) return map;
            foreach (var sub in key.GetSubKeyNames())
            {
                using var k = key.OpenSubKey(sub);
                var desc = k?.GetValue("DriverDesc")?.ToString();
                var mem = k?.GetValue("HardwareInformation.qwMemorySize");
                if (!string.IsNullOrEmpty(desc) && mem is not null)
                    map[desc] = Convert.ToUInt64(mem);
            }
        }
        catch { }
        return map;
    }

    private static List<Dictionary<string, object?>> Query(string query, string? scope = null)
    {
        var rows = new List<Dictionary<string, object?>>();
        using var searcher = new ManagementObjectSearcher(scope, query);
        foreach (ManagementObject o in searcher.Get())
        {
            var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            foreach (PropertyData p in o.Properties)
                row[p.Name] = p.Value;
            rows.Add(row);
        }
        return rows;
    }

    private static string S(Dictionary<string, object?>? o, string key) =>
        o is not null && o.TryGetValue(key, out var v) && v is not null ? v.ToString() ?? "" : "";

    private static int I(Dictionary<string, object?>? o, string key) =>
        o is not null && o.TryGetValue(key, out var v) && v is not null ? Convert.ToInt32(v) : 0;

    private static ulong U(Dictionary<string, object?>? o, string key) =>
        o is not null && o.TryGetValue(key, out var v) && v is not null ? Convert.ToUInt64(v) : 0UL;

    private static string ToDate(string s) => s.Length >= 8 ? $"{s[..4]}-{s[4..6]}-{s[6..8]}" : s;

    private static string MediaType(int v) => v switch { 3 => "HDD", 4 => "SSD", _ => "未知" };

    private static string BusType(int v) => v switch
    {
        1 => "SCSI", 2 => "ATAPI", 3 => "ATA", 4 => "IEEE 1394", 5 => "SSA", 6 => "Fibre Channel",
        7 => "USB", 8 => "RAID", 9 => "iSCSI", 10 => "SAS", 11 => "SATA", 12 => "SD", 13 => "MMC",
        14 => "Virtual", 15 => "File Backed Virtual", 16 => "Storage Spaces", 17 => "NVMe", _ => "未知",
    };
}

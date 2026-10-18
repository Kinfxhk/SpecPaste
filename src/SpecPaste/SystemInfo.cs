namespace SpecPaste;

public sealed record SystemInfo(
    OsInfo? Os,
    CpuInfo? Cpu,
    MemoryInfo? Memory,
    MotherboardInfo? Motherboard,
    IReadOnlyList<GpuInfo>? Gpus,
    IReadOnlyList<DiskInfo>? Disks,
    IReadOnlyList<VolumeInfo>? Volumes,
    IReadOnlyList<NetworkAdapterInfo>? NetworkAdapters);

public sealed record OsInfo(string Caption, string Version, string Build, string Architecture);
public sealed record CpuInfo(string Name, int Cores, int LogicalProcessors, int MaxClockMhz);
public sealed record MemoryInfo(ulong TotalBytes, IReadOnlyList<MemoryModuleInfo> Modules, int SlotsUsed, int SlotsTotal);
public sealed record MemoryModuleInfo(ulong CapacityBytes, int SpeedMhz, string Manufacturer, string PartNumber);
public sealed record MotherboardInfo(string Manufacturer, string Product, string BiosVendor, string BiosVersion, string BiosDate);
public sealed record GpuInfo(string Name, string DriverVersion, ulong MemoryBytes, int ResolutionWidth, int ResolutionHeight, int RefreshRateHz);
public sealed record DiskInfo(string Model, ulong SizeBytes, string MediaType, string BusType);
public sealed record VolumeInfo(string Letter, string FileSystem, ulong SizeBytes, ulong FreeBytes);
public sealed record NetworkAdapterInfo(string Name, ulong? LinkSpeedBps);

using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace SpecPaste;

internal static class Cli
{
    private const int StdOutputHandle = -11;
    private const int AttachParentProcess = -1;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AttachConsole(int processId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int handleId);

    public static void Print(string text)
    {
        try
        {
            var handle = GetStdHandle(StdOutputHandle);
            if (handle == IntPtr.Zero || handle == new IntPtr(-1))
            {
                AttachConsole(AttachParentProcess);
                handle = GetStdHandle(StdOutputHandle);
            }
            if (handle == IntPtr.Zero || handle == new IntPtr(-1))
                return;

            using var fs = new FileStream(new SafeFileHandle(handle, ownsHandle: false), FileAccess.Write);
            using var writer = new StreamWriter(fs, new UTF8Encoding(false)) { AutoFlush = true };
            writer.Write(text);
            writer.Write("\r\n");
        }
        catch
        {
            // no console to write to (e.g. launched from Explorer) — nothing to print
        }
    }
}

using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace SpecPaste;

public partial class MainWindow : Window
{
    private SystemInfo _info = new(null, null, null, null, null, null, null, null);

    public MainWindow()
    {
        InitializeComponent();
        Refresh();
    }

    private void Refresh()
    {
        _info = new Collector().Collect();
        View.Text = Formatter.Text(_info);
    }

    private void OnRefresh(object sender, RoutedEventArgs e) => Refresh();

    private void OnCopyText(object sender, RoutedEventArgs e) => Clipboard.SetText(Formatter.Text(_info));
    private void OnCopyMarkdown(object sender, RoutedEventArgs e) => Clipboard.SetText(Formatter.Markdown(_info));
    private void OnCopyBbCode(object sender, RoutedEventArgs e) => Clipboard.SetText(Formatter.BbCode(_info));

    private void OnSave(object sender, RoutedEventArgs e)
    {
        var dlg = new SaveFileDialog
        {
            Filter = "文字檔案 (*.txt)|*.txt",
            FileName = "SpecPaste.txt",
            DefaultExt = ".txt",
        };
        if (dlg.ShowDialog(this) == true)
            File.WriteAllText(dlg.FileName, Formatter.Text(_info));
    }
}

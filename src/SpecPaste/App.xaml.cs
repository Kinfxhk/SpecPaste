using System.Windows;

namespace SpecPaste;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        if (e.Args.Length > 0)
        {
            var info = new Collector().Collect();
            var text = e.Args[0] switch
            {
                "--text" => Formatter.Text(info),
                "--markdown" => Formatter.Markdown(info),
                "--bbcode" => Formatter.BbCode(info),
                _ => null,
            };
            if (text is not null)
            {
                Cli.Print(text);
                Shutdown(0);
                return;
            }
        }

        new MainWindow().Show();
    }
}

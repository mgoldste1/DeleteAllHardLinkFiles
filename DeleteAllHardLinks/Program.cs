using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Config;
using Microsoft.Extensions.Configuration;

 // Load configuration
var config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .Build();

bool loggingEnabled = config["Logging:Enabled"]?.ToLower() == "true";
ILog? logger = null;
if (loggingEnabled)
    logger = SetupLogger();

if (args.Length == 0)
{
    if (logger != null) logger.Info("No arguments provided. Exiting.");
    return;
}
else 
{
    if (logger != null) logger.Info($"Arguments: {string.Join(", ", args)}");
    await Task.WhenAll(args.ToList().Select(o => DeleteHLs(o, logger)));
}

static ILog SetupLogger()
{
    var repo = LogManager.CreateRepository("DeleteAllHardLinksRepo");
    var layout = new PatternLayout("%date [%thread] %-5level %logger - %message%newline");
    layout.ActivateOptions();

    var exeDir = AppDomain.CurrentDomain.BaseDirectory;
    var appender = new RollingFileAppender
    {
        File = Path.Combine(exeDir, "log.txt"),
        AppendToFile = true,
        MaxSizeRollBackups = 1, // keep only one rolled file
        MaximumFileSize = "10MB",
        RollingStyle = RollingFileAppender.RollingMode.Size,
        StaticLogFileName = true,
        PreserveLogFileNameExtension = true, // rolled file keeps .txt extension
        CountDirection = 1, // active stays log.txt, rolled becomes log.1.txt
        Layout = layout,
        LockingModel = new FileAppender.MinimalLock()
    };
    appender.ActivateOptions();

    BasicConfigurator.Configure(repo, appender);
    return LogManager.GetLogger(repo.Name, "DeleteAllHardLinksLogger");
}

static async Task DeleteHLs(string arg, ILog? logger)
{
    if (!string.IsNullOrWhiteSpace(arg))
    {
        FileInfo fi = new(arg);
        if (fi.Exists)
        {
            logger?.Info($"Processing file: {arg}");
            string output = await ExecuteAsync("CMD.exe", $"/C ln -l \"{arg}\"");
            logger?.Debug($"ln output: {output}");
            output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                  .Where(o => o.Substring(1, 1) == ":")
                  .Select(o => o.Trim()).ToList()
                  .ForEach(o => {
                      try
                      {
                          logger?.Info($"Sending to recycle bin: {o}");
                          o.ToRecyclingBin();
                      }
                      catch (Exception ex)
                      {
                          logger?.Error($"Error deleting {o}: {ex.Message}");
                      }
                  });
        }
        else
        {
            if (logger != null) logger.Warn($"File does not exist: {arg}");
        }
    }
}

static async Task<string> ExecuteAsync(string exePath, string parameters)
{
    string result = string.Empty;

    using (Process p = new Process())
    {
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.FileName = exePath;
        p.StartInfo.Arguments = parameters;
        p.Start();
        await p.WaitForExitAsync();
        result = p.StandardOutput.ReadToEnd();
    }
    return result;
}

public static class Extensions
{
    public static void ToRecyclingBin(this FileInfo fi)
    {
        if (fi.Exists) 
            FileSystem.DeleteFile(fi.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
    }
    public static void ToRecyclingBin(this string fi) => ToRecyclingBin(new FileInfo(fi));
}
//drop shortcut to exe in C:\Users\[username]\AppData\Roaming\Microsoft\Windows\SendTo
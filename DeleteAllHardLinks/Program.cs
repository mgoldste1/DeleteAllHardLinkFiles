using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;

if (!string.IsNullOrWhiteSpace(args[0]))
{
    FileInfo fi = new(args[0]);
    if (fi.Exists)
    {
        string output = Execute("CMD.exe", $"/C ln -l \"{args[0]}\"");
        output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
              .Where(o => o.Substring(1, 1) == ":")
              .Select(o => o.Trim()).ToList()
              .ForEach(o => o.ToRecyclingBin());
    }
}

static string Execute(string exePath, string parameters)
{
    string result = String.Empty;

    using (Process p = new Process())
    {
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.FileName = exePath;
        p.StartInfo.Arguments = parameters;
        p.Start();
        p.WaitForExit();
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
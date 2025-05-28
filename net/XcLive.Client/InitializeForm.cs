using System.IO.Compression;
using Frame.Data;
using Frame.Utils;

namespace Frame;

public partial class InitializeForm : Form
{
    public static readonly string TmpPathName = "tmp";
    private static readonly string SuccFileName = "初始化完成.txt";

    public InitializeForm()
    {
        InitializeComponent();
        Ini();
        Start();
    }

    private void Ini()
    {
        Icon = new Icon("res/star.ico");
        Task.Factory.StartNew(() =>
        {
            try
            {
                var title = AppHelper.GetFormTitle();
                Invoke(() => { Text = title; });
            }
            catch (Exception e)
            {
                Log4.Log.Error(e);
            }
        });
    }

    public static bool CheckBefore()
    {
        string outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TmpPathName);
        var filePath = Path.Combine(outputDir, SuccFileName);
        return File.Exists(filePath);
    }

    private void Start()
    {
        string outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TmpPathName);
        var filePath = Path.Combine(outputDir, SuccFileName);

        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        Task.Factory.StartNew(() =>
        {
            var list = new List<Tuple<string, string>>()
            {
                //new("chromiumembeddedframework.runtime.win-x64", "135.0.17"),
                //new("CefSharp.Common.NETCore", "135.0.170"),
            };
            var taskList = list.Select(l =>
            {
                return Task.Factory.StartNew(() =>
                {
                    try
                    {
                        return StartDownload(outputDir, l.Item1, l.Item2);
                    }
                    catch (Exception e)
                    {
                        Log4.Log.Error(e);
                        return new WgetHelper.DownloadMessage
                        {
                            Message = e.Message,
                            Status = WgetHelper.DownloadStatus.Failed
                        };
                    }
                });
            }).ToList();
            taskList.Add(Task.Factory.StartNew(() =>
            {
                try
                {
                    var url = "https://files.erp12345.com/live/虚拟麦克风.exe";
                    var result = WgetHelper.DownloadFileAsync(url, Path.Combine(outputDir, "../tools/虚拟麦克风.exe")).Result;
                    return result;
                }
                catch (Exception e)
                {
                    Log4.Log.Error(e);
                    return new WgetHelper.DownloadMessage
                    {
                        Message = e.Message,
                        Status = WgetHelper.DownloadStatus.Failed
                    };
                }
            }));
            taskList.Add(Task.Factory.StartNew(() =>
            {
                try
                {
                    var url = "https://files.erp12345.com/live/Chrome99.zip";
                    var result = WgetHelper.DownloadFileAsync(url, Path.Combine(outputDir, "Chrome99.zip")).Result;
                    if (result.Status == WgetHelper.DownloadStatus.Completed)
                    {
                        var r = ExtractFoldersFromNupkg(Path.Combine(outputDir, "Chrome99.zip"),
                            Path.Combine(outputDir, "../tools/"), ["Chrome99"]);
                        if (!r.success)
                        {
                            result.Status = WgetHelper.DownloadStatus.Failed;
                            result.Message = "下载完成，但解压失败" + r.message;
                        }
                    }

                    return result;
                }
                catch (Exception e)
                {
                    Log4.Log.Error(e);
                    return new WgetHelper.DownloadMessage
                    {
                        Message = e.Message,
                        Status = WgetHelper.DownloadStatus.Failed
                    };
                }
            }));
            taskList.Add(Task.Factory.StartNew(() =>
            {
                try
                {
                    var url = "https://files.erp12345.com/live/runtimes.zip";
                    var result = WgetHelper.DownloadFileAsync(url, Path.Combine(outputDir, "runtimes.zip"),(s, e) => { ShowProgress("chromiumembeddedframework.runtime.win-x64", e); }).Result;
                    if (result.Status==WgetHelper.DownloadStatus.Completed)
                    {
                        var r = ExtractFoldersFromNupkg(Path.Combine(outputDir, "runtimes.zip"), Path.Combine(outputDir, "../"), ["runtimes"]);
                        if (!r.success)
                        {
                            result.Status = WgetHelper.DownloadStatus.Failed;
                            result.Message = "下载完成，但解压失败" + r.message;
                        }
                    }
                    
                    return result;
                }
                catch (Exception e)
                {
                    Log4.Log.Error(e);
                    return new WgetHelper.DownloadMessage
                    {
                        Message = e.Message,
                        Status = WgetHelper.DownloadStatus.Failed
                    };
                }
            }));

            var results = Task.WhenAll(taskList).Result;
            var err = results.FirstOrDefault(r => r.Status != WgetHelper.DownloadStatus.Completed);
            if (err != null)
            {
                MessageBox.Show(err.Message, "出错");
                return;
            }

            File.WriteAllText(filePath, $"{DateTime.Now}初始化完成，如需要重新初始化请删除本文件");
            Invoke(ReturnOk);
        });
    }

    private WgetHelper.DownloadMessage StartDownload(string outputDir, string packageId, string version)
    {
        var url =
            $"https://globalcdn.nuget.org/packages/{packageId.ToLower()}.{version}.nupkg?packageVersion={version}";
        var fileName = $"{packageId}.{version}.nupkg";
        var outputPath = Path.Combine(outputDir, fileName);
        var result = WgetHelper.DownloadFileAsync(url, outputPath, (s, e) => { ShowProgress(fileName, e); }).Result;
        
        //解压文件不能并发，防止有相同文件名
        lock (this)
        {
            var runtimesPath = Directory.GetParent(outputDir)?.ToString();
            if (runtimesPath == null)
            {
                result.Message = "目录处理出错，请检查当前目录是否有问题";
                result.Status = WgetHelper.DownloadStatus.Failed;
                return result;
            }

            var r = ExtractFoldersFromNupkg(outputPath, runtimesPath, ["runtimes"]);
            if (!r.success)
            {
                result.Status = WgetHelper.DownloadStatus.Failed;
                result.Message = "下载完成，但解压失败" + r.message;
            }
            else
            {
                ExtractFoldersFromNupkg(outputPath, runtimesPath + "/runtimes/win-x64/native/", ["CEF/win-x64/"]);
                FileHelper.MoveDirectory(runtimesPath + "/runtimes/win-x64/native/CEF/win-x64/",
                    runtimesPath + "/runtimes/win-x64/native/");
                FileHelper.Copy(runtimesPath + "/runtimes/win-x64/lib/netcoreapp3.1/CefSharp.dll",
                    runtimesPath + "/runtimes/win-x64/native/CefSharp.dll");
                FileHelper.Copy(runtimesPath + "/runtimes/win-x64/lib/netcoreapp3.1/Ijwhost.dll",
                    runtimesPath + "/runtimes/win-x64/native/Ijwhost.dll");
            }
        }


        return result;
    }

    private void ShowProgress(string fileName, WgetHelper.DownloadMessage downloadMessage)
    {
        if (!fileName.Contains("chromiumembeddedframework.runtime.win-x64"))
        {
            return;
        }

        Invoke(() =>
        {
            lblTime.Text = $"用时：{(int)(DateTime.Now - startTime).TotalSeconds}秒";
            lblFileName.Text = $"";
            progressBar1.Value = (int)(downloadMessage.PercentComplete * 100);
        });
    }

    DateTime startTime = DateTime.Now;

    public static R ExtractFoldersFromNupkg(string nupkgPath, string outputDir, List<string> folderPrefixes)
    {
        try
        {
            using var archive = ZipFile.OpenRead(nupkgPath);
            foreach (var entry in archive.Entries)
            {
                // 检查 entry 是否在指定的目录下
                bool match = folderPrefixes.Any(prefix =>
                    entry.FullName.Replace('\\', '/')
                        .StartsWith(prefix.TrimEnd('/') + "/", StringComparison.OrdinalIgnoreCase));

                if (!match || string.IsNullOrEmpty(entry.Name)) continue; // 排除目录

                // 目标文件路径
                var destPath = Path.Combine(outputDir, entry.FullName);

                // 如果文件已存在，跳过
                /*if (File.Exists(destPath))
                {
                    continue;
                }*/

                // 确保目录存在
                Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);

                // 解压文件
                using var file = File.Create(destPath);
                entry.Open().CopyTo(file);
            }
        }
        catch (Exception e)
        {
            return R.Faild(e.Message);
        }

        return R.OK();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        ReturnOk();
    }

    private void ReturnOk()
    {
        DialogResult = DialogResult.OK;
    }
}

public static class FileHelper
{
    public static void MoveDirectory(string sourceDir, string targetDir)
    {
        if (!Directory.Exists(sourceDir))
            return;

        if (!Directory.Exists(targetDir))
            Directory.CreateDirectory(targetDir);

        foreach (var filePath in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(sourceDir, filePath);
            string targetPath = Path.Combine(targetDir, relativePath);
            string? targetDirPath = Path.GetDirectoryName(targetPath);

            if (!Directory.Exists(targetDirPath))
                Directory.CreateDirectory(targetDirPath!);

            // 如果目标文件存在，先删除再移动
            if (File.Exists(targetPath))
                File.Delete(targetPath);

            File.Move(filePath, targetPath);
        }

        // 移动完成后清空空目录
        DeleteEmptyDirs(sourceDir);
    }

    private static void DeleteEmptyDirs(string dir)
    {
        foreach (var subDir in Directory.GetDirectories(dir))
        {
            DeleteEmptyDirs(subDir);
        }

        if (Directory.GetFiles(dir).Length == 0 && Directory.GetDirectories(dir).Length == 0)
        {
            Directory.Delete(dir);
        }
    }

    public static void Copy(string sourceFileName, string targetFileName)
    {
        try
        {
            if (File.Exists(sourceFileName) && !File.Exists(targetFileName))
            {
                File.Copy(sourceFileName, targetFileName);
            }
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
        }
    }
}
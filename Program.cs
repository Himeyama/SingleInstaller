using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;

internal class Program
{
    const string outputFileName = "package.zip";

    private static async Task Main()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = "SingleInstaller.resources.package.zip"; // 名前空間を正確に指定！

        Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            // Console.WriteLine("リソースが見つかりませんでした！");
            return;
        }

        using (FileStream fileStream = new(outputFileName, FileMode.Create))
        {
            stream.CopyTo(fileStream);
        }

        // 展開先ディレクトリ
        string extractDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

        if (Directory.Exists(extractDir))
            Directory.Delete(extractDir, true);

        Directory.CreateDirectory(extractDir);

        // 展開したZIPファイルを解凍
        using (ZipArchive archive = ZipFile.OpenRead(outputFileName))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string destinationPath = Path.Combine(extractDir, entry.FullName);

                // サブディレクトリがある場合に備えて作成
                string? directory = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                // ディレクトリの場合はスキップ
                if (string.IsNullOrEmpty(entry.Name))
                    continue;

                // 既存のファイルを展開
                entry.ExtractToFile(destinationPath, true);
            }
        }

        // zipファイルを削除
        if (File.Exists(outputFileName))
            File.Delete(outputFileName);

        await RunInstaller(extractDir);
        await RemoveFiles(extractDir);
    }

    static async Task RemoveFiles(string extractDir)
    {
        while (true)
        {
            await Task.Delay(5000);

            // プロセスが起動しているか確認
            Process[] processes = Process.GetProcessesByName("HDS");
            if (processes.Length == 0)
            {
                Directory.Delete(extractDir, recursive: true);
                return;
            }
        }
    }

    static async Task RunInstaller(string extractDir)
    {
        using Process process = new()
        {
            StartInfo = new ProcessStartInfo
            {
                WorkingDirectory = extractDir,
                FileName = Path.Combine(extractDir, "HDS", "HDS.exe"),
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            }
        };

        process.Start();
        await process.WaitForExitAsync();
    }
}
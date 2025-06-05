using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;

internal class Program
{
    const string outputFileName = "package.zip";

    private static void Main()
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
        // Console.WriteLine("リソースが展開されました！");

        // 展開先ディレクトリ
        string extractDir = Path.Combine(Path.GetTempPath(), "tmp_installer");

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

        _ = RunInstaller(extractDir);
    }

    static async Task RunInstaller(string extractDir){
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
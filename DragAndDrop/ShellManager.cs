using System.Runtime.InteropServices;

public static class ShellManager
{
    // Константа для события "содержимое папки изменилось"
    private const int SHCNE_UPDATEDIR = 0x00001000;
    // Флаг, указывающий, что мы передаем путь к папке
    private const uint SHCNF_PATH = 0x0005;
    private const int SHCNE_ASSOCCHANGED = 0x08000000;
    private const uint SHCNF_IDLIST = 0x0000;

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern void SHChangeNotify(int wEventId, uint uFlags, string? dwItem1, string? dwItem2);

    /// <summary>
    /// Уведомляет Проводник Windows о том, что содержимое папки изменилось.
    /// Это заставит Проводник перечитать файлы в указанной папке.
    /// </summary>
    /// <param name="folderPath">Полный путь к папке.</param>
    public static void NotifyFolderUpdated(string folderPath)
    {
        SHChangeNotify(SHCNE_UPDATEDIR, SHCNF_PATH, folderPath, null);
    }

    /// <summary>
    /// Принудительно сообщает оболочке Windows, что состояние файловой системы изменилось.
    /// Полезно для случаев, когда Проводник не обновляет список автоматически.
    /// </summary>
    public static void ForceRefreshExplorer()
    {
        SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, null, null);
    }
}

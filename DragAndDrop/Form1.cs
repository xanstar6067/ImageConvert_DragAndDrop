using ImageMagick; // Для Magick.NET
using System.Media;
using Microsoft.VisualBasic.FileIO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DragAndDrop
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            // Загрузка форматов в ComboBox
            cmbOutputFormat.Items.AddRange(_supportedOutputFormats.Keys.ToArray());

            // Загрузка настроек
            LoadSettings();

            // Настройка событий
            chkStayOnTop.CheckedChanged += ChkStayOnTop_CheckedChanged;

            // Drag and Drop для формы и для Label
            this.AllowDrop = true;
            this.DragEnter += Form_DragEnter;
            this.DragDrop += Form_DragDrop;

            dropAreaLabel.AllowDrop = true; // Убедитесь, что это свойство установлено в дизайнере или здесь
            dropAreaLabel.DragEnter += Form_DragEnter; // Используем те же обработчики
            dropAreaLabel.DragDrop += Form_DragDrop;

            // Обновить состояние UI на основе загруженных настроек
            UpdateUiFromSettings();
        }

        // Список поддерживаемых форматов для вывода
        private readonly Dictionary<string, MagickFormat> _supportedOutputFormats = new()
        {
            { "JPG", MagickFormat.Jpg },
            { "PNG", MagickFormat.Png },
            { "WEBP", MagickFormat.WebP },
            { "AVIF", MagickFormat.Avif },
            { "BMP", MagickFormat.Bmp },
            { "GIF", MagickFormat.Gif },
            { "TIFF", MagickFormat.Tiff }
        };

        // Список расширений, которые считаем изображениями
        private readonly List<string> _validImageExtensions = new()
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".tif",
            ".webp", ".avif", ".heic", ".heif", ".svg", ".ico" // Добавьте другие, если нужно
            // Magick.NET поддерживает многие, но это для быстрой фильтрации
        };


        private const int MaxLogLines = 12;
        private const int MaxLogMessageLength = 130;
        private const int MaxPathLengthInLog = 40;
        private readonly Queue<string> _logLines = new();
        private const string DuplicateFilePrefix = "copy_";
        private static readonly Regex WindowsPathRegex = new(@"[A-Za-z]:\\[^\s\r\n\t]+", RegexOptions.Compiled);

        private void LogMessage(string message)
        {
            if (dropAreaLabel.InvokeRequired)
            {
                dropAreaLabel.Invoke(new Action<string>(LogMessage), message);
                return;
            }

            string normalizedMessage = NormalizeLogMessage(message);
            _logLines.Enqueue($"{DateTime.Now:HH:mm:ss} {normalizedMessage}");

            while (_logLines.Count > MaxLogLines)
            {
                _logLines.Dequeue();
            }

            dropAreaLabel.Text = string.Join(Environment.NewLine + Environment.NewLine, _logLines);
        }

        private static string NormalizeLogMessage(string message)
        {
            string trimmedMessage = message.Trim();
            string levelPrefix = ResolveLogLevelPrefix(trimmedMessage);
            string withShortPaths = ShortenLongPaths(trimmedMessage);
            string wrappedMessage = WrapByWords(withShortPaths, MaxLogMessageLength);

            if (string.IsNullOrEmpty(levelPrefix))
            {
                return wrappedMessage;
            }

            return $"{levelPrefix} {wrappedMessage}";
        }

        private static string ResolveLogLevelPrefix(string message)
        {
            if (message.StartsWith("[INFO]", StringComparison.OrdinalIgnoreCase) ||
                message.StartsWith("[WARN]", StringComparison.OrdinalIgnoreCase) ||
                message.StartsWith("[ERR]", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            if (message.StartsWith("Ошибка", StringComparison.OrdinalIgnoreCase) ||
                message.StartsWith("Критическая ошибка", StringComparison.OrdinalIgnoreCase))
            {
                return "[ERR]";
            }

            if (message.StartsWith("Внимание", StringComparison.OrdinalIgnoreCase))
            {
                return "[WARN]";
            }

            return "[INFO]";
        }

        private static string ShortenLongPaths(string message)
        {
            return WindowsPathRegex.Replace(message, match =>
            {
                string fullMatch = match.Value;
                string trimmedPath = fullMatch.TrimEnd('.', ',', ';', ':', ')', ']');
                string trailingChars = fullMatch[trimmedPath.Length..];

                if (trimmedPath.Length <= MaxPathLengthInLog)
                {
                    return fullMatch;
                }

                return $"{ApplyMiddleEllipsis(trimmedPath)}{trailingChars}";
            });
        }

        private static string ApplyMiddleEllipsis(string path)
        {
            if (path.Length <= MaxPathLengthInLog)
            {
                return path;
            }

            string root = Path.GetPathRoot(path)?.TrimEnd('\\') ?? string.Empty;
            string fileName = Path.GetFileName(path);

            if (!string.IsNullOrEmpty(root) && !string.IsNullOrEmpty(fileName))
            {
                return $"{root}\\...\\{fileName}";
            }

            int tailLength = Math.Max(10, MaxPathLengthInLog - 5);
            return $"...{path[^Math.Min(path.Length, tailLength)..]}";
        }

        private static string WrapByWords(string text, int maxLineLength)
        {
            var paragraphs = text.Split(Environment.NewLine);
            var wrappedParagraphs = new List<string>(paragraphs.Length);

            foreach (string paragraph in paragraphs)
            {
                if (string.IsNullOrWhiteSpace(paragraph))
                {
                    wrappedParagraphs.Add(string.Empty);
                    continue;
                }

                var words = paragraph.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var currentLine = new StringBuilder();
                var wrapped = new StringBuilder();

                foreach (string word in words)
                {
                    if (currentLine.Length == 0)
                    {
                        currentLine.Append(word);
                        continue;
                    }

                    if (currentLine.Length + 1 + word.Length > maxLineLength)
                    {
                        wrapped.AppendLine(currentLine.ToString());
                        currentLine.Clear();
                    }

                    if (currentLine.Length > 0)
                    {
                        currentLine.Append(' ');
                    }

                    currentLine.Append(word);
                }

                if (currentLine.Length > 0)
                {
                    wrapped.Append(currentLine);
                }

                wrappedParagraphs.Add(wrapped.ToString().TrimEnd());
            }

            return string.Join(Environment.NewLine, wrappedParagraphs);
        }

        #region Settings Load/Save

        private void LoadSettings()
        {
            chkStayOnTop.Checked = Properties.Settings.Default.StayOnTop;
            cmbOutputFormat.SelectedItem = _supportedOutputFormats.ContainsKey(Properties.Settings.Default.OutputFormat)
                                           ? Properties.Settings.Default.OutputFormat
                                           : "JPG"; // Значение по умолчанию, если сохранено некорректное
            tbQuality.Value = Math.Clamp(Properties.Settings.Default.Quality, tbQuality.Minimum, tbQuality.Maximum);
            chkDeleteOriginal.Checked = Properties.Settings.Default.DeleteOriginal;
            chkClearMeta.Checked = Properties.Settings.Default.ClearMeta;
            chkCustomOutputFolder.Checked = Properties.Settings.Default.UseCustomOutput;
            txtCustomOutputPath.Text = Properties.Settings.Default.CustomOutputPath;

            RestoreWindowPlacement();
            LogMessage("Настройки загружены.");
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.StayOnTop = chkStayOnTop.Checked;
            Properties.Settings.Default.OutputFormat = cmbOutputFormat.SelectedItem?.ToString() ?? "JPG";
            Properties.Settings.Default.Quality = tbQuality.Value;
            Properties.Settings.Default.DeleteOriginal = chkDeleteOriginal.Checked;
            Properties.Settings.Default.ClearMeta = chkClearMeta.Checked;
            Properties.Settings.Default.UseCustomOutput = chkCustomOutputFolder.Checked;
            Properties.Settings.Default.CustomOutputPath = txtCustomOutputPath.Text;

            SaveWindowPlacement();
            Properties.Settings.Default.Save();
            LogMessage("Настройки сохранены.");
        }

        private void RestoreWindowPlacement()
        {
            int savedWidth = Properties.Settings.Default.WindowWidth;
            int savedHeight = Properties.Settings.Default.WindowHeight;

            if (savedWidth <= 0 || savedHeight <= 0)
            {
                return;
            }

            var savedBounds = new Rectangle(
                Properties.Settings.Default.WindowLeft,
                Properties.Settings.Default.WindowTop,
                savedWidth,
                savedHeight);

            Screen? targetScreen = TryGetScreenByDeviceName(Properties.Settings.Default.LastScreenDeviceName);
            if (targetScreen == null || !IsRectangleVisibleOnScreen(savedBounds, targetScreen.WorkingArea))
            {
                targetScreen = Screen.FromRectangle(savedBounds);
            }

            Rectangle restoredBounds = savedBounds;
            if (!IsRectangleVisibleOnScreen(restoredBounds, targetScreen.WorkingArea))
            {
                restoredBounds = new Rectangle(
                    targetScreen.WorkingArea.Left + Math.Max(0, (targetScreen.WorkingArea.Width - savedBounds.Width) / 2),
                    targetScreen.WorkingArea.Top + Math.Max(0, (targetScreen.WorkingArea.Height - savedBounds.Height) / 2),
                    Math.Min(savedBounds.Width, targetScreen.WorkingArea.Width),
                    Math.Min(savedBounds.Height, targetScreen.WorkingArea.Height));
            }

            StartPosition = FormStartPosition.Manual;
            DesktopBounds = restoredBounds;
            LogMessage($"Положение окна восстановлено: {targetScreen.DeviceName}.");
        }

        private void SaveWindowPlacement()
        {
            Rectangle bounds = DesktopBounds;
            Screen currentScreen = Screen.FromRectangle(bounds);

            Properties.Settings.Default.WindowLeft = bounds.Left;
            Properties.Settings.Default.WindowTop = bounds.Top;
            Properties.Settings.Default.WindowWidth = bounds.Width;
            Properties.Settings.Default.WindowHeight = bounds.Height;
            Properties.Settings.Default.LastScreenDeviceName = currentScreen.DeviceName;
        }

        private static Screen? TryGetScreenByDeviceName(string? deviceName)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
            {
                return null;
            }

            return Screen.AllScreens.FirstOrDefault(screen =>
                string.Equals(screen.DeviceName, deviceName, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsRectangleVisibleOnScreen(Rectangle bounds, Rectangle workingArea)
        {
            Rectangle intersection = Rectangle.Intersect(bounds, workingArea);
            return intersection.Width >= 100 && intersection.Height >= 100;
        }

        private void UpdateUiFromSettings()
        {
            // Применить "Поверх всех окон"
            this.TopMost = chkStayOnTop.Checked;

            // Обновить метку качества
            lblQualityValue.Text = tbQuality.Value.ToString();

            // Обновить доступность элементов для выбора папки
            txtCustomOutputPath.Enabled = chkCustomOutputFolder.Checked;
            btnBrowseOutputFolder.Enabled = chkCustomOutputFolder.Checked;
        }

        #endregion

        #region Event Handlers
        private void ChkStayOnTop_CheckedChanged(object? sender, EventArgs e)
        {
            this.TopMost = chkStayOnTop.Checked;
        }

        private void TbQuality_Scroll(object? sender, EventArgs e)
        {
            lblQualityValue.Text = tbQuality.Value.ToString();
        }

        private void ChkCustomOutputFolder_CheckedChanged(object? sender, EventArgs e)
        {
            bool isChecked = chkCustomOutputFolder.Checked;
            txtCustomOutputPath.Enabled = isChecked;
            btnBrowseOutputFolder.Enabled = isChecked;
            /*txtCustomOutputPath.Enabled = chkCustomOutputFolder.Checked;
            btnBrowseOutputFolder.Enabled = chkCustomOutputFolder.Checked;
            if (chkCustomOutputFolder.Checked && string.IsNullOrWhiteSpace(txtCustomOutputPath.Text))
            {
                BtnBrowseOutputFolder_Click(null, EventArgs.Empty); // Предложить выбрать папку
            }*/

            // Если пользователь снимает галочку, можно опционально очистить путь
            // if (!isChecked)
            // {
            //     txtCustomOutputPath.Text = string.Empty;
            //     // И, возможно, обновить это в настройках, если такая логика нужна
            //     // Properties.Settings.Default.CustomOutputPath = string.Empty; 
            // }

        }

        private void BtnBrowseOutputFolder_Click(object? sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Выберите папку для сохранения сконвертированных файлов";
                if (!string.IsNullOrWhiteSpace(txtCustomOutputPath.Text) && Directory.Exists(txtCustomOutputPath.Text))
                {
                    dialog.SelectedPath = txtCustomOutputPath.Text;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtCustomOutputPath.Text = dialog.SelectedPath;
                    LogMessage($"Папка для вывода изменена: {txtCustomOutputPath.Text}");
                }
            }
        }

        private void Form_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Проверяем, что хотя бы один из файлов - изображение
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Any(f => _validImageExtensions.Contains(Path.GetExtension(f).ToLowerInvariant())))
                {
                    e.Effect = DragDropEffects.Copy; // Показать, что можно скопировать
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private async void Form_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data == null || !e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var imageFiles = files.Where(f => _validImageExtensions.Contains(Path.GetExtension(f).ToLowerInvariant())).ToList();

            if (!imageFiles.Any())
            {
                LogMessage("Перетащены файлы, не являющиеся изображениями, или с неподдерживаемым расширением.");
                MessageBox.Show("Пожалуйста, перетащите только файлы изображений (например, .jpg, .png, .webp, .avif).", "Неверный тип файла", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (chkCustomOutputFolder.Checked && (string.IsNullOrWhiteSpace(txtCustomOutputPath.Text) || !Directory.Exists(txtCustomOutputPath.Text)))
            {
                MessageBox.Show("Указана пользовательская папка для вывода, но путь не задан или папка не существует. Пожалуйста, выберите корректную папку.", "Ошибка папки вывода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogMessage("Ошибка: Не указана или не существует папка для вывода.");
                return;
            }

            dropAreaLabel.Enabled = false; // Блокируем перетаскивание во время обработки

            try
            {
                LogMessage($"Начата обработка {imageFiles.Count} файлов.");

                // >>> НОВОЕ: Создаем коллекцию для хранения УНИКАЛЬНЫХ папок, которые были изменены.
                var affectedDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var filePath in imageFiles)
                {
                    // >>> ИЗМЕНЕНИЕ: Передаем коллекцию в метод, чтобы он ее наполнял.
                    await ConvertImageAsync(filePath, affectedDirectories);
                }

                // >>> НОВОЕ: После того как все файлы обработаны, отправляем по одному уведомлению для каждой затронутой папки.
                foreach (var dirPath in affectedDirectories)
                {
                    ShellManager.NotifyFolderUpdated(dirPath);
                }

                if (affectedDirectories.Any())
                {
                    ShellManager.ForceRefreshExplorer();
                    LogMessage($"Проводник Windows уведомлен об изменениях в {affectedDirectories.Count} папках.");
                }

                LogMessage("Обработка завершена.");
                if (imageFiles.Count > 0) // Проверяем, были ли файлы для обработки
                {
                    SystemSounds.Exclamation.Play(); // Или другой звук на ваш выбор
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Критическая ошибка в процессе обработки: {ex.Message}");
            }
            finally
            {
                dropAreaLabel.Enabled = true; // Разблокируем
            }
            //MessageBox.Show($"Обработка {imageFiles.Count} файлов завершена.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private async Task ConvertImageAsync(string sourceFilePath, ISet<string> directoriesToRefresh)
        {
            try
            {
                LogMessage($"Конвертация: {Path.GetFileName(sourceFilePath)}...");

                // Получаем значения из UI контролов ДО вызова Task.Run
                string? selectedFormatKey = null;
                MagickFormat outputMagickFormat = MagickFormat.Unknown; // Инициализируем
                bool useCustomFolder = false;
                string customPath = "";
                bool deleteOriginal = false;
                bool clearMeta = false;
                int qualityValue = 0; // Переменная для хранения значения качества

                // Используем Invoke для безопасного доступа к контролам из UI потока
                // Это гарантирует, что мы читаем актуальные значения из правильного потока
                this.Invoke((MethodInvoker)delegate {
                    selectedFormatKey = cmbOutputFormat.SelectedItem?.ToString();
                    if (selectedFormatKey != null && _supportedOutputFormats.TryGetValue(selectedFormatKey, out var format))
                    {
                        outputMagickFormat = format;
                    }
                    useCustomFolder = chkCustomOutputFolder.Checked;
                    customPath = txtCustomOutputPath.Text;
                    deleteOriginal = chkDeleteOriginal.Checked;
                    clearMeta = chkClearMeta.Checked;
                    qualityValue = tbQuality.Value; // Получаем значение качества здесь
                });

                if (selectedFormatKey == null || outputMagickFormat == MagickFormat.Unknown)
                {
                    LogMessage($"Ошибка: Не выбран или не поддерживается выходной формат для {Path.GetFileName(sourceFilePath)}.");
                    return;
                }

                string outputExtension = selectedFormatKey.ToLowerInvariant();
                string outputFileName = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}.{outputExtension}";
                string? outputDirectory; // Используем nullable для большей безопасности

                if (useCustomFolder && !string.IsNullOrWhiteSpace(customPath) && Directory.Exists(customPath))
                {
                    outputDirectory = customPath;
                }
                else
                {
                    outputDirectory = Path.GetDirectoryName(sourceFilePath);
                }

                if (string.IsNullOrEmpty(outputDirectory))
                {
                    LogMessage($"Ошибка: Не удалось определить папку для сохранения {outputFileName}.");
                    return;
                }

                string outputFilePath = Path.Combine(outputDirectory, outputFileName);

                if (string.Equals(sourceFilePath, outputFilePath, StringComparison.OrdinalIgnoreCase))
                {
                    string originalName = Path.GetFileNameWithoutExtension(sourceFilePath);
                    // Используем outputExtension для _converted файла, чтобы он имел правильное расширение
                    outputFilePath = Path.Combine(outputDirectory, $"{originalName}_converted.{outputExtension}");
                    LogMessage($"Внимание: Исходный и целевой файлы совпадают. Результат будет сохранен как {Path.GetFileName(outputFilePath)}");
                }

                outputFilePath = GetUniqueOutputFilePath(outputFilePath);

                // Используем Task.Run для выноса тяжелой операции в фоновый поток
                await Task.Run(() =>
                {
                    using (var image = new MagickImage(sourceFilePath))
                    {
                        image.Format = outputMagickFormat;
                        if (outputMagickFormat == MagickFormat.Jpg ||
                            outputMagickFormat == MagickFormat.WebP ||
                            outputMagickFormat == MagickFormat.Avif)
                        {
                            // Используем сохраненное значение qualityValue
                            image.Quality = (uint)qualityValue;
                        }

                        if (clearMeta)
                        {
                            image.Strip();
                        }

                        image.Write(outputFilePath);
                    }
                });

                // >>> НОВОЕ: Добавляем папку, в которую сохранили файл, в список для обновления.
                directoriesToRefresh.Add(outputDirectory);

                LogMessage($"Успешно: {Path.GetFileName(sourceFilePath)} -> {Path.GetFileName(outputFilePath)}");

                if (deleteOriginal) // Используем значение, полученное из UI потока
                {
                    try
                    {
                        // Запоминаем исходную папку ПЕРЕД удалением файла
                        string? sourceDirectory = Path.GetDirectoryName(sourceFilePath);

                        // >>> ИЗМЕНЕНИЕ: Заменяем File.Delete на FileSystem.DeleteFile <<<
                        // Эта команда перемещает файл в корзину, а не удаляет навсегда.
                        FileSystem.DeleteFile(
                            sourceFilePath,
                            UIOption.OnlyErrorDialogs, // Не показывать диалоговые окна, кроме ошибок
                            RecycleOption.SendToRecycleBin // Главный параметр - отправить в корзину
                        );

                        // Если исходная папка существует, ее тоже добавляем в список.
                        if (sourceDirectory != null)
                        {
                            directoriesToRefresh.Add(sourceDirectory);
                        }

                        LogMessage($"Перемещен в корзину: {Path.GetFileName(sourceFilePath)}");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Ошибка перемещения в корзину {Path.GetFileName(sourceFilePath)}: {ex.Message}");
                    }
                }
            }
            catch (MagickException magickEx)
            {
                LogMessage($"Ошибка Magick.NET при обработке {Path.GetFileName(sourceFilePath)}: {magickEx.Message}");
            }
            catch (Exception ex)
            {
                LogMessage($"Общая ошибка при обработке {Path.GetFileName(sourceFilePath)}: {ex.Message}");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveSettings();
            base.OnFormClosing(e);
        }

        private string GetUniqueOutputFilePath(string outputFilePath)
        {
            if (!File.Exists(outputFilePath))
            {
                return outputFilePath;
            }

            string? directory = Path.GetDirectoryName(outputFilePath);
            if (string.IsNullOrWhiteSpace(directory))
            {
                return outputFilePath;
            }

            string extension = Path.GetExtension(outputFilePath);
            string fileName = Path.GetFileNameWithoutExtension(outputFilePath);

            string candidatePath = Path.Combine(directory, $"{DuplicateFilePrefix}{fileName}{extension}");
            int suffix = 1;

            while (File.Exists(candidatePath))
            {
                candidatePath = Path.Combine(directory, $"{DuplicateFilePrefix}{suffix}_{fileName}{extension}");
                suffix++;
            }

            LogMessage($"Внимание: Файл {Path.GetFileName(outputFilePath)} уже существует. Результат будет сохранен как {Path.GetFileName(candidatePath)}");

            return candidatePath;
        }
        #endregion
    }
}

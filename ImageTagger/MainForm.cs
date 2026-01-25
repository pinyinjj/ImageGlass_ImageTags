using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageGlass.Tools;

namespace ImageTagger
{
    /// <summary>
    /// Main window for image tagging and category management.
    /// Integrated with ImageGlass via official SDK and WinApi.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly string[] _args;
        private string? _imagePathToAdd = null;
        private ImageGlassTool? _igTool;
        private readonly System.Windows.Forms.Timer _zOrderTimer;

        public MainForm(string[] args)
        {
            InitializeComponent();
            _args = args;
            this.FormClosing += MainForm_FormClosing;

            // Timer to maintain TopMost status without stealing focus
            _zOrderTimer = new System.Windows.Forms.Timer { Interval = 500 };
            _zOrderTimer.Tick += ZOrderTimer_Tick;
            _zOrderTimer.Start();

            SetWindowAlwaysOnTop();
        }

        protected override async void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            
            if (_igTool == null)
            {
                _igTool = new ImageGlassTool();
                _igTool.ToolClosingRequest += _igTool_ToolClosingRequest;
                _igTool.ToolMessageReceived += _igTool_ToolMessageReceived;
                
                await _igTool.ConnectAsync();
                RequestCurrentImage();
            }
        }

        /// <summary>
        /// Proactively requests the current image state from ImageGlass.
        /// Uses reflection to access the underlying communication client.
        /// </summary>
        private async void RequestCurrentImage()
        {
            if (_igTool == null) return;

            try
            {
                var clientField = typeof(ImageGlassTool).GetField("_client", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var client = clientField?.GetValue(_igTool);
                
                if (client != null)
                {
                    var sendAsyncMethod = client.GetType().GetMethod("SendAsync", new[] { typeof(string) });
                    var msgSeparatorProp = typeof(ImageGlassTool).GetProperty("MSG_SEPARATOR", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    
                    if (sendAsyncMethod != null)
                    {
                        string separator = (string?)msgSeparatorProp?.GetValue(null) ?? "{:+IG_TOOL+:}";
                        string message = $"igtool.request.get_image{separator}";
                        
                        var task = (Task?)sendAsyncMethod.Invoke(client, new object[] { message });
                        if (task != null) await task;
                    }
                }
            }
            catch
            {
                // Background synchronization failed silently
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            DataManager.Load();
            RefreshCategoryList();

            // Handle initial startup image data
            if (_args.Length > 0 && !string.IsNullOrWhiteSpace(_args[0]))
            {
                try
                {
                    var args = IgImageLoadedEventArgs.Deserialize(_args[0]);
                    _imagePathToAdd = args?.FilePath;
                }
                catch
                {
                    _imagePathToAdd = _args[0];
                }

                if (!string.IsNullOrWhiteSpace(_imagePathToAdd) && File.Exists(_imagePathToAdd))
                {
                    tabControlMain.SelectedTab = tabPageImageOperations;
                    PopulateDynamicAddButtons();
                }
            }
            else
            {
                 tabControlMain.SelectedTab = tabPageCategories;
            }

            AdjustFormHeight();
        }

        private void ZOrderTimer_Tick(object? sender, EventArgs e) => SetWindowAlwaysOnTop();

        private void SetWindowAlwaysOnTop()
        {
            if (this.IsHandleCreated && !this.Disposing && !this.IsDisposed)
            {
                WinApi.SetWindowPos(this.Handle, WinApi.HWND_TOPMOST, 0, 0, 0, 0, 
                    WinApi.SWP_NOMOVE | WinApi.SWP_NOSIZE | WinApi.SWP_NOACTIVATE);
            }
        }

        private void _igTool_ToolClosingRequest(object? sender, DisconnectedEventArgs e)
        {
            if (this.InvokeRequired) this.Invoke(new MethodInvoker(this.Close));
            else this.Close();
        }

        private void _igTool_ToolMessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            if (e == null || string.IsNullOrEmpty(e.MessageData)) return;

            if (e.MessageName == ImageGlassEvents.IMAGE_LOADED) 
            {
                this.Invoke((MethodInvoker)delegate
                {
                    try
                    {
                        var args = IgImageLoadedEventArgs.Deserialize(e.MessageData);
                        if (args != null && !string.IsNullOrWhiteSpace(args.FilePath))
                        {
                            _imagePathToAdd = args.FilePath;
                            PopulateDynamicAddButtons();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Error sync image: {ex.Message}");
                    }
                });
            }
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _zOrderTimer.Stop();
            _zOrderTimer.Dispose();

            if (_igTool != null)
            {
                _igTool.ToolClosingRequest -= _igTool_ToolClosingRequest;
                _igTool.ToolMessageReceived -= _igTool_ToolMessageReceived;
                _igTool.Dispose();
            }
        }

        private void LogMessage(string message)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(new MethodInvoker(() => LogMessage(message)));
            }
            else
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                rtbLog.AppendText($"[{timestamp}] - {message}\n");
                rtbLog.ScrollToCaret();
            }
        }

        private void RefreshCategoryList()
        {
            lstCategories.Items.Clear();
            foreach (var category in DataManager.Categories.OrderBy(c => c.Name))
            {
                string displayText = category.ImagePaths.Count > 0 
                    ? $"{category.Name} - {category.ImagePaths.Count}" 
                    : category.Name;
                lstCategories.Items.Add(displayText);
            }
            PopulateDynamicAddButtons();
        }

        private Category? GetSelectedCategory()
        {
            if (lstCategories.SelectedItem == null)
            {
                LogMessage("Warning: Please select a category first.");
                return null;
            }

            string selectedText = lstCategories.SelectedItem.ToString() ?? "";
            string categoryName = selectedText;
            if (selectedText.Contains(" - "))
            {
                categoryName = selectedText.Substring(0, selectedText.LastIndexOf(" - "));
            }
            return DataManager.Categories.FirstOrDefault(c => c.Name == categoryName);
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            string name = txtNewCategory.Text.Trim();
            if (string.IsNullOrEmpty(name)) return;

            if (DataManager.Categories.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                LogMessage($"Warning: '{name}' already exists.");
                return;
            }

            DataManager.Categories.Add(new Category { Name = name });
            DataManager.Save();
            RefreshCategoryList();
            
            for (int i = 0; i < lstCategories.Items.Count; i++)
            {
                if (lstCategories.Items[i].ToString()?.StartsWith(name) == true)
                {
                    lstCategories.SelectedIndex = i;
                    break;
                }
            }

            txtNewCategory.Clear();
            AdjustFormHeight();
        }

        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            var category = GetSelectedCategory();
            if (category == null) return;

            var result = MessageBox.Show($"Delete category '{category.Name}'?", "Confirm", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                DataManager.Categories.Remove(category);
                DataManager.Save();
                RefreshCategoryList();
            }
            AdjustFormHeight();
        }
        
        private void btnCopy_Click(object sender, EventArgs e) => BatchProcess(false);
        private void btnMove_Click(object sender, EventArgs e) => BatchProcess(true);

        private void BatchProcess(bool move)
        {
            var category = GetSelectedCategory();
            if (category == null || category.ImagePaths.Count == 0) return;

            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                int success = 0;
                var remaining = new List<string>();

                foreach (string src in category.ImagePaths)
                {
                    if (File.Exists(src))
                    {
                        try
                        {
                            string dest = GetUniqueDestinationPath(Path.Combine(fbd.SelectedPath, Path.GetFileName(src)));
                            if (move) File.Move(src, dest);
                            else File.Copy(src, dest);
                            success++;
                        }
                        catch (Exception ex)
                        {
                            remaining.Add(src);
                            LogMessage($"Error processing {Path.GetFileName(src)}: {ex.Message}");
                        }
                    }
                }

                if (move) category.ImagePaths = remaining;
                else if (success > 0) category.ImagePaths.Clear();

                DataManager.Save();
                RefreshCategoryList();
                LogMessage($"{(move ? "Moved" : "Copied")} {success} files.");
            }
        }

        private void btnPrev_Click(object sender, EventArgs e) => Navigate(-1);
        private void btnNext_Click(object sender, EventArgs e) => Navigate(1);

        private void Navigate(int direction)
        {
            WinApi.ImageGlassControl.SendImageGlassKey(direction > 0 ? Keys.Right : Keys.Left);
            Task.Delay(150).ContinueWith(_ => 
            {
                if (!this.IsDisposed) this.Invoke(new Action(RequestCurrentImage));
            });
        }

        private void PopulateDynamicAddButtons()
        {
            pnlDynamicAddButtons.Controls.Clear();

            if (_imagePathToAdd == null)
            {
                pnlDynamicAddButtons.Controls.Add(new Label { Text = "No image loaded.", AutoSize = true });
                AdjustFormHeight();
                return;
            }

            int width = Math.Max(240, pnlDynamicAddButtons.ClientSize.Width - 20);

            foreach (var category in DataManager.Categories.OrderBy(c => c.Name))
            {
                var btn = new Button
                {
                    Text = category.ImagePaths.Count > 0 ? $"Add to {category.Name} - {category.ImagePaths.Count}" : $"Add to {category.Name}",
                    Tag = category.Name,
                    Width = width,
                    Height = 40,
                    Margin = new Padding(5)
                };
                btn.Click += DynamicAddButton_Click;
                pnlDynamicAddButtons.Controls.Add(btn);
            }
            AdjustFormHeight();
        }

        private void DynamicAddButton_Click(object? sender, EventArgs e)
        {
            if (_imagePathToAdd == null || sender is not Button btn || btn.Tag is not string categoryName) return;

            var category = DataManager.Categories.FirstOrDefault(c => c.Name == categoryName);
            if (category == null) return;

            if (category.ImagePaths.Contains(_imagePathToAdd))
            {
                LogMessage($"Already in '{categoryName}'.");
            }
            else
            {
                category.ImagePaths.Add(_imagePathToAdd);
                DataManager.Save();
                LogMessage($"Added '{Path.GetFileName(_imagePathToAdd)}' to '{categoryName}'.");
                RefreshCategoryList();
                Navigate(1); // Auto next
            }
        }

        private void tabControlMain_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabPageImageOperations) PopulateDynamicAddButtons();
            AdjustFormHeight();
        }

        private string GetUniqueDestinationPath(string path)
        {
            if (!File.Exists(path)) return path;
            string dir = Path.GetDirectoryName(path) ?? "";
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);
            int i = 1;
            while (File.Exists(Path.Combine(dir, $"{name} ({i}){ext}"))) i++;
            return Path.Combine(dir, $"{name} ({i}){ext}");
        }

        private void AdjustFormHeight()
        {
            if (this.IsDisposed) return;
            this.SuspendLayout();

            var screen = Screen.FromControl(this);
            int maxH = (int)(screen.WorkingArea.Height * 0.9);
            int overhead = (this.Height - this.ClientSize.Height) + tabControlMain.Top + rtbLog.Height + 40;

            int tabH = 0;
            if (tabControlMain.SelectedTab == tabPageImageOperations)
            {
                int btnH = pnlDynamicAddButtons.Controls.Cast<Control>().Sum(c => c.Height + c.Margin.Vertical) + 20;
                pnlDynamicAddButtons.Height = Math.Min(btnH, maxH - overhead - 100);
                tabH = pnlDynamicAddButtons.Bottom + 10;
            }
            else
            {
                tabH = grpCategoryActions.Bottom + 10;
            }

            int headerH = Math.Max(24, tabControlMain.Height - tabControlMain.DisplayRectangle.Height);
            tabControlMain.Height = Math.Min(tabH + headerH, maxH - overhead);
            rtbLog.Location = new Point(rtbLog.Location.X, tabControlMain.Bottom + 5);
            this.ClientSize = new Size(this.ClientSize.Width, rtbLog.Bottom + 10);

            this.ResumeLayout();
        }
    }
}

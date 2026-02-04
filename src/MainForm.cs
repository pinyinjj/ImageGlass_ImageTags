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
    /// Main window for image tagging and tag management.
    /// Integrated with ImageGlass via official SDK and WinApi.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly string[] _args;
        private string? _imagePathToAdd = null;
        private ImageGlassTool? _igTool;
        private readonly System.Windows.Forms.Timer _zOrderTimer;
        private readonly UndoManager _undoManager;
        private string? _currentDirectory = null;
        private bool _hasSeenFirstImage = false;

        public MainForm(string[] args)
        {
            InitializeComponent();
            _args = args;
            this.FormClosing += MainForm_FormClosing;

            _undoManager = new UndoManager(10);
            _undoManager.StateChanged += (s, e) => { if (btnUndo != null) btnUndo.Enabled = _undoManager.CanUndo; };

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
            RefreshTagList();
            InitializeTagContextMenu();
            
            // Hide old controls
            btnDeleteTag.Visible = false;
            txtNewTag.Visible = false;
            btnAddTag.Visible = false;

            // Reposition GroupBox to fill gap from hidden controls
            grpTagActions.Top = lstTags.Bottom + 10;

            // Initialize new UI
            InitializeHeaderButtons();

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
                    tabControlMain.SelectedTab = tabPageTagging;
                    PopulateDynamicAddButtons();
                }
            }
            else
            {
                 tabControlMain.SelectedTab = tabPageTags;
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
                            CheckForFirstImage(args.FilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Error sync image: {ex.Message}");
                    }
                });
            }
        }

        private async void CheckForFirstImage(string currentImagePath)
        {
            try
            {
                string? dir = Path.GetDirectoryName(currentImagePath);
                if (!string.Equals(dir, _currentDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    _currentDirectory = dir;
                    _hasSeenFirstImage = false;
                }

                // Give ImageGlass a moment to update its window title
                await Task.Delay(200);

                IntPtr hwnd = WinApi.ImageGlassControl.FindImageGlassWindow();
                if (hwnd == IntPtr.Zero) return;

                int len = WinApi.GetWindowTextLength(hwnd);
                if (len <= 0) return;

                StringBuilder sb = new StringBuilder(len + 1);
                WinApi.GetWindowText(hwnd, sb, sb.Capacity);
                string title = sb.ToString();

                // Regex matches "1/N" or " 1 / N " patterns (e.g. "image.jpg - 1/50 - ImageGlass")
                if (System.Text.RegularExpressions.Regex.IsMatch(title, @"\b1\s*/\s*\d+\b"))
                {
                    if (_hasSeenFirstImage)
                    {
                        // Use TopMost MessageBox to ensure it's seen over ImageGlass
                        MessageBox.Show(new Form { TopMost = true }, "已到第一张图片", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        _hasSeenFirstImage = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error checking first image: {ex.Message}");
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

        private void RefreshTagList()
        {
            lstTags.Items.Clear();
            foreach (var tag in DataManager.Tags.OrderBy(c => c.Name))
            {
                string displayText = tag.ImagePaths.Count > 0 
                    ? $"{tag.Name} - {tag.ImagePaths.Count}" 
                    : tag.Name;
                lstTags.Items.Add(displayText);
            }
            PopulateDynamicAddButtons();
        }

        private Tag? GetSelectedTag()
        {
            if (lstTags.SelectedItem == null)
            {
                LogMessage("Warning: Please select a tag first.");
                return null;
            }

            string selectedText = lstTags.SelectedItem.ToString() ?? "";
            string tagName = selectedText;
            if (selectedText.Contains(" - "))
            {
                tagName = selectedText.Substring(0, selectedText.LastIndexOf(" - "));
            }
            return DataManager.Tags.FirstOrDefault(c => c.Name == tagName);
        }

        private void btnAddTag_Click(object sender, EventArgs e)
        {
            string name = txtNewTag.Text.Trim();
            if (string.IsNullOrEmpty(name)) return;

            if (DataManager.Tags.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                LogMessage($"Warning: '{name}' already exists.");
                return;
            }

            DataManager.Tags.Add(new Tag { Name = name });
            DataManager.Save();
            RefreshTagList();
            
            for (int i = 0; i < lstTags.Items.Count; i++)
            {
                if (lstTags.Items[i].ToString()?.StartsWith(name) == true)
                {
                    lstTags.SelectedIndex = i;
                    break;
                }
            }

            txtNewTag.Clear();
            AdjustFormHeight();
        }

        private void ItemDelete_Click(object? sender, EventArgs e)
        {
            DeleteSelectedTag();
        }

        private void DeleteSelectedTag()
        {
            var tag = GetSelectedTag();
            if (tag == null) return;

            var result = MessageBox.Show($"Delete tag '{tag.Name}'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                DataManager.Tags.Remove(tag);
                DataManager.Save();
                RefreshTagList();
                LogMessage($"Deleted tag '{tag.Name}'.");
            }
            AdjustFormHeight();
        }

        private void btnDeleteTag_Click(object sender, EventArgs e)
        {
            DeleteSelectedTag();
        }
        
        private void btnCopy_Click(object sender, EventArgs e) => BatchProcess(false);
        private void btnMove_Click(object sender, EventArgs e) => BatchProcess(true);

        private void BatchProcess(bool move)
        {
            var tag = GetSelectedTag();
            if (tag == null || tag.ImagePaths.Count == 0) return;

            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                int success = 0;
                var remaining = new List<string>();

                foreach (string src in tag.ImagePaths)
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

                if (move) tag.ImagePaths = remaining;

                DataManager.Save();
                RefreshTagList();
                LogMessage($"{(move ? "Moved" : "Copied")} {success} files.");
            }
        }

        private void btnPrev_Click(object sender, EventArgs e) => Navigate(-1);
        private void btnNext_Click(object sender, EventArgs e) => Navigate(1);
        private void btnUndo_Click(object sender, EventArgs e) => _undoManager.Undo();

        private void Navigate(int direction, bool recordUndo = true)
        {
            if (recordUndo)
            {
                _undoManager.Push(new NavigationCommand(this, direction));
            }

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

            foreach (var tag in DataManager.Tags.OrderBy(c => c.Name))
            {
                var btn = new Button
                {
                    Text = tag.ImagePaths.Count > 0 ? $"Add to {tag.Name} - {tag.ImagePaths.Count}" : $"Add to {tag.Name}",
                    Tag = tag.Name,
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
            if (_imagePathToAdd == null || sender is not Button btn || btn.Tag is not string tagName) return;

            var tag = DataManager.Tags.FirstOrDefault(c => c.Name == tagName);
            if (tag == null) return;

            if (tag.ImagePaths.Contains(_imagePathToAdd))
            {
                LogMessage($"Already in '{tagName}'.");
            }
            else
            {
                tag.ImagePaths.Add(_imagePathToAdd);
                DataManager.Save();
                
                _undoManager.Push(new AddTagCommand(this, tagName, _imagePathToAdd, true));

                LogMessage($"Added '{Path.GetFileName(_imagePathToAdd)}' to '{tagName}'.");
                RefreshTagList();
                Navigate(1, false); // Auto next, don't record separate navigation
            }
        }

        private void tabControlMain_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabPageTagging) PopulateDynamicAddButtons();
            AdjustFormHeight();
        }

        private void tabControlMain_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            TabPage page = tabControlMain.TabPages[e.Index];
            Rectangle bounds = tabControlMain.GetTabRect(e.Index);

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            // Background - use a clean white for selected, and a light gray for unselected
            Color backColor = isSelected ? Color.White : Color.FromArgb(245, 245, 245);
            using (Brush backBrush = new SolidBrush(backColor))
            {
                g.FillRectangle(backBrush, bounds);
            }

            // Selection indicator (bottom line)
            if (isSelected)
            {
                // Use a standard Windows accent color
                using (Brush accentBrush = new SolidBrush(Color.FromArgb(0, 120, 212)))
                {
                    g.FillRectangle(accentBrush, new Rectangle(bounds.X, bounds.Bottom - 3, bounds.Width, 3));
                }
            }

            // Text rendering
            TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine;
            Color textColor = isSelected ? Color.Black : Color.FromArgb(120, 120, 120);
            
            // Slightly bolder font for selected tab
            using (Font tabFont = new Font(tabControlMain.Font.FontFamily, 9.5f, isSelected ? FontStyle.Bold : FontStyle.Regular))
            {
                TextRenderer.DrawText(g, page.Text, tabFont, bounds, textColor, flags);
            }
        }

        private string GetUniqueDestinationPath(string path)
        {
            if (!File.Exists(path)) return path;
            string dir = Path.GetDirectoryName(path) ?? "";
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);
            int i = 1;
            while (File.Exists(Path.Combine(dir, $"{name} ({i}){ext}")))
            {
                i++;
            }
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
            if (tabControlMain.SelectedTab == tabPageTagging)
            {
                int btnH = pnlDynamicAddButtons.Controls.Cast<Control>().Sum(c => c.Height + c.Margin.Vertical) + 20;
                pnlDynamicAddButtons.Height = Math.Min(btnH, maxH - overhead - 100);
                tabH = pnlDynamicAddButtons.Bottom + 10;
            }
            else
            {
                tabH = grpTagActions.Bottom + 10;
            }

            int headerH = Math.Max(24, tabControlMain.Height - tabControlMain.DisplayRectangle.Height);
            tabControlMain.Height = Math.Min(tabH + headerH, maxH - overhead);
            rtbLog.Location = new Point(rtbLog.Location.X, tabControlMain.Bottom + 5);
            this.ClientSize = new Size(this.ClientSize.Width, rtbLog.Bottom + 10);

            this.ResumeLayout();
        }

        private void InitializeTagContextMenu()
        {
            var ctxMenu = new ContextMenuStrip();
            
            var itemClear = new ToolStripMenuItem("Clear");
            itemClear.Click += ItemClear_Click;
            
            var itemDuplicate = new ToolStripMenuItem("Duplicate");
            itemDuplicate.Click += ItemCopy_Click;

            var itemDelete = new ToolStripMenuItem("Delete");
            itemDelete.Click += ItemDelete_Click;

            ctxMenu.Items.Add(itemClear);
            ctxMenu.Items.Add(itemDuplicate);
            ctxMenu.Items.Add(new ToolStripSeparator());
            ctxMenu.Items.Add(itemDelete);

            lstTags.ContextMenuStrip = ctxMenu;
            lstTags.MouseDown += LstCategories_MouseDown;
        }

        private void InitializeHeaderButtons()
        {
            // Calculate positions based on the list box or tab page
            int btnSize = 25;
            int padding = 5;
            int rightEdge = lstTags.Right;
            int topPos = lstTags.Top - btnSize - 2;

            var btnMinus = new Button
            {
                Text = "-",
                Size = new Size(btnSize, btnSize),
                Location = new Point(rightEdge - btnSize, topPos),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Parent = tabPageTags,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 0, 0, 3), // Bottom padding pushes text up
                UseCompatibleTextRendering = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point)
            };
            btnMinus.Click += (s, e) => DeleteSelectedTag();

            var btnPlus = new Button
            {
                Text = "+",
                Size = new Size(btnSize, btnSize),
                Location = new Point(rightEdge - btnSize * 2 - padding, topPos),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Parent = tabPageTags,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 0, 0, 3),
                UseCompatibleTextRendering = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point)
            };
            btnPlus.Click += BtnPlus_Click;
            
            // Ensure they are on top
            btnMinus.BringToFront();
            btnPlus.BringToFront();
        }

                        private void BtnPlus_Click(object? sender, EventArgs e)
                        {
                            _zOrderTimer.Stop();
                            try
                            {
                                using var form = new Form
                                {
                                    Text = "Add Tag",
                                    ClientSize = new Size(350, 140),
                                    StartPosition = FormStartPosition.CenterParent,
                                    FormBorderStyle = FormBorderStyle.FixedDialog,
                                    MaximizeBox = false,
                                    MinimizeBox = false,
                                    ShowIcon = false,
                                    ShowInTaskbar = false,
                                    ControlBox = true,
                                    AutoScaleMode = AutoScaleMode.Font,
                                    Font = new Font("Segoe UI", 9F),
                                    TopMost = true
                                };
                
                                var lbl = new Label 
                                { 
                                    Text = "Enter new tag name:", 
                                    Left = 20, 
                                    Top = 20, 
                                    AutoSize = true,
                                    TabIndex = 2
                                };
                                
                                var txt = new TextBox 
                                { 
                                    Left = 20, 
                                    Top = 45, 
                                    Width = 310, 
                                    TabIndex = 0 
                                };
                                
                                var btnAdd = new Button 
                                { 
                                    Text = "Add", 
                                    Left = 170, 
                                    Top = 90,
                                    Width = 75, 
                                    Height = 30,
                                    DialogResult = DialogResult.OK,
                                    UseVisualStyleBackColor = true,
                                    TabIndex = 1
                                };
                                
                                var btnCancel = new Button 
                                { 
                                    Text = "Cancel", 
                                    Left = 255, 
                                    Top = 90, 
                                    Width = 75, 
                                    Height = 30, 
                                    DialogResult = DialogResult.Cancel,
                                    UseVisualStyleBackColor = true,
                                    TabIndex = 3
                                };
                
                                form.Controls.AddRange(new Control[] { lbl, txt, btnAdd, btnCancel });
                                form.AcceptButton = btnAdd;
                                form.CancelButton = btnCancel;
                
                                if (form.ShowDialog(this) == DialogResult.OK)
                                {
                                    string name = txt.Text.Trim();
                                    if (!string.IsNullOrEmpty(name))
                                    {
                                       CreateTag(name);
                                    }
                                }
                            }
                            finally
                            {
                                _zOrderTimer.Start();
                            }
                        }        private void CreateTag(string name)
        {
             if (DataManager.Tags.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                LogMessage($"Warning: '{name}' already exists.");
                return;
            }

            DataManager.Tags.Add(new Tag { Name = name });
            DataManager.Save();
            RefreshTagList();
            
            for (int i = 0; i < lstTags.Items.Count; i++)
            {
                if (lstTags.Items[i].ToString()?.StartsWith(name) == true)
                {
                    lstTags.SelectedIndex = i;
                    break;
                }
            }
            AdjustFormHeight();
            LogMessage($"Added tag '{name}'.");
        }

        private void LstCategories_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = lstTags.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    lstTags.SelectedIndex = index;
                }
            }
        }

        private void ItemClear_Click(object? sender, EventArgs e)
        {
            var tag = GetSelectedTag();
            if (tag == null) return;

            if (MessageBox.Show($"Are you sure you want to clear the list for '{tag.Name}'?", "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                tag.ImagePaths.Clear();
                DataManager.Save();
                RefreshTagList();
                LogMessage($"Cleared list for tag '{tag.Name}'.");
            }
        }

        private void ItemCopy_Click(object? sender, EventArgs e)
        {
            var tag = GetSelectedTag();
            if (tag == null) return;

            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string newName = $"{tag.Name}_{timestamp}";

            // Ensure unique name if somehow it already exists (unlikely with seconds precision but good practice)
            if (DataManager.Tags.Any(c => c.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)))
            {
                newName += $"_{Guid.NewGuid().ToString().Substring(0, 4)}";
            }

            var newTag = new Tag
            {
                Name = newName,
                ImagePaths = new List<string>(tag.ImagePaths)
            };

            DataManager.Tags.Add(newTag);
            DataManager.Save();
            RefreshTagList();
            
            LogMessage($"Copied tag '{tag.Name}' to '{newName}'.");
            
                        // Optionally select the new tag
            
                         for (int i = 0; i < lstTags.Items.Count; i++)
            
                        {
            
                            if (lstTags.Items[i].ToString()?.StartsWith(newName) == true)
            
                            {
            
                                lstTags.SelectedIndex = i;
            
                                break;
            
                            }
            
                        }
            
                    }
            
            
            
                    private class NavigationCommand : IUndoCommand
            
                    {
            
                        private readonly MainForm _form;
            
                        private readonly int _direction;
            
                        public string Description => "Navigation";
            
            
            
                        public NavigationCommand(MainForm form, int direction)
            
                        {
            
                            _form = form;
            
                            _direction = direction;
            
                        }
            
            
            
                        public void Undo()
            
                        {
            
                            _form.Navigate(-_direction, false);
            
                        }
            
                    }
            
            
            
                    private class AddTagCommand : IUndoCommand
            
                    {
            
                        private readonly MainForm _form;
            
                        private readonly string _tagName;
            
                        private readonly string _imagePath;
            
                        private readonly bool _autoNavigated;
            
                        public string Description => $"Add to {_tagName}";
            
            
            
                        public AddTagCommand(MainForm form, string tagName, string imagePath, bool autoNavigated)
            
                        {
            
                            _form = form;
            
                            _tagName = tagName;
            
                            _imagePath = imagePath;
            
                            _autoNavigated = autoNavigated;
            
                        }
            
            
            
                        public void Undo()
            
                        {
            
                            if (_autoNavigated)
            
                            {
            
                                _form.Navigate(-1, false);
            
                            }
            
            
            
                            var tag = DataManager.Tags.FirstOrDefault(t => t.Name == _tagName);
            
                            if (tag != null)
            
                            {
            
                                if (tag.ImagePaths.Remove(_imagePath))
            
                                {
            
                                    DataManager.Save();
            
                                    _form.RefreshTagList();
            
                                    _form.LogMessage($"Undid add to '{_tagName}'");
            
                                }
            
                            }
            
                        }
            
                    }
            
                }
            
            }
            
            
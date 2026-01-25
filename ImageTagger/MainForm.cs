using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; // Explicitly use this for Timer
using System.IO;
using ImageGlass.Tools; // Added for ImageGlass.Tools SDK
using System.Text.Json; // Added for JSON serialization/deserialization

namespace ImageTagger
{
    public partial class MainForm : Form
    {
        private string[] _args;
        private string? _imagePathToAdd = null; // Will now store only the FilePath string
        private ImageGlassTool? _igTool; // Declared ImageGlassTool instance
        private System.Windows.Forms.Timer _topMostTimer; // Explicitly use Forms.Timer

        public MainForm(string[] args)
        {
            InitializeComponent();
            _args = args;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing); // Ensure FormClosing is subscribed

            // Initialize and start the TopMost enforcement timer
            _topMostTimer = new System.Windows.Forms.Timer(); // Explicitly instantiate Forms.Timer
            _topMostTimer.Interval = 500; // Check every 500ms
            _topMostTimer.Tick += TopMostTimer_Tick;
            _topMostTimer.Start();

            // Immediately attempt to set TopMost when the form is created
            SetWindowAlwaysOnTop();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            
            // Initialize ImageGlassTool only when the window handle is available
            if (_igTool == null)
            {
                _igTool = new ImageGlassTool();
                _igTool.ToolClosingRequest += _igTool_ToolClosingRequest;
                _igTool.ToolMessageReceived += _igTool_ToolMessageReceived;
                LogMessage("Debug: ImageGlassTool initialized and connected.");
                
                // Initial connect
                _igTool.ConnectAsync();

                // Request current image info on startup
                RequestCurrentImage();
            }
        }

        private async void RequestCurrentImage()
        {
            if (_igTool == null) return;

            try
            {
                // ImageGlassTool uses an internal PipeClient named _client.
                // We use reflection to access it and send a request for the current image.
                var clientField = typeof(ImageGlassTool).GetField("_client", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var client = clientField?.GetValue(_igTool);
                if (client != null)
                {
                    var sendAsyncMethod = client.GetType().GetMethod("SendAsync", new[] { typeof(string) });
                    if (sendAsyncMethod != null)
                    {
                        // In ImageGlass 9, sending a message can trigger ImageGlass to re-send the IMAGE_LOADED event.
                        // The format expected by PipeClient is "MessageName{: +IG_TOOL+:}MessageData"
                        string msgSeparator = (string)typeof(ImageGlassTool).GetProperty("MSG_SEPARATOR", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).GetValue(null);
                        string message = $"igtool.request.get_image{msgSeparator}";
                        
                        var task = (Task)sendAsyncMethod.Invoke(client, new object[] { message });
                        await task;
                        LogMessage("Debug: Requested current image info from ImageGlass.");
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Debug: Failed to request image via reflection: {ex.Message}");
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            DataManager.Load();
            RefreshCategoryList();

            // Check if an image path was passed as an argument
            if (_args.Length > 0 && !string.IsNullOrWhiteSpace(_args[0]))
            {
                // Attempt to parse the argument as IgImageLoadedEventArgs JSON
                try
                {
                    var args = IgImageLoadedEventArgs.Deserialize(_args[0]);
                    _imagePathToAdd = args?.FilePath;
                }
                catch
                {
                    // If not valid JSON, treat it as a plain file path (fallback)
                    _imagePathToAdd = _args[0];
                }

                if (!string.IsNullOrWhiteSpace(_imagePathToAdd) && File.Exists(_imagePathToAdd))
                {
                    // Switch to Image Operations tab and populate buttons
                    tabControlMain.SelectedTab = tabPageImageOperations;
                    PopulateDynamicAddButtons();
                    LogMessage($"Launched with initial image: {Path.GetFileName(_imagePathToAdd)}");
                }
                else if (!string.IsNullOrWhiteSpace(_imagePathToAdd))
                {
                    LogMessage($"Launched with initial image: '{Path.GetFileName(_imagePathToAdd)}', but file not found.");
                }
                else
                {
                    LogMessage("Launched with invalid initial image data.");
                }
            } else {
                 // If not launched with image, default to Category Management tab
                 tabControlMain.SelectedTab = tabPageCategories;
                 LogMessage("Launched without initial image. Ready for operations.");
            }

            // Perform initial adjustment of form height
            AdjustFormHeight();
        }
        
        // Removed MainForm_Shown event handler
        // private async void MainForm_Shown(object sender, EventArgs e) { ... }

        // Timer Tick event handler to continuously enforce TopMost
        private void TopMostTimer_Tick(object? sender, EventArgs e)
        {
            SetWindowAlwaysOnTop();
        }

        // Helper method to set the window always on top
        private void SetWindowAlwaysOnTop()
        {
            if (this.IsHandleCreated && !this.Disposing && !this.IsDisposed)
            {
                // Use SWP_NOACTIVATE to prevent stealing focus from ImageGlass during enforcement
                WinApi.SetWindowPos(this.Handle, WinApi.HWND_TOPMOST, 0, 0, 0, 0, WinApi.SWP_NOMOVE | WinApi.SWP_NOSIZE | WinApi.SWP_NOACTIVATE);
            }
        }

        // Handle ImageGlass closing request
        private void _igTool_ToolClosingRequest(object? sender, DisconnectedEventArgs e)
        {
            LogMessage("Received ImageGlass closing request. Shutting down.");
            // ImageGlass is closing, so close this tool too.
            // Using Invoke to ensure thread safety as this event might come from a different thread.
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(this.Close));
            }
            else
            {
                this.Close();
            }
        }

        // Handle ImageGlass messages (e.g., image changed)
        private void _igTool_ToolMessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            if (e == null) return;

            // Log every message name for diagnostics
            LogMessage($"[Debug] Message from ImageGlass: {e.MessageName}");

            if (string.IsNullOrEmpty(e.MessageData)) 
            {
                return;
            }

            // Handle Image Loaded event using the official SDK class
            if (e.MessageName == ImageGlassEvents.IMAGE_LOADED) 
            {
                this.Invoke((MethodInvoker)delegate
                {
                    try
                    {
                        // Use the Deserialize method provided by the SDK class
                        var args = IgImageLoadedEventArgs.Deserialize(e.MessageData);
                        if (args != null && !string.IsNullOrWhiteSpace(args.FilePath))
                        {
                            _imagePathToAdd = args.FilePath;
                            LogMessage($"ImageGlass loaded new image: {Path.GetFileName(_imagePathToAdd)}");
                            PopulateDynamicAddButtons();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Error parsing IMAGE_LOADED data: {ex.Message}");
                    }
                });
            }
            // Also handle Image Loading event if needed
            else if (e.MessageName == ImageGlassEvents.IMAGE_LOADING)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    try
                    {
                        var args = IgImageLoadingEventArgs.Deserialize(e.MessageData);
                        // We can log that a new image is being loaded
                        if (args != null)
                        {
                            LogMessage($"[Debug] Loading image at index: {args.NewIndex}");
                        }
                    }
                    catch { /* Ignore parsing errors for loading event */ }
                });
            }
        }

        // Handle form closing for disposal and event unsubscription
        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _topMostTimer.Stop(); // Stop the timer when the form is closing
            _topMostTimer.Dispose(); // Dispose the timer

            // Prompt user to clear all image lists (not categories) on exit
            DialogResult result = MessageBox.Show(
                "Do you want to clear all image lists (keeping categories) before exiting?",
                "Confirm Clear Image Lists on Exit",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Clear image paths from all categories, but keep categories
                foreach (var category in DataManager.Categories)
                {
                    category.ImagePaths.Clear();
                }
                DataManager.Save();
                LogMessage("All image lists cleared on exit, categories retained.");
            }
            else if (result == DialogResult.Cancel)
            {
                e.Cancel = true; // Cancel closing if user selects Cancel
                return;
            }

            if (_igTool != null)
            {
                _igTool.ToolClosingRequest -= _igTool_ToolClosingRequest; // Unsubscribe
                _igTool.ToolMessageReceived -= _igTool_ToolMessageReceived; // Unsubscribe
                _igTool.Dispose(); // Dispose the ImageGlassTool instance
            }
            LogMessage("Image Tagger is closing.");
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
                rtbLog.ScrollToCaret(); // Auto-scroll to the bottom
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
            PopulateDynamicAddButtons(); // Refresh dynamic buttons whenever category list changes
        }

        private Category? GetSelectedCategory()
        {
            // This method is now only relevant for the Category Management tab, using lstCategories
            if (lstCategories.SelectedItem == null)
            {
                LogMessage("Warning: Please select a category first.");
                return null;
            }
            // Extract original category name (strip " - Count" if present)
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
            string newCategoryName = txtNewCategory.Text.Trim();
            if (string.IsNullOrEmpty(newCategoryName))
            {
                LogMessage("Warning: Please enter a category name.");
                return;
            }

            if (DataManager.Categories.Any(c => c.Name.Equals(newCategoryName, StringComparison.OrdinalIgnoreCase)))
            {
                LogMessage($"Warning: Category '{newCategoryName}' already exists.");
                return;
            }

            var newCategory = new Category { Name = newCategoryName };
            DataManager.Categories.Add(newCategory);
            DataManager.Save();
            RefreshCategoryList(); // This also calls PopulateDynamicAddButtons()
            LogMessage($"Category '{newCategoryName}' added successfully.");

            // Select the newly added category - note that RefreshCategoryList might have added " - 0"
            // So we find the item that starts with the name
            for (int i = 0; i < lstCategories.Items.Count; i++)
            {
                string itemText = lstCategories.Items[i].ToString() ?? "";
                if (itemText == newCategoryName || itemText.StartsWith(newCategoryName + " - "))
                {
                    lstCategories.SelectedIndex = i;
                    break;
                }
            }

            txtNewCategory.Clear();
            AdjustFormHeight(); // Adjust height after adding new category
        }

        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            var categoryToRemove = GetSelectedCategory();
            if (categoryToRemove == null) return;

            DialogResult result = MessageBox.Show($"Are you sure you want to delete the '{categoryToRemove.Name}' category? This will not delete the image files themselves.", "Confirm Delete Category", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                DataManager.Categories.Remove(categoryToRemove);
                DataManager.Save();
                RefreshCategoryList(); // This also calls PopulateDynamicAddButtons()
                LogMessage($"Category '{categoryToRemove.Name}' deleted successfully.");
            } else {
                LogMessage($"Deletion of category '{categoryToRemove.Name}' cancelled.");
            }
            AdjustFormHeight(); // Adjust height after deleting category
        }
        
        private void btnCopy_Click(object sender, EventArgs e)
        {
            var category = GetSelectedCategory();
            if (category == null || category.ImagePaths.Count == 0)
            {
                LogMessage("Warning: No category selected or category is empty for copy operation.");
                return;
            }

            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select a folder to copy the images to.";
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    int successCount = 0;
                    int failCount = 0;
                    var copiedPaths = new List<string>(category.ImagePaths); // Keep original list for tracking what was attempted

                    foreach (string sourcePath in copiedPaths) // Iterate over a copy to allow modification of original list
                    {
                        if (File.Exists(sourcePath))
                        {
                            string destFileName = Path.GetFileName(sourcePath);
                            string desiredDestPath = Path.Combine(fbd.SelectedPath, destFileName);
                            string uniqueDestPath = GetUniqueDestinationPath(desiredDestPath);

                            try
                            {
                                if (desiredDestPath != uniqueDestPath)
                                {
                                    LogMessage($"Detected duplicate: '{destFileName}'. Renaming to '{Path.GetFileName(uniqueDestPath)}'.");
                                }
                                File.Copy(sourcePath, uniqueDestPath, false); // false: do not overwrite
                                successCount++;
                                LogMessage($"Copied '{Path.GetFileName(sourcePath)}' to '{Path.GetFileName(uniqueDestPath)}' in '{fbd.SelectedPath}'.");
                            }
                            catch (Exception ex) 
                            { 
                                failCount++; 
                                LogMessage($"Error copying '{Path.GetFileName(sourcePath)}': {ex.Message}");
                            }
                        }
                        else 
                        { 
                            failCount++; 
                            LogMessage($"Error: Source file '{Path.GetFileName(sourcePath)}' not found for copy.");
                        }
                    }

                    if (successCount > 0) // If any files were successfully copied, clear the source list
                    {
                        category.ImagePaths.Clear(); // Clear all original paths from the category list
                        DataManager.Save();
                        RefreshCategoryList(); // This also calls PopulateDynamicAddButtons() and updates counts
                        LogMessage($"Successfully copied {successCount} files. Source category list cleared.");
                    } else {
                        LogMessage($"Copy operation finished: {successCount} files copied, {failCount} failed. Source category list not cleared due to no successful copies.");
                    }
                } else {
                    LogMessage("Copy operation cancelled by user.");
                }
            }
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            var category = GetSelectedCategory();
            if (category == null || category.ImagePaths.Count == 0)
            {
                LogMessage("Warning: No category selected or category is empty for move operation.");
                return;
            }

            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select a folder to move the images to.";
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    int successCount = 0;
                    int failCount = 0;
                    var failedMovePaths = new List<string>();
                    var originalPaths = new List<string>(category.ImagePaths); // Work with a copy of the original paths

                    foreach (string sourcePath in originalPaths)
                    {
                        if (File.Exists(sourcePath))
                        {
                            string destFileName = Path.GetFileName(sourcePath);
                            string desiredDestPath = Path.Combine(fbd.SelectedPath, destFileName);
                            string uniqueDestPath = GetUniqueDestinationPath(desiredDestPath);

                            try
                            {
                                if (desiredDestPath != uniqueDestPath)
                                {
                                    LogMessage($"Detected duplicate: '{destFileName}'. Renaming to '{Path.GetFileName(uniqueDestPath)}'.");
                                }
                                File.Move(sourcePath, uniqueDestPath, false); // false: do not overwrite
                                successCount++;
                                LogMessage($"Moved '{Path.GetFileName(sourcePath)}' to '{Path.GetFileName(uniqueDestPath)}' in '{fbd.SelectedPath}'.");
                            }
                            catch (Exception ex) 
                            { 
                                failedMovePaths.Add(sourcePath); failCount++; 
                                LogMessage($"Error moving '{Path.GetFileName(sourcePath)}': {ex.Message}");
                            }
                        }
                        else 
                        { 
                            failedMovePaths.Add(sourcePath); // If file doesn't exist, it implicitly fails to move
                            failCount++; 
                            LogMessage($"Error: Source file '{Path.GetFileName(sourcePath)}' not found for move.");
                        }
                    }
                    
                    category.ImagePaths = failedMovePaths; // Update the list to only contain paths that failed to move
                    DataManager.Save();
                    RefreshCategoryList(); // This also calls PopulateDynamicAddButtons() and updates counts
                    LogMessage($"Move operation finished: {successCount} files moved, {failCount} failed/missing. Remaining items in category are failed moves.");
                } else {
                    LogMessage("Move operation cancelled by user.");
                }
            }
        }
        // Removed btnDelete_Click as requested
        // private void btnDelete_Click(object sender, EventArgs e) { ... }
        
        // Removed chkTopMost_CheckedChanged and trackOpacity_Scroll as per new request.

        private void btnPrev_Click(object sender, EventArgs e)
        {
            WinApi.ImageGlassControl.SendImageGlassKey(Keys.Left);
            LogMessage("Sent 'Left Arrow' key to ImageGlass.");
            Task.Delay(100).ContinueWith(_ => {
                if (!this.IsDisposed) {
                    this.Invoke((MethodInvoker)delegate { RequestCurrentImage(); });
                }
            });
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            WinApi.ImageGlassControl.SendImageGlassKey(Keys.Right);
            LogMessage("Sent 'Right Arrow' key to ImageGlass.");
            Task.Delay(100).ContinueWith(_ => {
                if (!this.IsDisposed) {
                    this.Invoke((MethodInvoker)delegate { RequestCurrentImage(); });
                }
            });
        }

        // Method to dynamically create "Add to Category" buttons
        private void PopulateDynamicAddButtons()
        {
            pnlDynamicAddButtons.Controls.Clear(); // Clear existing buttons

            if (_imagePathToAdd == null)
            {
                // If no image is passed, display a message or disable buttons
                Label lblNoImage = new Label() { Text = "No image loaded from ImageGlass. Please open an image in ImageGlass and click the tool button.", AutoSize = true, Margin = new Padding(5) };
                pnlDynamicAddButtons.Controls.Add(lblNoImage);
                AdjustFormHeight();
                return;
            }

            int btnWidth = pnlDynamicAddButtons.ClientSize.Width - pnlDynamicAddButtons.Padding.Horizontal - 10;
            if (btnWidth <= 50) btnWidth = 240; // Fallback to a reasonable default if width is not yet available

            foreach (var category in DataManager.Categories.OrderBy(c => c.Name))
            {
                Button btn = new Button();
                string btnText = category.ImagePaths.Count > 0 
                    ? $"Add to {category.Name} - {category.ImagePaths.Count}" 
                    : $"Add to {category.Name}";
                btn.Text = btnText;
                btn.Tag = category.Name; // Store original category name for click handler
                btn.Width = btnWidth;
                btn.Height = 40; // Fixed height
                btn.Margin = new Padding(5);
                btn.Click += DynamicAddButton_Click;
                pnlDynamicAddButtons.Controls.Add(btn);
            }
            if (DataManager.Categories.Count == 0 && _imagePathToAdd != null)
            {
                Label lblNoCategories = new Label() { Text = "No categories available. Please create some on the 'Category Management' tab.", AutoSize = true, Margin = new Padding(5) };
                pnlDynamicAddButtons.Controls.Add(lblNoCategories);
            }
            AdjustFormHeight(); // Adjust height after buttons potentially change
        }

        // Event handler for dynamically created "Add to Category" buttons
        private void DynamicAddButton_Click(object? sender, EventArgs e)
        {
            if (_imagePathToAdd == null)
            {
                LogMessage("Error: No image is currently loaded from ImageGlass for adding to category.");
                return;
            }

            Button clickedButton = sender as Button;
            if (clickedButton?.Tag is string categoryName)
            {
                var category = DataManager.Categories.FirstOrDefault(c => c.Name == categoryName);
                if (category != null)
                {
                    // Check if the current image (as a plain file path) is already in the list
                    if (category.ImagePaths.Contains(_imagePathToAdd))
                    {
                        LogMessage($"Info: Image is already in '{categoryName}'.");
                    }
                    else
                    {
                        category.ImagePaths.Add(_imagePathToAdd);
                        DataManager.Save();
                        LogMessage($"Image '{Path.GetFileName(_imagePathToAdd)}' added to '{categoryName}'.");
                        
                        // Refresh the UI to update counts
                        RefreshCategoryList();

                        WinApi.ImageGlassControl.SendImageGlassKey(Keys.Right); // Automatically switch to next image
                        LogMessage("Automatically switched to next image after successful add.");
                        
                        // Force a refresh of the image info after a short delay to allow IG to load the next image
                        Task.Delay(100).ContinueWith(_ => {
                            if (!this.IsDisposed) {
                                this.Invoke((MethodInvoker)delegate { RequestCurrentImage(); });
                            }
                        });
                    }
                }
            }
        }

        // Handle tab selection change
        private void tabControlMain_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabPageImageOperations)
            {
                PopulateDynamicAddButtons();
                LogMessage("Switched to 'Image Operations' tab.");
            } else if (tabControlMain.SelectedTab == tabPageCategories) {
                LogMessage("Switched to 'Category Management' tab.");
            }
            AdjustFormHeight(); // Adjust height after tab change
        }

        /// <summary>
        /// Generates a unique destination path by appending (1), (2), etc. to the filename if it already exists.
        /// </summary>
        /// <param name="desiredDestPath">The initially desired full destination path.</param>
        /// <returns>A unique, non-existing full destination path.</returns>
        private string GetUniqueDestinationPath(string desiredDestPath)
        {
            if (!File.Exists(desiredDestPath))
            {
                return desiredDestPath;
            }

            string directory = Path.GetDirectoryName(desiredDestPath) ?? "";
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(desiredDestPath);
            string extension = Path.GetExtension(desiredDestPath);

            int count = 1;
            string newDestPath;
            do
            {
                newDestPath = Path.Combine(directory, $"{fileNameWithoutExtension} ({count}){extension}");
                count++;
            } while (File.Exists(newDestPath));

            return newDestPath;
        }

        private void AdjustFormHeight()
        {
            if (this.IsDisposed) return;
            
            this.SuspendLayout();

            // 1. Calculate max available height for TabControl based on screen size
            var screen = Screen.FromControl(this);
            int maxFormHeight = (int)(screen.WorkingArea.Height * 0.9);
            // Overhead: borders, title bar, tabControl Top margin, rtbLog height, and bottom margins
            int overhead = (this.Height - this.ClientSize.Height) + tabControlMain.Top + rtbLog.Height + 30;
            int maxTabHeight = maxFormHeight - overhead;

            int headerHeight = tabControlMain.Height - tabControlMain.DisplayRectangle.Height;
            if (headerHeight <= 0) headerHeight = 24;

            int tabContentHeight = 0;
            if (tabControlMain.SelectedTab == tabPageImageOperations)
            {
                // Calculate required height for all buttons in FlowLayoutPanel
                int requiredButtonsHeight = 0;
                foreach (Control ctrl in pnlDynamicAddButtons.Controls)
                {
                    requiredButtonsHeight += ctrl.Height + ctrl.Margin.Vertical;
                }
                requiredButtonsHeight += pnlDynamicAddButtons.Padding.Vertical + 5;

                // Max height the panel can take before we hit screen limit
                int maxPanelHeight = maxTabHeight - headerHeight - pnlDynamicAddButtons.Top - 10;
                
                // Adjust pnlDynamicAddButtons height, capping it if necessary
                pnlDynamicAddButtons.Height = Math.Min(requiredButtonsHeight, Math.Max(100, maxPanelHeight));
                
                // Position pnlDynamicAddButtons below Prev/Next buttons area
                tabContentHeight = pnlDynamicAddButtons.Bottom + 10;
            }
            else if (tabControlMain.SelectedTab == tabPageCategories)
            {
                // For categories tab, use the bottom of the lowest control
                tabContentHeight = grpCategoryActions.Bottom + 10;
            }

            // Calculate and set TabControl required height
            int targetTabHeight = tabContentHeight + headerHeight;
            tabControlMain.Height = Math.Min(targetTabHeight, maxTabHeight);

            // Position rtbLog below TabControl
            rtbLog.Location = new Point(rtbLog.Location.X, tabControlMain.Bottom + 5);

            // Set the new form client size
            this.ClientSize = new Size(this.ClientSize.Width, rtbLog.Bottom + 10);

            this.ResumeLayout();
        }
    }
}
namespace ImageTagger
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageImageOperations = new System.Windows.Forms.TabPage();
            this.pnlDynamicAddButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.tabPageCategories = new System.Windows.Forms.TabPage();
            this.grpCategoryActions = new System.Windows.Forms.GroupBox();
            this.btnMove = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnDeleteCategory = new System.Windows.Forms.Button();
            this.btnAddCategory = new System.Windows.Forms.Button();
            this.txtNewCategory = new System.Windows.Forms.TextBox();
            this.lblCategories = new System.Windows.Forms.Label();
            this.lstCategories = new System.Windows.Forms.ListBox();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.tabControlMain.SuspendLayout();
            this.tabPageImageOperations.SuspendLayout();
            this.tabPageCategories.SuspendLayout();
            this.grpCategoryActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageImageOperations);
            this.tabControlMain.Controls.Add(this.tabPageCategories);
            this.tabControlMain.Location = new System.Drawing.Point(5, 5);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(275, 400);
            this.tabControlMain.TabIndex = 8;
            this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlMain.SelectedIndexChanged += new System.EventHandler(this.tabControlMain_SelectedIndexChanged);
            // 
            // tabPageImageOperations
            // 
            this.tabPageImageOperations.Controls.Add(this.pnlDynamicAddButtons);
            this.tabPageImageOperations.Controls.Add(this.btnNext);
            this.tabPageImageOperations.Controls.Add(this.btnPrev);
            this.tabPageImageOperations.Location = new System.Drawing.Point(4, 24);
            this.tabPageImageOperations.Name = "tabPageImageOperations";
            this.tabPageImageOperations.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageImageOperations.Size = new System.Drawing.Size(267, 372);
            this.tabPageImageOperations.TabIndex = 0;
            this.tabPageImageOperations.Text = "Image Operations";
            this.tabPageImageOperations.UseVisualStyleBackColor = true;
            // 
            // pnlDynamicAddButtons
            // 
            this.pnlDynamicAddButtons.AutoScroll = true;
            this.pnlDynamicAddButtons.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlDynamicAddButtons.Location = new System.Drawing.Point(6, 70);
            this.pnlDynamicAddButtons.Name = "pnlDynamicAddButtons";
            this.pnlDynamicAddButtons.Size = new System.Drawing.Size(255, 290);
            this.pnlDynamicAddButtons.TabIndex = 2;
            this.pnlDynamicAddButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDynamicAddButtons.WrapContents = false;
            // 
            // btnNext
            // 
            this.btnNext.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnNext.Location = new System.Drawing.Point(137, 8);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(120, 56);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Next >";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // btnPrev
            // 
            this.btnPrev.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnPrev.Location = new System.Drawing.Point(8, 8);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(120, 56);
            this.btnPrev.TabIndex = 0;
            this.btnPrev.Text = "< Prev";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // tabPageCategories
            // 
            this.tabPageCategories.Controls.Add(this.grpCategoryActions);
            this.tabPageCategories.Controls.Add(this.btnDeleteCategory);
            this.tabPageCategories.Controls.Add(this.btnAddCategory);
            this.tabPageCategories.Controls.Add(this.txtNewCategory);
            this.tabPageCategories.Controls.Add(this.lblCategories);
            this.tabPageCategories.Controls.Add(this.lstCategories);
            this.tabPageCategories.Location = new System.Drawing.Point(4, 24);
            this.tabPageCategories.Name = "tabPageCategories";
            this.tabPageCategories.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCategories.Size = new System.Drawing.Size(267, 372);
            this.tabPageCategories.TabIndex = 1;
            this.tabPageCategories.Text = "Category Management";
            this.tabPageCategories.UseVisualStyleBackColor = true;
            // 
            // grpCategoryActions
            // 
            this.grpCategoryActions.Controls.Add(this.btnMove);
            this.grpCategoryActions.Controls.Add(this.btnCopy);
            this.grpCategoryActions.Location = new System.Drawing.Point(6, 290);
            this.grpCategoryActions.Name = "grpCategoryActions";
            this.grpCategoryActions.Size = new System.Drawing.Size(255, 60);
            this.grpCategoryActions.TabIndex = 11;
            this.grpCategoryActions.TabStop = false;
            this.grpCategoryActions.Text = "Actions for All Images in Category";
            this.grpCategoryActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // btnMove
            // 
            this.btnMove.Location = new System.Drawing.Point(129, 22);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(120, 23);
            this.btnMove.TabIndex = 1;
            this.btnMove.Text = "Move";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(6, 22);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(120, 23);
            this.btnCopy.TabIndex = 0;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnDeleteCategory
            // 
            this.btnDeleteCategory.Location = new System.Drawing.Point(6, 261);
            this.btnDeleteCategory.Name = "btnDeleteCategory";
            this.btnDeleteCategory.Size = new System.Drawing.Size(255, 23);
            this.btnDeleteCategory.TabIndex = 10;
            this.btnDeleteCategory.Text = "Delete Selected Category";
            this.btnDeleteCategory.UseVisualStyleBackColor = true;
            this.btnDeleteCategory.Click += new System.EventHandler(this.btnDeleteCategory_Click);
            this.btnDeleteCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // btnAddCategory
            // 
            this.btnAddCategory.Location = new System.Drawing.Point(177, 232);
            this.btnAddCategory.Name = "btnAddCategory";
            this.btnAddCategory.Size = new System.Drawing.Size(84, 23);
            this.btnAddCategory.TabIndex = 9;
            this.btnAddCategory.Text = "Add";
            this.btnAddCategory.UseVisualStyleBackColor = true;
            this.btnAddCategory.Click += new System.EventHandler(this.btnAddCategory_Click);
            this.btnAddCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // txtNewCategory
            // 
            this.txtNewCategory.Location = new System.Drawing.Point(6, 232);
            this.txtNewCategory.Name = "txtNewCategory";
            this.txtNewCategory.PlaceholderText = "Enter new category name...";
            this.txtNewCategory.Size = new System.Drawing.Size(165, 23);
            this.txtNewCategory.TabIndex = 8;
            this.txtNewCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // lblCategories
            // 
            this.lblCategories.AutoSize = true;
            this.lblCategories.Location = new System.Drawing.Point(6, 15);
            this.lblCategories.Name = "lblCategories";
            this.lblCategories.Size = new System.Drawing.Size(63, 15);
            this.lblCategories.TabIndex = 7;
            // 
            // lstCategories
            // 
            this.lstCategories.FormattingEnabled = true;
            this.lstCategories.ItemHeight = 15;
            this.lstCategories.Location = new System.Drawing.Point(6, 33);
            this.lstCategories.Name = "lstCategories";
            this.lstCategories.Size = new System.Drawing.Size(255, 184);
            this.lstCategories.TabIndex = 6;
            this.lstCategories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // rtbLog
            // 
            this.rtbLog.Location = new System.Drawing.Point(12, 410);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbLog.Size = new System.Drawing.Size(260, 110);
            this.rtbLog.TabIndex = 9;
            this.rtbLog.Text = "";
            this.rtbLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 530); // Initial size, will AutoSize
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.tabControlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle; // Changed to FixedSingle
            this.MaximizeBox = false; // Changed to false
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image Tagger";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControlMain.ResumeLayout(false);
            this.tabPageImageOperations.ResumeLayout(false);
            this.tabPageCategories.ResumeLayout(false);
            this.tabPageCategories.PerformLayout();
            this.grpCategoryActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageImageOperations;
        private System.Windows.Forms.TabPage tabPageCategories;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.ListBox lstCategories;
        private System.Windows.Forms.TextBox txtNewCategory;
        private System.Windows.Forms.Button btnAddCategory;
        private System.Windows.Forms.Button btnDeleteCategory;
        private System.Windows.Forms.GroupBox grpCategoryActions;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Label lblCategories;
        private System.Windows.Forms.FlowLayoutPanel pnlDynamicAddButtons;
        private System.Windows.Forms.RichTextBox rtbLog;
    }
}

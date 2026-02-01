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
            this.tabPageTagging = new System.Windows.Forms.TabPage();
            this.pnlDynamicAddButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.tabPageTags = new System.Windows.Forms.TabPage();
            this.grpTagActions = new System.Windows.Forms.GroupBox();
            this.btnMove = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnDeleteTag = new System.Windows.Forms.Button();
            this.btnAddTag = new System.Windows.Forms.Button();
            this.txtNewTag = new System.Windows.Forms.TextBox();
            this.lblTags = new System.Windows.Forms.Label();
            this.lstTags = new System.Windows.Forms.ListBox();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.tabControlMain.SuspendLayout();
            this.tabPageTagging.SuspendLayout();
            this.tabPageTags.SuspendLayout();
            this.grpTagActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageTagging);
            this.tabControlMain.Controls.Add(this.tabPageTags);
            this.tabControlMain.Location = new System.Drawing.Point(5, 5);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(275, 400);
            this.tabControlMain.TabIndex = 8;
            this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlMain.SelectedIndexChanged += new System.EventHandler(this.tabControlMain_SelectedIndexChanged);
            // 
            // tabPageTagging
            // 
            this.tabPageTagging.Controls.Add(this.pnlDynamicAddButtons);
            this.tabPageTagging.Controls.Add(this.btnNext);
            this.tabPageTagging.Controls.Add(this.btnUndo);
            this.tabPageTagging.Controls.Add(this.btnPrev);
            this.tabPageTagging.Location = new System.Drawing.Point(4, 24);
            this.tabPageTagging.Name = "tabPageTagging";
            this.tabPageTagging.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTagging.Size = new System.Drawing.Size(267, 372);
            this.tabPageTagging.TabIndex = 0;
            this.tabPageTagging.Text = "Tagging";
            this.tabPageTagging.UseVisualStyleBackColor = true;
            // 
            // pnlDynamicAddButtons
            // 
            this.pnlDynamicAddButtons.AutoScroll = true;
            this.pnlDynamicAddButtons.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlDynamicAddButtons.Location = new System.Drawing.Point(6, 70);
            this.pnlDynamicAddButtons.Name = "pnlDynamicAddButtons";
            this.pnlDynamicAddButtons.Size = new System.Drawing.Size(255, 290);
            this.pnlDynamicAddButtons.TabIndex = 3;
            this.pnlDynamicAddButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDynamicAddButtons.WrapContents = false;
            // 
            // btnNext
            // 
            this.btnNext.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnNext.Location = new System.Drawing.Point(156, 8);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(103, 56);
            this.btnNext.TabIndex = 2;
            this.btnNext.Text = "Next >";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // btnUndo
            // 
            this.btnUndo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnUndo.Location = new System.Drawing.Point(116, 8);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(35, 56);
            this.btnUndo.TabIndex = 1;
            this.btnUndo.Text = "⟲";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            this.btnUndo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // btnPrev
            // 
            this.btnPrev.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnPrev.Location = new System.Drawing.Point(8, 8);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(103, 56);
            this.btnPrev.TabIndex = 0;
            this.btnPrev.Text = "< Prev";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // tabPageTags
            // 
            this.tabPageTags.Controls.Add(this.grpTagActions);
            this.tabPageTags.Controls.Add(this.btnDeleteTag);
            this.tabPageTags.Controls.Add(this.btnAddTag);
            this.tabPageTags.Controls.Add(this.txtNewTag);
            this.tabPageTags.Controls.Add(this.lblTags);
            this.tabPageTags.Controls.Add(this.lstTags);
            this.tabPageTags.Location = new System.Drawing.Point(4, 24);
            this.tabPageTags.Name = "tabPageTags";
            this.tabPageTags.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTags.Size = new System.Drawing.Size(267, 372);
            this.tabPageTags.TabIndex = 1;
            this.tabPageTags.Text = "Tags";
            this.tabPageTags.UseVisualStyleBackColor = true;
            // 
            // grpTagActions
            // 
            this.grpTagActions.Controls.Add(this.btnMove);
            this.grpTagActions.Controls.Add(this.btnCopy);
            this.grpTagActions.Location = new System.Drawing.Point(6, 290);
            this.grpTagActions.Name = "grpTagActions";
            this.grpTagActions.Size = new System.Drawing.Size(255, 60);
            this.grpTagActions.TabIndex = 11;
            this.grpTagActions.TabStop = false;
            this.grpTagActions.Text = "Tag List Actions";
            this.grpTagActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // btnMove
            // 
            this.btnMove.Location = new System.Drawing.Point(129, 22);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(120, 23);
            this.btnMove.TabIndex = 1;
            this.btnMove.Text = "Move to...";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(6, 22);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(120, 23);
            this.btnCopy.TabIndex = 0;
            this.btnCopy.Text = "Copy to...";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnDeleteTag
            // 
            this.btnDeleteTag.Location = new System.Drawing.Point(6, 261);
            this.btnDeleteTag.Name = "btnDeleteTag";
            this.btnDeleteTag.Size = new System.Drawing.Size(255, 23);
            this.btnDeleteTag.TabIndex = 10;
            this.btnDeleteTag.Text = "Delete Selected Tag";
            this.btnDeleteTag.UseVisualStyleBackColor = true;
            this.btnDeleteTag.Click += new System.EventHandler(this.btnDeleteTag_Click);
            this.btnDeleteTag.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // btnAddTag
            // 
            this.btnAddTag.Location = new System.Drawing.Point(177, 232);
            this.btnAddTag.Name = "btnAddTag";
            this.btnAddTag.Size = new System.Drawing.Size(84, 23);
            this.btnAddTag.TabIndex = 9;
            this.btnAddTag.Text = "Add";
            this.btnAddTag.UseVisualStyleBackColor = true;
            this.btnAddTag.Click += new System.EventHandler(this.btnAddTag_Click);
            this.btnAddTag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // txtNewTag
            // 
            this.txtNewTag.Location = new System.Drawing.Point(6, 232);
            this.txtNewTag.Name = "txtNewTag";
            this.txtNewTag.PlaceholderText = "Enter new tag name...";
            this.txtNewTag.Size = new System.Drawing.Size(165, 23);
            this.txtNewTag.TabIndex = 8;
            this.txtNewTag.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // lblTags
            // 
            this.lblTags.AutoSize = true;
            this.lblTags.Location = new System.Drawing.Point(6, 15);
            this.lblTags.Name = "lblTags";
            this.lblTags.Size = new System.Drawing.Size(32, 15);
            this.lblTags.TabIndex = 7;
            this.lblTags.Text = "Tags:"; // Changed to "Tags:" from "Categories" (implied) or similar? No, original was just "lblCategories". I'll assume text is "Tags"
            // Wait, looking at previous file, there was no Text set for lblCategories in the snippet I saw? 
            // Ah, I missed it. Let's check the previous read. 
            // `this.lblCategories = new System.Windows.Forms.Label();`
            // ...
            // `this.lblCategories.Name = "lblCategories";`
            // It seems the text wasn't set in InitializeComponent explicitly? Or I missed it.
            // Wait, the previous read shows:
            // `this.lblCategories.TabIndex = 7;`
            // But no `.Text =`. It might be in the generated code but hidden or default. 
            // I'll set it to "Tags".
            // Actually, in the previous read:
            // `this.lblCategories.Size = new System.Drawing.Size(63, 15);`
            // But no text line. Weird. Default label text is name.
            // I will set Text = "Tags".
            // 
            // lstTags
            // 
            this.lstTags.FormattingEnabled = true;
            this.lstTags.ItemHeight = 15;
            this.lstTags.Location = new System.Drawing.Point(6, 33);
            this.lstTags.Name = "lstTags";
            this.lstTags.Size = new System.Drawing.Size(255, 184);
            this.lstTags.TabIndex = 6;
            this.lstTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
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
            this.tabPageTagging.ResumeLayout(false);
            this.tabPageTags.ResumeLayout(false);
            this.tabPageTags.PerformLayout();
            this.grpTagActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageTagging;
        private System.Windows.Forms.TabPage tabPageTags;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.ListBox lstTags;
        private System.Windows.Forms.TextBox txtNewTag;
        private System.Windows.Forms.Button btnAddTag;
        private System.Windows.Forms.Button btnDeleteTag;
        private System.Windows.Forms.GroupBox grpTagActions;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Label lblTags;
        private System.Windows.Forms.FlowLayoutPanel pnlDynamicAddButtons;
        private System.Windows.Forms.RichTextBox rtbLog;
    }
}
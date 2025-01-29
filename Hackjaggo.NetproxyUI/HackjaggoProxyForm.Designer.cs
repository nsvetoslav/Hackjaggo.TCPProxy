namespace Hackjaggo.NetproxyUI
{
    partial class HackjaggoProxyForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HackjaggoProxyForm));
            routingInformationLabel = new Label();
            currentConnectionsListView = new ListView();
            rejectedConnectionsListView = new ListView();
            establishedConnectionsLabel = new Label();
            rejectedConnectionsLabel = new Label();
            button1 = new Button();
            Clear = new Button();
            startStopButton = new Button();
            statusPictureBox = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)statusPictureBox).BeginInit();
            SuspendLayout();
            // 
            // routingInformationLabel
            // 
            routingInformationLabel.AutoSize = true;
            routingInformationLabel.FlatStyle = FlatStyle.Popup;
            routingInformationLabel.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold | FontStyle.Italic);
            routingInformationLabel.Location = new Point(113, 8);
            routingInformationLabel.Name = "routingInformationLabel";
            routingInformationLabel.Size = new Size(238, 19);
            routingInformationLabel.TabIndex = 0;
            routingInformationLabel.Text = "Routing over TCP from ${IP} to ${IP}";
            // 
            // currentConnectionsListView
            // 
            currentConnectionsListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            currentConnectionsListView.FullRowSelect = true;
            currentConnectionsListView.GridLines = true;
            currentConnectionsListView.LabelEdit = true;
            currentConnectionsListView.Location = new Point(12, 55);
            currentConnectionsListView.Name = "currentConnectionsListView";
            currentConnectionsListView.Size = new Size(536, 395);
            currentConnectionsListView.TabIndex = 1;
            currentConnectionsListView.UseCompatibleStateImageBehavior = false;
            currentConnectionsListView.View = View.Details;
            // 
            // rejectedConnectionsListView
            // 
            rejectedConnectionsListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rejectedConnectionsListView.FullRowSelect = true;
            rejectedConnectionsListView.GridLines = true;
            rejectedConnectionsListView.ImeMode = ImeMode.NoControl;
            rejectedConnectionsListView.LabelEdit = true;
            rejectedConnectionsListView.Location = new Point(554, 55);
            rejectedConnectionsListView.Name = "rejectedConnectionsListView";
            rejectedConnectionsListView.Size = new Size(547, 395);
            rejectedConnectionsListView.TabIndex = 2;
            rejectedConnectionsListView.UseCompatibleStateImageBehavior = false;
            rejectedConnectionsListView.View = View.Details;
            // 
            // establishedConnectionsLabel
            // 
            establishedConnectionsLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            establishedConnectionsLabel.AutoSize = true;
            establishedConnectionsLabel.Location = new Point(12, 37);
            establishedConnectionsLabel.Name = "establishedConnectionsLabel";
            establishedConnectionsLabel.Size = new Size(134, 15);
            establishedConnectionsLabel.TabIndex = 3;
            establishedConnectionsLabel.Text = "Established connections";
            // 
            // rejectedConnectionsLabel
            // 
            rejectedConnectionsLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            rejectedConnectionsLabel.AutoSize = true;
            rejectedConnectionsLabel.Location = new Point(554, 37);
            rejectedConnectionsLabel.Name = "rejectedConnectionsLabel";
            rejectedConnectionsLabel.Size = new Size(120, 15);
            rejectedConnectionsLabel.TabIndex = 4;
            rejectedConnectionsLabel.Text = "Rejected connections";
            rejectedConnectionsLabel.Click += label3_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.Location = new Point(1026, 29);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 5;
            button1.Text = "Clear";
            button1.UseVisualStyleBackColor = true;
            button1.Click += OnRejectedConnectionsClearButton;
            // 
            // Clear
            // 
            Clear.Location = new Point(473, 29);
            Clear.Name = "Clear";
            Clear.Size = new Size(75, 23);
            Clear.TabIndex = 6;
            Clear.Text = "Clear";
            Clear.UseVisualStyleBackColor = true;
            Clear.Click += OnCurrentConnectionsClearButton;
            // 
            // startStopButton
            // 
            startStopButton.Location = new Point(32, 6);
            startStopButton.Name = "startStopButton";
            startStopButton.Size = new Size(75, 23);
            startStopButton.TabIndex = 7;
            startStopButton.Text = "Stop";
            startStopButton.UseVisualStyleBackColor = true;
            startStopButton.Click += startStopButton_ClickAsync;
            // 
            // statusPictureBox
            // 
            statusPictureBox.InitialImage = null;
            statusPictureBox.Location = new Point(10, 11);
            statusPictureBox.Name = "statusPictureBox";
            statusPictureBox.Size = new Size(16, 16);
            statusPictureBox.TabIndex = 8;
            statusPictureBox.TabStop = false;
            // 
            // HackjaggoProxyForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(1113, 472);
            Controls.Add(statusPictureBox);
            Controls.Add(startStopButton);
            Controls.Add(Clear);
            Controls.Add(button1);
            Controls.Add(rejectedConnectionsLabel);
            Controls.Add(establishedConnectionsLabel);
            Controls.Add(rejectedConnectionsListView);
            Controls.Add(currentConnectionsListView);
            Controls.Add(routingInformationLabel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1112, 489);
            Name = "HackjaggoProxyForm";
            Text = "JaggoProxy";
            ((System.ComponentModel.ISupportInitialize)statusPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public Label routingInformationLabel;
        public ListView currentConnectionsListView;
        public ListView rejectedConnectionsListView;
        public Label establishedConnectionsLabel;
        public Label rejectedConnectionsLabel;
        public Button button1;
        public Button Clear;
        public Button startStopButton;
        public PictureBox statusPictureBox;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem locateToolStripMenuItem;
    }
}

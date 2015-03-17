namespace TSD_Slider.UI.Forms
{
    partial class TSD_SliderView
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
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.MeasurementName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MeasuredValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Units = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LowerLimit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpperLimit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.traceListenerTextBoxControl = new TsdLib.UI.Controls.TraceListenerTextBoxControl();
            this.progressControl = new TsdLib.UI.Controls.ProgressControl();
            this.multiConfigControl = new TsdLib.UI.Controls.MultiConfigControl();
            this.testDetailsControl = new TsdLib.UI.Controls.TestDetailsControl();
            this.testSequenceControl = new TsdLib.UI.Controls.TestSequenceControl();
            this.testInfoDataGridViewControl = new TsdLib.UI.Controls.TestInfoDataGridViewControl();
            this.measurementDataGridViewControl = new TsdLib.UI.Controls.MeasurementDataGridViewControl();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.testCasesMenuItem = new TsdLib.UI.Controls.TestCasesMenuItem(this.components);
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MeasurementName
            // 
            this.MeasurementName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.MeasurementName.FillWeight = 200F;
            this.MeasurementName.HeaderText = "MeasurementName";
            this.MeasurementName.Name = "MeasurementName";
            this.MeasurementName.ReadOnly = true;
            // 
            // MeasuredValue
            // 
            this.MeasuredValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.MeasuredValue.FillWeight = 300F;
            this.MeasuredValue.HeaderText = "MeasuredValue";
            this.MeasuredValue.Name = "MeasuredValue";
            this.MeasuredValue.ReadOnly = true;
            // 
            // Units
            // 
            this.Units.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Units.FillWeight = 150F;
            this.Units.HeaderText = "Units";
            this.Units.Name = "Units";
            this.Units.ReadOnly = true;
            // 
            // LowerLimit
            // 
            this.LowerLimit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.LowerLimit.FillWeight = 200F;
            this.LowerLimit.HeaderText = "LowerLimit";
            this.LowerLimit.Name = "LowerLimit";
            this.LowerLimit.ReadOnly = true;
            // 
            // UpperLimit
            // 
            this.UpperLimit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.UpperLimit.FillWeight = 200F;
            this.UpperLimit.HeaderText = "UpperLimit";
            this.UpperLimit.Name = "UpperLimit";
            this.UpperLimit.ReadOnly = true;
            // 
            // Result
            // 
            this.Result.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Result.FillWeight = 150F;
            this.Result.HeaderText = "Result";
            this.Result.Name = "Result";
            this.Result.ReadOnly = true;
            // 
            // traceListenerTextBoxControl
            // 
            this.traceListenerTextBoxControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.traceListenerTextBoxControl.Location = new System.Drawing.Point(12, 615);
            this.traceListenerTextBoxControl.Name = "traceListenerTextBoxControl";
            this.traceListenerTextBoxControl.Size = new System.Drawing.Size(1032, 153);
            this.traceListenerTextBoxControl.TabIndex = 5;
            // 
            // progressControl
            // 
            this.progressControl.Location = new System.Drawing.Point(12, 568);
            this.progressControl.Name = "progressControl";
            this.progressControl.Size = new System.Drawing.Size(1032, 41);
            this.progressControl.TabIndex = 7;
            // 
            // multiConfigControl
            // 
            this.multiConfigControl.FirstColumnSizePercent = 50F;
            this.multiConfigControl.Location = new System.Drawing.Point(12, 38);
            this.multiConfigControl.Name = "multiConfigControl";
            this.multiConfigControl.Size = new System.Drawing.Size(387, 222);
            this.multiConfigControl.TabIndex = 6;
            // 
            // testDetailsControl
            // 
            this.testDetailsControl.Location = new System.Drawing.Point(448, 38);
            this.testDetailsControl.Name = "testDetailsControl";
            this.testDetailsControl.Size = new System.Drawing.Size(188, 80);
            this.testDetailsControl.TabIndex = 4;
            // 
            // testSequenceControl
            // 
            this.testSequenceControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.testSequenceControl.Location = new System.Drawing.Point(448, 145);
            this.testSequenceControl.Name = "testSequenceControl";
            this.testSequenceControl.Size = new System.Drawing.Size(188, 115);
            this.testSequenceControl.TabIndex = 3;
            // 
            // testInfoDataGridViewControl
            // 
            this.testInfoDataGridViewControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testInfoDataGridViewControl.Location = new System.Drawing.Point(679, 38);
            this.testInfoDataGridViewControl.Name = "testInfoDataGridViewControl";
            this.testInfoDataGridViewControl.Size = new System.Drawing.Size(365, 222);
            this.testInfoDataGridViewControl.TabIndex = 2;
            // 
            // measurementDataGridViewControl
            // 
            this.measurementDataGridViewControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.measurementDataGridViewControl.DisplayLimitsAndResult = true;
            this.measurementDataGridViewControl.Location = new System.Drawing.Point(12, 280);
            this.measurementDataGridViewControl.Name = "measurementDataGridViewControl";
            this.measurementDataGridViewControl.Size = new System.Drawing.Size(1032, 282);
            this.measurementDataGridViewControl.TabIndex = 1;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testCasesMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1056, 24);
            this.menuStrip.TabIndex = 8;
            this.menuStrip.Text = "menuStrip";
            // 
            // testCasesMenuItem
            // 
            this.testCasesMenuItem.Name = "testCasesMenuItem";
            this.testCasesMenuItem.Size = new System.Drawing.Size(74, 20);
            this.testCasesMenuItem.Text = "&Test Cases";
            // 
            // ViewBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 780);
            this.Controls.Add(this.traceListenerTextBoxControl);
            this.Controls.Add(this.progressControl);
            this.Controls.Add(this.multiConfigControl);
            this.Controls.Add(this.testDetailsControl);
            this.Controls.Add(this.testSequenceControl);
            this.Controls.Add(this.testInfoDataGridViewControl);
            this.Controls.Add(this.measurementDataGridViewControl);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(1072, 751);
            this.Name = "ViewBase";
            this.Text = "TsdLib Generic Test System";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ViewBase_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        /// <summary>
        /// Displays measurements as they are generated by the Test Sequence.
        /// </summary>
        private System.Windows.Forms.DataGridViewTextBoxColumn MeasurementName;
        private System.Windows.Forms.DataGridViewTextBoxColumn MeasuredValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn Units;
        private System.Windows.Forms.DataGridViewTextBoxColumn LowerLimit;
        private System.Windows.Forms.DataGridViewTextBoxColumn UpperLimit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Result;
        protected TsdLib.UI.Controls.MeasurementDataGridViewControl measurementDataGridViewControl;
        protected TsdLib.UI.Controls.TestInfoDataGridViewControl testInfoDataGridViewControl;
        protected TsdLib.UI.Controls.TestSequenceControl testSequenceControl;
        protected TsdLib.UI.Controls.TestDetailsControl testDetailsControl;
        protected TsdLib.UI.Controls.TraceListenerTextBoxControl traceListenerTextBoxControl;
        protected TsdLib.UI.Controls.MultiConfigControl multiConfigControl;
        protected TsdLib.UI.Controls.ProgressControl progressControl;
        protected System.Windows.Forms.MenuStrip menuStrip;
        private TsdLib.UI.Controls.TestCasesMenuItem testCasesMenuItem;
    }
}
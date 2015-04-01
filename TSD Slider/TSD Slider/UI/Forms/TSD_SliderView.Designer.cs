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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.MeasurementName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MeasuredValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Units = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LowerLimit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpperLimit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.testCasesMenuItem = new TsdLib.UI.Controls.TestCasesMenuItem(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbSliderCycle = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtRegCycCompleteName = new System.Windows.Forms.TextBox();
            this.txtRegCyclesName = new System.Windows.Forms.TextBox();
            this.txtRegLiftOffName = new System.Windows.Forms.TextBox();
            this.txtSlideCycleName = new System.Windows.Forms.TextBox();
            this.txtCharName = new System.Windows.Forms.TextBox();
            this.txtFanucIP = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtLiftOff = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtCycleOffset = new System.Windows.Forms.TextBox();
            this.txtCalEvery = new System.Windows.Forms.TextBox();
            this.txtZForce = new System.Windows.Forms.TextBox();
            this.txtNumCycles = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tbData = new System.Windows.Forms.TabPage();
            this.charData1 = new TSD_Slider.UI.Components.CharData();
            this.tbGraphs = new System.Windows.Forms.TabPage();
            this.ResultsChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tbEngScreen = new System.Windows.Forms.TabPage();
            this.progCalibration = new System.Windows.Forms.ProgressBar();
            this.label13 = new System.Windows.Forms.Label();
            this.traceListenerTextBoxControl = new TsdLib.UI.Controls.TraceListenerTextBoxControl();
            this.multiConfigControl = new TsdLib.UI.Controls.MultiConfigControl();
            this.testDetailsControl = new TsdLib.UI.Controls.TestDetailsControl();
            this.testSequenceControl = new TsdLib.UI.Controls.TestSequenceControl();
            this.testInfoDataGridViewControl = new TsdLib.UI.Controls.TestInfoDataGridViewControl();
            this.measurementDataGridViewControl = new TsdLib.UI.Controls.MeasurementDataGridViewControl();
            this.menuStrip.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tbSliderCycle.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tbData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.charData1)).BeginInit();
            this.tbGraphs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResultsChart)).BeginInit();
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
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testCasesMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1552, 24);
            this.menuStrip.TabIndex = 8;
            this.menuStrip.Text = "menuStrip";
            // 
            // testCasesMenuItem
            // 
            this.testCasesMenuItem.Name = "testCasesMenuItem";
            this.testCasesMenuItem.Size = new System.Drawing.Size(74, 20);
            this.testCasesMenuItem.Text = "&Test Cases";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tbSliderCycle);
            this.tabControl1.Controls.Add(this.tbData);
            this.tabControl1.Controls.Add(this.tbGraphs);
            this.tabControl1.Controls.Add(this.tbEngScreen);
            this.tabControl1.Location = new System.Drawing.Point(984, 38);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(558, 792);
            this.tabControl1.TabIndex = 24;
            // 
            // tbSliderCycle
            // 
            this.tbSliderCycle.Controls.Add(this.groupBox2);
            this.tbSliderCycle.Controls.Add(this.groupBox1);
            this.tbSliderCycle.Controls.Add(this.lblTitle);
            this.tbSliderCycle.Location = new System.Drawing.Point(4, 22);
            this.tbSliderCycle.Name = "tbSliderCycle";
            this.tbSliderCycle.Padding = new System.Windows.Forms.Padding(3);
            this.tbSliderCycle.Size = new System.Drawing.Size(550, 766);
            this.tbSliderCycle.TabIndex = 0;
            this.tbSliderCycle.Text = "Slider Cycle";
            this.tbSliderCycle.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblStatus);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.txtRegCycCompleteName);
            this.groupBox2.Controls.Add(this.txtRegCyclesName);
            this.groupBox2.Controls.Add(this.txtRegLiftOffName);
            this.groupBox2.Controls.Add(this.txtSlideCycleName);
            this.groupBox2.Controls.Add(this.txtCharName);
            this.groupBox2.Controls.Add(this.txtFanucIP);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(27, 231);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(513, 517);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Fanuc";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.Black;
            this.lblStatus.Location = new System.Drawing.Point(253, 382);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(192, 31);
            this.lblStatus.TabIndex = 20;
            this.lblStatus.Text = "Disconnected";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(88, 392);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(145, 20);
            this.label11.TabIndex = 19;
            this.label11.Text = "Connection Status:";
            // 
            // txtRegCycCompleteName
            // 
            this.txtRegCycCompleteName.Location = new System.Drawing.Point(265, 192);
            this.txtRegCycCompleteName.Name = "txtRegCycCompleteName";
            this.txtRegCycCompleteName.Size = new System.Drawing.Size(100, 20);
            this.txtRegCycCompleteName.TabIndex = 18;
            // 
            // txtRegCyclesName
            // 
            this.txtRegCyclesName.Location = new System.Drawing.Point(265, 160);
            this.txtRegCyclesName.Name = "txtRegCyclesName";
            this.txtRegCyclesName.Size = new System.Drawing.Size(100, 20);
            this.txtRegCyclesName.TabIndex = 17;
            // 
            // txtRegLiftOffName
            // 
            this.txtRegLiftOffName.Location = new System.Drawing.Point(265, 129);
            this.txtRegLiftOffName.Name = "txtRegLiftOffName";
            this.txtRegLiftOffName.Size = new System.Drawing.Size(100, 20);
            this.txtRegLiftOffName.TabIndex = 16;
            // 
            // txtSlideCycleName
            // 
            this.txtSlideCycleName.Location = new System.Drawing.Point(265, 96);
            this.txtSlideCycleName.Name = "txtSlideCycleName";
            this.txtSlideCycleName.Size = new System.Drawing.Size(100, 20);
            this.txtSlideCycleName.TabIndex = 15;
            // 
            // txtCharName
            // 
            this.txtCharName.Location = new System.Drawing.Point(265, 66);
            this.txtCharName.Name = "txtCharName";
            this.txtCharName.Size = new System.Drawing.Size(100, 20);
            this.txtCharName.TabIndex = 14;
            // 
            // txtFanucIP
            // 
            this.txtFanucIP.Location = new System.Drawing.Point(265, 39);
            this.txtFanucIP.Name = "txtFanucIP";
            this.txtFanucIP.Size = new System.Drawing.Size(100, 20);
            this.txtFanucIP.TabIndex = 8;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(78, 195);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(161, 13);
            this.label10.TabIndex = 13;
            this.label10.Text = "Register Cycles Complete Name:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(125, 163);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(114, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "Register Cycles Name:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(125, 132);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(114, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Register Lift Off Name:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(66, 99);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(173, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Slider Cycle Teach Pendant Name:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(45, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(194, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Characterization Teach Pendant Name:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(121, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(118, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Connection IP Address:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtLiftOff);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.txtCycleOffset);
            this.groupBox1.Controls.Add(this.txtCalEvery);
            this.groupBox1.Controls.Add(this.txtZForce);
            this.groupBox1.Controls.Add(this.txtNumCycles);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(22, 67);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(513, 161);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Slider Control";
            // 
            // txtLiftOff
            // 
            this.txtLiftOff.Location = new System.Drawing.Point(265, 132);
            this.txtLiftOff.Name = "txtLiftOff";
            this.txtLiftOff.Size = new System.Drawing.Size(100, 20);
            this.txtLiftOff.TabIndex = 9;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(169, 132);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(69, 13);
            this.label12.TabIndex = 8;
            this.label12.Text = "Lift Off (mm) :";
            // 
            // txtCycleOffset
            // 
            this.txtCycleOffset.Location = new System.Drawing.Point(265, 106);
            this.txtCycleOffset.Name = "txtCycleOffset";
            this.txtCycleOffset.Size = new System.Drawing.Size(100, 20);
            this.txtCycleOffset.TabIndex = 7;
            // 
            // txtCalEvery
            // 
            this.txtCalEvery.Location = new System.Drawing.Point(265, 76);
            this.txtCalEvery.Name = "txtCalEvery";
            this.txtCalEvery.Size = new System.Drawing.Size(100, 20);
            this.txtCalEvery.TabIndex = 6;
            // 
            // txtZForce
            // 
            this.txtZForce.Location = new System.Drawing.Point(265, 46);
            this.txtZForce.Name = "txtZForce";
            this.txtZForce.Size = new System.Drawing.Size(100, 20);
            this.txtZForce.TabIndex = 5;
            // 
            // txtNumCycles
            // 
            this.txtNumCycles.Location = new System.Drawing.Point(265, 20);
            this.txtNumCycles.Name = "txtNumCycles";
            this.txtNumCycles.Size = new System.Drawing.Size(100, 20);
            this.txtNumCycles.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(169, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Cycle Completed :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(158, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Calibrate Every:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(172, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Z-Force (lb) :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(146, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Number of Cycles:";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(248, 28);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(79, 29);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "label1";
            // 
            // tbData
            // 
            this.tbData.Controls.Add(this.charData1);
            this.tbData.Location = new System.Drawing.Point(4, 22);
            this.tbData.Name = "tbData";
            this.tbData.Size = new System.Drawing.Size(550, 766);
            this.tbData.TabIndex = 3;
            this.tbData.Text = "Data";
            this.tbData.UseVisualStyleBackColor = true;
            // 
            // charData1
            // 
            this.charData1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.charData1.Displacement = null;
            this.charData1.Force = null;
            this.charData1.Location = new System.Drawing.Point(38, 20);
            this.charData1.Name = "charData1";
            this.charData1.ReadOnly = true;
            this.charData1.Size = new System.Drawing.Size(379, 643);
            this.charData1.TabIndex = 0;
            // 
            // tbGraphs
            // 
            this.tbGraphs.Controls.Add(this.ResultsChart);
            this.tbGraphs.Location = new System.Drawing.Point(4, 22);
            this.tbGraphs.Name = "tbGraphs";
            this.tbGraphs.Padding = new System.Windows.Forms.Padding(3);
            this.tbGraphs.Size = new System.Drawing.Size(550, 766);
            this.tbGraphs.TabIndex = 1;
            this.tbGraphs.Text = "Graphs";
            this.tbGraphs.UseVisualStyleBackColor = true;
            // 
            // ResultsChart
            // 
            chartArea1.Name = "ChartArea1";
            this.ResultsChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.ResultsChart.Legends.Add(legend1);
            this.ResultsChart.Location = new System.Drawing.Point(17, 17);
            this.ResultsChart.Name = "ResultsChart";
            this.ResultsChart.Size = new System.Drawing.Size(519, 709);
            this.ResultsChart.TabIndex = 0;
            this.ResultsChart.Text = "chart1";
            // 
            // tbEngScreen
            // 
            this.tbEngScreen.Location = new System.Drawing.Point(4, 22);
            this.tbEngScreen.Name = "tbEngScreen";
            this.tbEngScreen.Size = new System.Drawing.Size(550, 766);
            this.tbEngScreen.TabIndex = 2;
            this.tbEngScreen.Text = "Engineering Screen";
            this.tbEngScreen.UseVisualStyleBackColor = true;
            // 
            // progCalibration
            // 
            this.progCalibration.Location = new System.Drawing.Point(93, 280);
            this.progCalibration.Name = "progCalibration";
            this.progCalibration.Size = new System.Drawing.Size(885, 23);
            this.progCalibration.TabIndex = 25;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(28, 285);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 13);
            this.label13.TabIndex = 26;
            this.label13.Text = "Calibration:";
            // 
            // traceListenerTextBoxControl
            // 
            this.traceListenerTextBoxControl.Location = new System.Drawing.Point(16, 597);
            this.traceListenerTextBoxControl.Name = "traceListenerTextBoxControl";
            this.traceListenerTextBoxControl.Size = new System.Drawing.Size(959, 233);
            this.traceListenerTextBoxControl.TabIndex = 5;
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
            this.testSequenceControl.Location = new System.Drawing.Point(448, 131);
            this.testSequenceControl.Name = "testSequenceControl";
            this.testSequenceControl.Size = new System.Drawing.Size(189, 115);
            this.testSequenceControl.TabIndex = 3;
            // 
            // testInfoDataGridViewControl
            // 
            this.testInfoDataGridViewControl.Location = new System.Drawing.Point(679, 38);
            this.testInfoDataGridViewControl.Name = "testInfoDataGridViewControl";
            this.testInfoDataGridViewControl.Size = new System.Drawing.Size(293, 222);
            this.testInfoDataGridViewControl.TabIndex = 2;
            // 
            // measurementDataGridViewControl
            // 
            this.measurementDataGridViewControl.DisplayLimitsAndResult = true;
            this.measurementDataGridViewControl.Location = new System.Drawing.Point(12, 309);
            this.measurementDataGridViewControl.Name = "measurementDataGridViewControl";
            this.measurementDataGridViewControl.Size = new System.Drawing.Size(961, 282);
            this.measurementDataGridViewControl.TabIndex = 1;
            // 
            // TSD_SliderView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1552, 842);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.progCalibration);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.traceListenerTextBoxControl);
            this.Controls.Add(this.multiConfigControl);
            this.Controls.Add(this.testDetailsControl);
            this.Controls.Add(this.testSequenceControl);
            this.Controls.Add(this.testInfoDataGridViewControl);
            this.Controls.Add(this.measurementDataGridViewControl);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(1072, 751);
            this.Name = "TSD_SliderView";
            this.Text = "TsdLib Generic Test System";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ViewBase_FormClosing);
            this.Load += new System.EventHandler(this.OnLoad);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tbSliderCycle.ResumeLayout(false);
            this.tbSliderCycle.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tbData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.charData1)).EndInit();
            this.tbGraphs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ResultsChart)).EndInit();
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
        protected System.Windows.Forms.MenuStrip menuStrip;
        private TsdLib.UI.Controls.TestCasesMenuItem testCasesMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tbSliderCycle;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtRegCycCompleteName;
        private System.Windows.Forms.TextBox txtRegCyclesName;
        private System.Windows.Forms.TextBox txtRegLiftOffName;
        private System.Windows.Forms.TextBox txtSlideCycleName;
        private System.Windows.Forms.TextBox txtCharName;
        private System.Windows.Forms.TextBox txtFanucIP;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtLiftOff;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtCycleOffset;
        private System.Windows.Forms.TextBox txtCalEvery;
        private System.Windows.Forms.TextBox txtZForce;
        private System.Windows.Forms.TextBox txtNumCycles;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TabPage tbData;
        private System.Windows.Forms.TabPage tbGraphs;
        private System.Windows.Forms.TabPage tbEngScreen;
        private System.Windows.Forms.ProgressBar progCalibration;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.DataVisualization.Charting.Chart ResultsChart;
        private Components.CharData charData1;
    }
}
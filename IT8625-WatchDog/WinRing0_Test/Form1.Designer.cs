namespace WinRing0_Test
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chip_name = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.fan_ram = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cpu_tem = new System.Windows.Forms.Label();
            this.mod = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.HddTemp = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10F);
            this.label1.Location = new System.Drawing.Point(240, 172);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(222, 27);
            this.label1.TabIndex = 0;
            this.label1.Text = "driver init ...";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 224);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(178, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "当前芯片型号：";
            // 
            // chip_name
            // 
            this.chip_name.AutoSize = true;
            this.chip_name.Location = new System.Drawing.Point(222, 224);
            this.chip_name.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.chip_name.Name = "chip_name";
            this.chip_name.Size = new System.Drawing.Size(82, 24);
            this.chip_name.TabIndex = 2;
            this.chip_name.Text = "unknow";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 456);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(478, 24);
            this.label4.TabIndex = 3;
            this.label4.Text = "CPU风扇转速[如被动散热或没有接则为10]：";
            // 
            // fan_ram
            // 
            this.fan_ram.AutoSize = true;
            this.fan_ram.Location = new System.Drawing.Point(516, 456);
            this.fan_ram.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.fan_ram.Name = "fan_ram";
            this.fan_ram.Size = new System.Drawing.Size(82, 24);
            this.fan_ram.TabIndex = 4;
            this.fan_ram.Text = "unknow";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 174);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(190, 24);
            this.label3.TabIndex = 5;
            this.label3.Text = "SuperIO_DERVICE";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(32, 508);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(490, 24);
            this.label5.TabIndex = 6;
            this.label5.Text = "本程序启动后默认加载看门狗，并自动最小化";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(290, 44);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(260, 48);
            this.label6.TabIndex = 8;
            this.label6.Text = "专用看门狗";
            // 
            // cpu_tem
            // 
            this.cpu_tem.AutoSize = true;
            this.cpu_tem.Location = new System.Drawing.Point(196, 282);
            this.cpu_tem.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.cpu_tem.Name = "cpu_tem";
            this.cpu_tem.Size = new System.Drawing.Size(82, 24);
            this.cpu_tem.TabIndex = 11;
            this.cpu_tem.Text = "unknow";
            // 
            // mod
            // 
            this.mod.AutoSize = true;
            this.mod.Location = new System.Drawing.Point(196, 338);
            this.mod.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.mod.Name = "mod";
            this.mod.Size = new System.Drawing.Size(82, 24);
            this.mod.TabIndex = 12;
            this.mod.Text = "unknow";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(42, 282);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(118, 24);
            this.label7.TabIndex = 13;
            this.label7.Text = "CPU温度：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(42, 338);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(130, 24);
            this.label8.TabIndex = 14;
            this.label8.Text = "主板温度：";
            // 
            // HddTemp
            // 
            this.HddTemp.AutoSize = true;
            this.HddTemp.Location = new System.Drawing.Point(196, 398);
            this.HddTemp.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.HddTemp.Name = "HddTemp";
            this.HddTemp.Size = new System.Drawing.Size(82, 24);
            this.HddTemp.TabIndex = 15;
            this.HddTemp.Text = "unknow";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(46, 396);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(130, 24);
            this.label9.TabIndex = 16;
            this.label9.Text = "硬盘温度：";
            // 
            // chkAutoStart
            // 
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkAutoStart.Location = new System.Drawing.Point(474, 122);
            this.chkAutoStart.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(239, 37);
            this.chkAutoStart.TabIndex = 17;
            this.chkAutoStart.Text = "开机自动启动";
            this.chkAutoStart.UseVisualStyleBackColor = true;
            this.chkAutoStart.CheckedChanged += new System.EventHandler(this.chkAutoStart_CheckedChanged_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 576);
            this.Controls.Add(this.chkAutoStart);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.HddTemp);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.mod);
            this.Controls.Add(this.cpu_tem);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fan_ram);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chip_name);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "Form1";
            this.Text = "Aiotoia_Wacthdog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label chip_name;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label fan_ram;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label cpu_tem;
        private System.Windows.Forms.Label mod;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label HddTemp;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox chkAutoStart;
    }
}


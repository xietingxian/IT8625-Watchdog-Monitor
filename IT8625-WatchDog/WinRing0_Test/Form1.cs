using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections; // 添加Queue所需命名空间
using System.Management; // 添加此命名空间

namespace WinRing0_Test
{
    public partial class Form1 : Form
    {
        private TestByWinRing0 testByWinRing0 = null;
        private static System.Timers.Timer Timer;
        private NotifyIcon notifyIcon;
        private int feedCount = 0;
        private bool hddTemperatureEverShown = false;

        // 温度历史记录用于滤波
        private Queue<float> cpuTempHistory = new Queue<float>();
        private Queue<float> mbTempHistory = new Queue<float>();
        private Queue<float> hddTempHistory = new Queue<float>();
        private const int FILTER_WINDOW = 5;

        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
            InitializeAutoStartSetting();// 注册表初始化
            InitLogSystem();
            Init_Driver();
            this.WindowState = FormWindowState.Minimized;
            this.SizeChanged += MainFormSizeChanged;
            this.FormClosing += new FormClosingEventHandler(offdog_FormClosing);
        }
        private void InitializeAutoStartSetting()
        {
            // 检查注册表设置并更新UI
            chkAutoStart.Checked = RegistryHelper.IsAutoStartEnabled();

            // 添加事件处理
            chkAutoStart.CheckedChanged += chkAutoStart_CheckedChanged_1;
        }



        private void InitializeCustomComponents()
        {
            // 初始化通知图标
            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.Icon = SystemIcons.Information; // 默认图标
            this.notifyIcon.Visible = true;
            this.notifyIcon.Click += NotifyIconClick;
            this.notifyIcon.Text = "Aiotoia_WatchDog";

            // 设置初始文本
            cpu_tem.Text = "-- °C";
            mod.Text = "-- °C";
            HddTemp.Text = " -- °C";
            fan_ram.Text = "-- RPM";

            // 设置颜色
           // HddTemp.ForeColor = Color.DarkMagenta;
        }

        // 初始化日志系统
        private void InitLogSystem()
        {
            try
            {
                string logDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Aiotoia_Watchdog");

                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                string logPath = Path.Combine(logDir, "watchdog_log3.txt");
                File.AppendAllText(logPath, $"===== 看门狗程序启动 {DateTime.Now} =====\n");
            }
            catch { /* 忽略日志错误 */ }
        }

        private void LogEvent(string message, bool isError = false)
        {
            try
            {
                string logDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Aiotoia_Watchdog");
                string logPath = Path.Combine(logDir, "watchdog_log3.txt");

                string prefix = isError ? "[ERROR]" : "[INFO]";
                File.AppendAllText(logPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {prefix} {message}\n");
            }
            catch { /* 忽略日志错误 */ }
        }

        private void offdog_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                LogEvent("===== 看门狗程序关闭 =====");
                LogEvent($"总喂狗次数: {feedCount}");

                if (testByWinRing0 != null)
                {
                    testByWinRing0.closewatchdog();
                    LogEvent("看门狗已关闭");
                }

                if (MessageBox.Show("确定要关闭看门狗程序吗？", "确认关闭",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                LogEvent($"关闭错误: {ex.Message}", true);
            }
            finally
            {
                if (notifyIcon != null)
                    notifyIcon.Dispose();
            }
        }

        private void MainFormSizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                if (notifyIcon != null)
                {
                    notifyIcon.ShowBalloonTip(1000, "Aiotoia_看门狗程序",
                        "看门狗已最小化到系统托盘", ToolTipIcon.Info);
                }
            }
        }

        private void NotifyIconClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
        }

        private void Init_Driver()
        {
            try
            {
                testByWinRing0 = new TestByWinRing0();
                bool initResult = testByWinRing0.Initialize();

                if (!initResult)
                {
                    LogEvent("WinRing0初始化失败", true);
                    MessageBox.Show("硬件访问初始化失败，请以管理员权限运行程序", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 如果是首次运行，设置开机自启动
                if (!RegistryHelper.IsAutoStartEnabled())
                {
                    RegistryHelper.SetAutoStart(true);
                    chkAutoStart.Checked = true;
                    LogEvent("已设置开机自启动");
                }

                LogEvent("WinRing0初始化成功");

                testByWinRing0.InitSuperIO();
                string chipName = testByWinRing0.GetChipName();
                LogEvent($"检测到芯片: {chipName}");
                chip_name.Text = chipName; // 更新芯片名称显示

                testByWinRing0.InitEc();
                LogEvent("EC初始化完成");

                testByWinRing0.openwatchdog();
                LogEvent("看门狗已开启");

                // 初始化定时器
                Timer = new System.Timers.Timer(5000); // 5秒刷新间隔
                Timer.AutoReset = true;
                Timer.Elapsed += TimerElapsedHandler;
                Timer.Start();
                LogEvent($"定时监控已启动，间隔: {Timer.Interval}ms");

                this.Load += (s, e) =>
                {
                    // 延迟100ms确保窗体完全初始化
                    System.Threading.Thread.Sleep(100);
                    UpdateFanSpeed();
                };
            }
            catch (Exception ex)
            {
                LogEvent($"初始化错误: {ex.Message}", true);
                MessageBox.Show($"初始化失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TimerElapsedHandler(object sender, ElapsedEventArgs e)
        {
            try
            {
                // 喂狗操作
                if (testByWinRing0 != null)
                {
                    testByWinRing0.feedwatchdog(null, null);
                }
                feedCount++;

                // 保存当前温度值用于日志记录
                float? lastCpuTemp = null;
                float? lastMbTemp = null;
                float? lastHddTemp = null;                

                bool hddAvailable = false;

                // 更新温度和风扇
                UpdateTemperatures(ref lastCpuTemp, ref lastMbTemp, ref lastHddTemp, ref hddAvailable);
                int fanRpm = UpdateFanSpeed();

                // 每150秒记录一次状态
                if (feedCount % 30 == 0)
                {
                    string hddText = hddAvailable ? $", 硬盘: {lastHddTemp:F1}°C" : "";
                    string fanText = fanRpm >= 0 ? $", 风扇: {fanRpm} RPM" : ", 风扇: N/A";
                    LogEvent($"运行状态 - 喂狗次数: {feedCount}, CPU: {lastCpuTemp:F1}°C, 主板: {lastMbTemp:F1}°C{hddText}{fanText}");
                }
            }
            catch (Exception ex)
            {
                LogEvent($"定时操作错误: {ex.Message}", true);
            }
        }

        // 温度滤波算法
        private float GetFilteredTemp(float newTemp, ref Queue<float> history)
        {
            history.Enqueue(newTemp);
            if (history.Count > FILTER_WINDOW)
            {
                history.Dequeue();
            }

            // 计算平均值
            float sum = 0;
            foreach (float temp in history)
            {
                sum += temp;
            }
            return sum / history.Count;
        }


        private void UpdateTemperatures(ref float? lastCpuTemp, ref float? lastMbTemp, ref float? lastHddTemp, ref bool hddAvailable)
        {
            try
            {
                if (testByWinRing0 == null) return;

                float cpuTemp = testByWinRing0.GetCpuTemperature();
                float mbTemp = testByWinRing0.GetMotherboardTemperature();
                float? rawHddTemp = testByWinRing0.GetHddTemperature();

                // 保存温度值用于日志记录
                lastCpuTemp = cpuTemp;
                lastMbTemp = mbTemp;
                lastHddTemp = rawHddTemp;
                hddAvailable = rawHddTemp.HasValue;

                // 应用滤波
                float filteredCpu = GetFilteredTemp(cpuTemp, ref cpuTempHistory);
                float filteredMb = GetFilteredTemp(mbTemp, ref mbTempHistory);

                float filteredHdd = 0;
                bool hddAvailableUI = hddAvailable;

                if (rawHddTemp.HasValue)
                {
                    filteredHdd = GetFilteredTemp(rawHddTemp.Value, ref hddTempHistory);
                    hddAvailable = true;
                }

                // 更新UI
                this.Invoke((MethodInvoker)delegate
                {
                    cpu_tem.Text = $" {filteredCpu:F1}°C";
                    mod.Text = $" {filteredMb:F1}°C";

                    // 温度过高变色
                    cpu_tem.ForeColor = filteredCpu > 75 ? Color.Red : Color.DarkGreen;
                    mod.ForeColor = filteredMb > 65 ? Color.Red : Color.DarkGreen;

                    // 更新硬盘温度显示
                    if (hddAvailableUI)
                    {
                        HddTemp.Text = $" {filteredHdd:F1}°C";
                        HddTemp.ForeColor = filteredHdd > 50 ? Color.Red : Color.DarkGreen;

                    }
                    else
                    {
                        HddTemp.Text = "硬盘温度: N/A";
                        HddTemp.ForeColor = Color.Gray;

                        // 记录失败原因
                        if (!hddTemperatureEverShown && feedCount % 10 == 0)
                        {
                            LogEvent($"无法读取硬盘温度，请检查: 1.管理员权限 2.硬盘支持");
                        }
                    }
                });

                // 温度安全检查
                CheckTemperatureSafety(filteredCpu, filteredMb, hddAvailable ? filteredHdd : (float?)null);
            }
            catch (Exception ex)
            {
                LogEvent($"温度更新失败: {ex.Message}", true);
            }
        }

        private int  UpdateFanSpeed()
        {
            try
            {
                if (testByWinRing0 == null) return-1;

                int rpm = testByWinRing0.GetFanRpm();
                this.Invoke((MethodInvoker)delegate
                {
                    fan_ram.Text = $" {rpm} RPM";

                    // 根据转速变色
                    if (rpm > 3200) fan_ram.ForeColor = Color.Red;
                    //else if (rpm > 2000) fan_ram.ForeColor = Color.Orange;
                    else fan_ram.ForeColor = Color.Green;
                });
                return rpm;
            }
            catch (Exception ex)
            {
                LogEvent($"风扇转速更新失败: {ex.Message}", true);
                return -1;
            }
        }

        private void CheckTemperatureSafety(float cpuTemp, float mbTemp, float? hddTemp)
        {
            const float CPU_MAX_TEMP = 90.0f;
            const float MB_MAX_TEMP = 80.0f;
            const float HDD_MAX_TEMP = 50.0f;

            if (cpuTemp > CPU_MAX_TEMP)
            {
                LogEvent($"CPU过热警告! 当前温度: {cpuTemp}°C", true);
                this.Invoke((MethodInvoker)delegate {
                    if (notifyIcon != null)
                    {
                        notifyIcon.ShowBalloonTip(3000, "温度警告",
                            $"CPU温度过高: {cpuTemp}°C", ToolTipIcon.Warning);
                    }
                });
            }

            if (mbTemp > MB_MAX_TEMP)
            {
                LogEvent($"主板过热警告! 当前温度: {mbTemp}°C", true);
                this.Invoke((MethodInvoker)delegate {
                    if (notifyIcon != null)
                    {
                        notifyIcon.ShowBalloonTip(3000, "温度警告",
                            $"主板温度过高: {mbTemp}°C", ToolTipIcon.Warning);
                    }
                });
            }

            if (hddTemp.HasValue && hddTemp > HDD_MAX_TEMP)
            {
                LogEvent($"硬盘过热警告! 当前温度: {hddTemp}°C", true);
                this.Invoke((MethodInvoker)delegate {
                    if (notifyIcon != null)
                    {
                        notifyIcon.ShowBalloonTip(3000, "温度警告",
                            $"硬盘温度过高: {hddTemp}°C", ToolTipIcon.Warning);
                    }
                });
            }
        }

        // 打开日志菜单项
        private void openLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string logPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Aiotoia_Watchdog", "watchdog_log3.txt");

                if (File.Exists(logPath))
                {
                    Process.Start("notepad.exe", logPath);
                }
                else
                {
                    MessageBox.Show("日志文件不存在", "信息",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开日志: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkAutoStart_CheckedChanged_1(object sender, EventArgs e)
        {
            bool success = RegistryHelper.SetAutoStart(chkAutoStart.Checked);

            if (success)
            {
                string status = chkAutoStart.Checked ? "已启用" : "已禁用";
                LogEvent($"开机自启动{status}");

                //MessageBox.Show($"开机自启动{status}成功", "设置成功",
                //    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                LogEvent("开机自启动设置失败，请尝试以管理员身份运行", true);
                MessageBox.Show("设置失败，请尝试以管理员身份运行程序", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // 恢复之前的状态
                chkAutoStart.Checked = !chkAutoStart.Checked;
            }
        }
    }
}
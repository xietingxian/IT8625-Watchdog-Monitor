using HidSharp;
using LibreHardwareMonitor.Hardware;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management; // 添加命名空间
using System.Text;
using System.Windows.Forms;
using OpenHardwareMonitor.Hardware; // 添加 OpenHardwareMonitor 引用
using OpenHwComputer = OpenHardwareMonitor.Hardware.Computer;
using OpenHwIHardware = OpenHardwareMonitor.Hardware.IHardware;
using OpenHwISensor = OpenHardwareMonitor.Hardware.ISensor;
using OpenHwHardwareType = OpenHardwareMonitor.Hardware.HardwareType;
using OpenHwSensorType = OpenHardwareMonitor.Hardware.SensorType;


namespace WinRing0_Test
{
    public class TestByWinRing0
    {
        private static OpenLibSys.Ols MyOls;
        public static ushort ec_addr_port;
        public static ushort ec_data_port;

        // 添加 OpenHardwareMonitor 相关成员
        private OpenHwComputer _openHardwareComputer;
        private OpenHwIHardware _hddHardware;
        private OpenHwISensor _hddTemperatureSensor;
        private bool _openHwInitialized = false;

        public bool Initialize()
        {
            MyOls = new OpenLibSys.Ols();
            bool olsStatus = MyOls.GetStatus() == (uint)OpenLibSys.Ols.Status.NO_ERROR;

            // 初始化 OpenHardwareMonitor
            InitializeOpenHardwareMonitor();

            return olsStatus;
            //return MyOls.GetStatus() == (uint)OpenLibSys.Ols.Status.NO_ERROR;


        }

        // 初始化 OpenHardwareMonitor 硬件监控
        private void InitializeOpenHardwareMonitor()
        {
            try
            {
                _openHardwareComputer = new OpenHwComputer
                {
                    HDDEnabled = true,    // 启用硬盘监控
                    CPUEnabled = false,
                    GPUEnabled = false,
                    MainboardEnabled = false,
                    FanControllerEnabled = false
                };

                _openHardwareComputer.Open();

                // 查找硬盘硬件
                foreach (var hardware in _openHardwareComputer.Hardware)
                {
                    if (hardware.HardwareType == OpenHwHardwareType.HDD)
                    {
                        _hddHardware = hardware;
                        break;
                    }
                }

                _openHwInitialized = true;
                LogEvent("OpenHardwareMonitor 初始化成功");
            }
            catch (Exception ex)
            {
                LogEvent($"OpenHardwareMonitor 初始化失败: {ex.Message}");
                _openHwInitialized = false;
            }
        }

        private static int SuperIo_Inw(byte data)
        {
            int val;
            MyOls.WriteIoPortByte(0x2e, data++);
            val = MyOls.ReadIoPortByte(0x2f) << 8;
            MyOls.WriteIoPortByte(0x2e, data);
            val |= MyOls.ReadIoPortByte(0x2f);
            return val;
        }

        public void InitSuperIO()
        {
            MyOls.WriteIoPortByte(0x2e, 0x87);
            MyOls.WriteIoPortByte(0x2e, 0x01);
            MyOls.WriteIoPortByte(0x2e, 0x55);
            MyOls.WriteIoPortByte(0x2e, 0x55);
        }

        public string GetChipName()
        {
            ushort chip_type = (ushort)SuperIo_Inw(0x20);
            return "IT" + Convert.ToString(chip_type, 16);
        }

        public void InitEc()
        {
            // 启用EC控制器
            MyOls.WriteIoPortByte(0x2e, 0x07);
            MyOls.WriteIoPortByte(0x2f, 0x04);
            MyOls.WriteIoPortByte(0x2e, 0x30);
            MyOls.WriteIoPortByte(0x2f, 0x01);

            // 获取EC基地址
            ushort ec_base = (ushort)SuperIo_Inw(0x60);
            ec_addr_port = (ushort)(ec_base + 0x05);
            ec_data_port = (ushort)(ec_base + 0x06);

            // 配置风扇控制
            MyOls.WriteIoPortByte(ec_addr_port, 0x0c);
            MyOls.WriteIoPortByte(ec_data_port, 0x00);

        }


        public int GetFanRpm()
        {
            // 读取风扇低位
            MyOls.WriteIoPortByte(ec_addr_port, 0x0d);
            byte lval = MyOls.ReadIoPortByte(ec_data_port);

            // 读取风扇高位
            MyOls.WriteIoPortByte(ec_addr_port, 0x18);
            byte mval = MyOls.ReadIoPortByte(ec_data_port);

            int fan_speed = (mval << 8) | lval;
            return (int)(1.35e6 / (fan_speed * 2));
        }

        public float GetCpuTemperature()
        {
            // IT8625 CPU温度寄存器是0x29
            MyOls.WriteIoPortByte(ec_addr_port, 0x29);
            byte rawTemp = MyOls.ReadIoPortByte(ec_data_port);
            return rawTemp;
        }

        public float GetMotherboardTemperature()
        {
            // IT8625 主板温度寄存器是0x2A
            MyOls.WriteIoPortByte(ec_addr_port, 0x2A);
            byte rawTemp = MyOls.ReadIoPortByte(ec_data_port);
            return rawTemp;
        }

        // 添加硬盘温度读取方法
        public float? GetHddTemperature()
        {
            if (!_openHwInitialized || _hddHardware == null)
                return null;

            try
            {
                // 更新硬件数据
                _hddHardware.Update();

                // 查找温度传感器
                foreach (var sensor in _hddHardware.Sensors)
                {
                    if (sensor.SensorType == OpenHwSensorType.Temperature)
                    {
                        _hddTemperatureSensor = sensor;
                        return (float?)sensor.Value;
                    }
                }

                // 如果没有找到温度传感器，尝试重新查找硬件
                foreach (var hardware in _openHardwareComputer.Hardware)
                {
                    if (hardware.HardwareType == OpenHwHardwareType.HDD)
                    {
                        hardware.Update();
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == OpenHwSensorType.Temperature)
                            {
                                _hddHardware = hardware;
                                _hddTemperatureSensor = sensor;
                                return (float?)sensor.Value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogEvent($"硬盘温度读取错误: {ex.Message}");
            }

            return null;
        }
   

        // 看门狗控制方法保持不变...
        public void openwatchdog() { /* 原有代码 */ }
        public void feedwatchdog(object sender, System.Timers.ElapsedEventArgs e) { /* 原有代码 */ }
        public void closewatchdog() { 
                        if (_openHardwareComputer != null)
            {
                _openHardwareComputer.Close();
                LogEvent("OpenHardwareMonitor 已关闭");
            }
            
            /* 原有代码 */ }

        // 日志辅助方法
        private static void LogEvent(string message)
        {
            try
            {
                string logDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Aiotoia_Watchdog");
                string logPath = Path.Combine(logDir, "watchdog_log3.txt");

                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                File.AppendAllText(logPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [HW] {message}\n");
            }
            catch { /* 忽略日志错误 */ }
        }
    }
}
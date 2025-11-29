using Microsoft.Win32;
using System;
using System.IO;

namespace WinRing0_Test
{
    public static class RegistryHelper
    {
        // 设置开机自启动
        public static bool SetAutoStart(bool enable)
        {
            try
            {
                const string appName = "AiotoiaWatchdog";
                string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                // 使用当前用户注册表项（不需要管理员权限）
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                    "Software\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (enable)
                    {
                        key.SetValue(appName, $"\"{appPath}\"");
                    }
                    else
                    {
                        key.DeleteValue(appName, false);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                // 记录错误
                LogError($"注册表操作失败: {ex.Message}");
                return false;
            }
        }

        // 检查是否已设置自启动
        public static bool IsAutoStartEnabled()
        {
            try
            {
                const string appName = "Aiotoia_Watchdog";

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                    "Software\\Microsoft\\Windows\\CurrentVersion\\Run", false))
                {
                    return key.GetValue(appName) != null;
                }
            }
            catch
            {
                return false;
            }
        }

        private static void LogError(string message)
        {
            try
            {
                string logDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Aiotoia_Watchdog");
                string logPath = Path.Combine(logDir, "registry_log.txt");

                File.AppendAllText(logPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [ERROR] {message}\n");
            }
            catch { /* 忽略错误 */ }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WpfApp1.Util;
using WpfApp1.Models;

namespace WpfApp1
{
    // GlobalPara 类用于保存全局配置信息、参数和静态资源
    public class GlobalPara
    {
        // 系统平台，存储当前的系统平台信息（例如 Windows、Linux 等）
        public static string SysPlattform = "";

        // 接收、发送、日志、显示相关的配置对象
        public static ReceivePara ReceivePara = new ReceivePara();
        public static SendPara SendPara = new SendPara();
        public static LogPara LogPara = new LogPara();
        public static DisplayPara DisplayPara = new DisplayPara();

        // 是否检查更新的标志
        public static bool IsCheckUpdate = true;

        // 定义了几个常用的画刷（颜色）
        public static readonly SolidColorBrush RedBrush = new SolidColorBrush(Colors.Red);  // 红色画刷
        public static readonly SolidColorBrush GreenBrush = new SolidColorBrush(Colors.Green);  // 绿色画刷
        public static readonly SolidColorBrush TransparentBrush = new SolidColorBrush(Colors.Transparent);  // 透明画刷

        // 用于存储配置对象的实例
        public static mycfg MyCfg = new mycfg();
        public static hiscfg HisCfg = new hiscfg();
        public static FastCmdsCfg FastCfg = new FastCmdsCfg();

        // 配置文件的文件名
        private static string _cfgfile = "my.cfg";
        private static string _hisfile = "his.cfg";
        private static string _cmdfile = "fastcmds.cfg";

        // 应用程序的路径和配置路径
        public static string AppPath = System.Windows.Forms.Application.StartupPath;
        public static string CfgPath = AppPath + "\\bycfg";
        public static string LogFolder = AppPath + "\\Log";

        /// <summary>
        /// 曲线显示参数（用于图表的显示配置）
        /// </summary>
        public static ChartParas ChartParas = new ChartParas();

        /// <summary>
        /// 是否正在显示图表
        /// </summary>
        public static bool IsShowChart { get; set; } = false;

        // 获取本地配置的方法
        public static void GetLocSet()
        {
            try
            {
                // 如果日志文件夹不存在，则创建
                if (!Directory.Exists(GlobalPara.LogFolder))
                {
                    Directory.CreateDirectory(GlobalPara.LogFolder);
                }

                // 获取配置文件的路径
                string path = Path.Combine(CfgPath, _cfgfile);

                // 如果配置文件存在，读取并反序列化为 MyCfg 对象
                if (File.Exists(path))
                {
                    MyCfg = JSONHelper.DeserializeJsonToObject<mycfg>(File.ReadAllText(path));
                    if (MyCfg != null)
                    {
                        // 设置日志文件名（以当前时间为文件名）
                        LogPara.FileName = Path.Combine(GlobalPara.LogFolder, DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt");

                        // 将显示参数中的颜色值设置为 MyCfg 中存储的颜色
                        var converter = new BrushConverter();
                        DisplayPara.FormatDisColor = MyCfg.FormatDisColor;
                        DisplayPara.SendColor = (SolidColorBrush)converter.ConvertFromString(MyCfg.SendColor);
                        DisplayPara.ReceiveColor = (SolidColorBrush)converter.ConvertFromString(MyCfg.RecColor);

                        // 设置是否检查更新
                        IsCheckUpdate = MyCfg.CheckUpdate;
                    }
                }

                // 读取历史配置文件
                path = Path.Combine(CfgPath, _hisfile);
                if (File.Exists(path))
                {
                    HisCfg = JSONHelper.DeserializeJsonToObject<hiscfg>(File.ReadAllText(path));
                }

                // 读取快速命令配置文件
                path = Path.Combine(CfgPath, _cmdfile);
                if (File.Exists(path))
                {
                    FastCfg = JSONHelper.DeserializeJsonToObject<FastCmdsCfg>(File.ReadAllText(path));
                }

            }
            catch (Exception ex)
            {
                // 异常捕获并抛出
                throw ex;
            }
        }

        // 保存当前配置的方法
        public static void SaveCurCfg()
        {
            try
            {
                // 如果配置文件夹不存在，则创建
                if (!Directory.Exists(CfgPath))
                {
                    Directory.CreateDirectory(CfgPath);
                }

                // 更新 MyCfg 配置中的值
                MyCfg.FormatDisColor = DisplayPara.FormatDisColor;
                MyCfg.SendColor = DisplayPara.SendColor.ToString();
                MyCfg.RecColor = DisplayPara.ReceiveColor.ToString();
                MyCfg.CheckUpdate = IsCheckUpdate;

                // 序列化 MyCfg 对象并写入到配置文件
                string txt = JSONHelper.SerializeObject(MyCfg);
                File.WriteAllText(Path.Combine(CfgPath, _cfgfile), txt);

                // 序列化 HisCfg 对象并写入到历史配置文件
                txt = JSONHelper.SerializeObject(HisCfg);
                File.WriteAllText(Path.Combine(CfgPath, _hisfile), txt);

                // 序列化 FastCfg 对象并写入到快速命令配置文件
                txt = JSONHelper.SerializeObject(FastCfg);
                File.WriteAllText(Path.Combine(CfgPath, _cmdfile), txt);
            }
            catch (Exception ex)
            {
                // 异常捕获并抛出
                throw ex;
            }
        }
    }
}

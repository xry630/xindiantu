using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WpfApp1.Base;

namespace WpfApp1.Models
{
    public class ReceivePara : NotificationObject
    {


        private Visibility _EncodingVisual = Visibility.Visible;
        /// <summary>
        /// TCP 字符串传输时，是否显示编码选项
        /// </summary>
        public Visibility EncodingVisual
        {
            get { return _EncodingVisual; }
            set
            {
                _EncodingVisual = value;
                RaisePropertyChanged("EncodingVisual");
            }
        }

        private Encoding _TextEncoding = Encoding.ASCII;
        /// <summary>
        /// TCP 文本传输时字符串编码格式
        /// </summary>
        public Encoding TextEncoding
        {
            get { return _TextEncoding; }
            set
            {
                _TextEncoding = value;
                RaisePropertyChanged("TextEncoding");
            }
        }
        private bool _IsHex = false;

        public bool IsHex
        {
            get => _IsHex;
            set
            {
                _IsHex = value;
                this.RaisePropertyChanged("IsHex");
            }
        }

        private bool _AutoFeed = true;

        public bool AutoFeed
        {
            get => _AutoFeed;
            set
            {
                _AutoFeed = value;
                this.RaisePropertyChanged("AutoFeed");
            }
        }


        private bool _DisplaySend = false;

        public bool DisplaySend
        {
            get => _DisplaySend;
            set
            {
                _DisplaySend = value;
                this.RaisePropertyChanged("DisplaySend");
            }
        }

        private bool _DisplayTime = false;

        public bool DisplayTime
        {
            get => _DisplayTime;
            set
            {
                _DisplayTime = value;
                this.RaisePropertyChanged("DisplayTime");
            }
        }

        private int _MinimalInterval = 500;

        public int MinimalInterval
        {
            get => _MinimalInterval;
            set
            {
                _MinimalInterval = value;
                this.RaisePropertyChanged("MinimalInterval");
            }
        }

        private string _TimeFormat = "[HH:mm:ss.fff] ";

        public string TimeFormat
        {
            get => _TimeFormat;
            set
            {
                _TimeFormat = value;
                TimeFormatTip = System.DateTime.Now.ToString(_TimeFormat);
                this.RaisePropertyChanged("TimeFormat");
            }
        }
        private string _TimeFormatTip = System.DateTime.Now.ToString("[HH:mm:ss.fff] ");

        public string TimeFormatTip
        {
            get => _TimeFormatTip;
            set
            {
                _TimeFormatTip = value;
                this.RaisePropertyChanged("TimeFormatTip");
            }
        }

    }
}

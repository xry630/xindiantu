using WpfApp1.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    // ChartParas 类继承自 NotificationObject，表示图表参数
    public class ChartParas : NotificationObject
    {
        // HexStartIndex：十六进制数据的起始索引，默认值为 0
        private int _HexStartIndex = 0;
        public int HexStartIndex
        {
            get { return _HexStartIndex; }
            set
            {
                _HexStartIndex = value;
                RaisePropertyChanged("HexStartIndex"); // 触发属性改变通知
            }
        }

        // HexDataTypeInex：十六进制数据类型的索引，默认值为 0
        private int _HexDataTypeInex = 0;
        public int HexDataTypeInex
        {
            get { return _HexDataTypeInex; }
            set
            {
                _HexDataTypeInex = value;
                RaisePropertyChanged("HexDataTypeInex"); // 触发属性改变通知
                HexDataType = (DataType)_HexDataTypeInex; // 根据索引设置 HexDataType 枚举值
            }
        }

        // HexDataByteOrderInex：十六进制数据字节顺序的索引，默认值为 0
        private int _HexDataByteOrderInex = 0;
        public int HexDataByteOrderInex
        {
            get { return _HexDataByteOrderInex; }
            set
            {
                _HexDataByteOrderInex = value;
                RaisePropertyChanged("HexDataByteOrderInex"); // 触发属性改变通知
                HexByteOrder = (ByteOrder)_HexDataByteOrderInex; // 根据索引设置 HexByteOrder 枚举值
            }
        }

        // AsciiStartIndex：ASCII 数据的起始索引，默认值为 0
        private int _AsciiStartIndex = 0;
        public int AsciiStartIndex
        {
            get { return _AsciiStartIndex; }
            set
            {
                _AsciiStartIndex = value;
                RaisePropertyChanged("AsciiStartIndex"); // 触发属性改变通知
            }
        }

        // AsciiLen：ASCII 数据的长度，默认值为 2
        private int _AsciiLen = 2;
        public int AsciiLen
        {
            get { return _AsciiLen; }
            set
            {
                _AsciiLen = value;
                RaisePropertyChanged("AsciiLen"); // 触发属性改变通知
            }
        }



        // HexDataType：十六进制数据类型，默认值为 UShort
        public DataType HexDataType { get; set; } = DataType.UShort;

        // HexByteOrder：字节顺序，默认值为 B12
        public ByteOrder HexByteOrder { get; set; } = ByteOrder.B12;

        /// <summary>
        /// 截取的数值的数据类型
        /// </summary>
        public enum DataType
        {
            UShort = 0,  // 无符号短整型
            UInt = 1,    // 无符号整型
            Short = 2,   // 短整型
            Int = 3,     // 整型
            Float = 4,   // 浮动点型
        }

        /// <summary>
        /// 字节顺序
        /// </summary>
        public enum ByteOrder
        {
            B12 = 0,    // 字节顺序 1 2
            B21 = 1,    // 字节顺序 2 1
            B1234 = 2,  // 字节顺序 1 2 3 4
            B3412 = 3,  // 字节顺序 3 4 1 2
            B4321 = 4,  // 字节顺序 4 3 2 1
        }


        // XTimeSpan：X 轴的时间跨度，单位：秒，默认值为 300
        private int _XTimeSpan = 300;
        /// <summary>
        /// X轴时间跨度，单位：秒
        /// </summary>
        public int XTimeSpan
        {
            get { return _XTimeSpan; }
            set
            {
                // 如果设置的值小于 20，重置为 300
                if (value < 20)
                {
                    _XTimeSpan = 300;
                }
                else
                {
                    _XTimeSpan = value;
                }
                RaisePropertyChanged(); // 触发属性改变通知
            }
        }

        // XStep：X 轴的步距，单位：秒，默认值为 50
        private int _XStep = 50;
        /// <summary>
        /// X轴步距，单位：秒
        /// </summary>
        public int XStep
        {
            get { return _XStep; }
            set
            {
                _XStep = value;
                RaisePropertyChanged(); // 触发属性改变通知
            }
        }

        // YStep：Y 轴的步距，单位：秒，默认值为 30
        private int _YStep = 30;
        /// <summary>
        /// Y轴步距，单位：秒
        /// </summary>
        public int YStep
        {
            get { return _YStep; }
            set
            {
                _YStep = value;
                RaisePropertyChanged(); // 触发属性改变通知
            }
        }

        // YMax：Y 轴的最大值，默认值为 100
        private double _YMax = 100;
        /// <summary>
        /// Y轴最大值
        /// </summary>
        public double YMax
        {
            get { return _YMax; }
            set
            {
                _YMax = value;
                RaisePropertyChanged(); // 触发属性改变通知
            }
        }

        // YMin：Y 轴的最小值，默认值为 0
        private double _YMin = 0;
        /// <summary>
        /// Y轴最小值
        /// </summary>
        public double YMin
        {
            get { return _YMin; }
            set
            {
                _YMin = value;
                RaisePropertyChanged(); // 触发属性改变通知
            }
        }
    }
}


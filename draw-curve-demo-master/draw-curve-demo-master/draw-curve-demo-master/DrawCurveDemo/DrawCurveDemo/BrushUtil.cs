using System.Windows.Media;

namespace DrawCurveDemo
{
	public class BrushUtil
	{
		public static SolidColorBrush ToBrush(string color, double? opacity = null)
		{
			var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
			if (opacity != null) brush.Opacity = opacity.Value;
			return brush;
		}
	}
}

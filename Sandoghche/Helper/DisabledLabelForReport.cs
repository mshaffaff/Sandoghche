using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sandoghche.Helpers
{
    public class DisabledLabelForReport : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] values = value.ToString().Split(new[] { "#" }, StringSplitOptions.None);
            bool isDeleted = System.Convert.ToBoolean(values[0]);
            bool isEdited = System.Convert.ToBoolean(values[1]);
            if (isDeleted && !isEdited)
                return Color.Red;
            else if (!isDeleted && isEdited)
                return Color.Green;
            else if (isDeleted && isEdited)
                return Color.DarkOrange;
            else
                return Color.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
            //throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;

namespace ScheduleCreate.Converters
{
    public class BoolToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEditMode && parameter is string titles)
            {
                var parts = titles.Split('|');
                if (parts.Length == 2)
                {
                    return isEditMode ? parts[0] : parts[1];
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 
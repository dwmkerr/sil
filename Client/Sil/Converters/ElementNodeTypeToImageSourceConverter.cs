using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Sil.Main;

namespace Sil.Converters
{
    public class ElementNodeTypeToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var elementNodeType = (ElementNodeType) value;
            switch (elementNodeType)
            {
                case ElementNodeType.Assembly:
                    return new BitmapImage(new Uri(@"pack://application:,,,/Resources/Icons/Assembly.png"));
                case ElementNodeType.Class:
                    return new BitmapImage(new Uri(@"pack://application:,,,/Resources/Icons/Class.png"));
                case ElementNodeType.Struct:
                    return new BitmapImage(new Uri(@"pack://application:,,,/Resources/Icons/Structure.png"));
                case ElementNodeType.Interface:
                    return new BitmapImage(new Uri(@"pack://application:,,,/Resources/Icons/Interface.png"));
                case ElementNodeType.Namespace:
                    return new BitmapImage(new Uri(@"pack://application:,,,/Resources/Icons/Assembly.png"));
                case ElementNodeType.Enum:
                    return new BitmapImage(new Uri(@"pack://application:,,,/Resources/Icons/Enum.png"));
                case ElementNodeType.Method:
                    return new BitmapImage(new Uri(@"pack://application:,,,/Resources/Icons/Method.png"));
                case ElementNodeType.Delegate:
                    return new BitmapImage(new Uri(@"pack://application:,,,/Resources/Icons/Delegate.png"));
                case ElementNodeType.Field:
                    return new BitmapImage(new Uri(@"pack://application:,,,/Resources/Icons/Field.png"));
                case ElementNodeType.Property:
                    return new BitmapImage(new Uri(@"pack://application:,,,/Resources/Icons/Property.png"));
                case ElementNodeType.Event:
                    return new BitmapImage(new Uri(@"pack://application:,,,/Resources/Icons/Event.png"));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

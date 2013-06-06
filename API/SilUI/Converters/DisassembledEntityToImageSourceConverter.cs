using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SilAPI;

namespace SilUI.Converters
{
    public class DisassembledEntityToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var disassembledEntity = (DisassembledEntity)value;
            
            if(disassembledEntity is DisassembledAssembly)
                return new BitmapImage(new Uri(@"pack://application:,,,/SilUI;component/Resources/Icons/Entities/Assembly.png"));
            if (disassembledEntity is DisassembledClass)
                return new BitmapImage(new Uri(@"pack://application:,,,/SilUI;component/Resources/Icons/Entities/Class.png"));
            if (disassembledEntity is DisassembledStructure)
                return new BitmapImage(new Uri(@"pack://application:,,,/SilUI;component/Resources/Icons/Entities/Structure.png"));
            if (disassembledEntity is DisassembledInterface)
                return new BitmapImage(new Uri(@"pack://application:,,,/SilUI;component/Resources/Icons/Entities/Interface.png"));
            if (disassembledEntity is DisassembledEnumeration)
                return new BitmapImage(new Uri(@"pack://application:,,,/SilUI;component/Resources/Icons/Entities/Enum.png"));
            if (disassembledEntity is DisassembledMethod)
                return new BitmapImage(new Uri(@"pack://application:,,,/SilUI;component/Resources/Icons/Entities/Method.png"));
            if (disassembledEntity is DisassembledDelegate)
                return new BitmapImage(new Uri(@"pack://application:,,,/SilUI;component/Resources/Icons/Entities/Delegate.png"));
            if (disassembledEntity is DisassembledField)
                return new BitmapImage(new Uri(@"pack://application:,,,/SilUI;component/Resources/Icons/Entities/Field.png"));
            if (disassembledEntity is DisassembledProperty)
                return new BitmapImage(new Uri(@"pack://application:,,,/SilUI;component/Resources/Icons/Entities/Property.png"));
            if (disassembledEntity is DisassembledEvent)
                return new BitmapImage(new Uri(@"pack://application:,,,/SilUI;component/Resources/Icons/Entities/Event.png"));

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

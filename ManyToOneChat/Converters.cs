using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ManyToOneChat.Converters
{
    public class ChatMessageIsFromClient : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine("Converter value: " + value.ToString());
            if (value.ToString() == "CLIENT")
            {
                Console.WriteLine("TRUE");
                return true;
            }
            else
            {
                Console.WriteLine("FALSE");
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
}

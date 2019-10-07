using DownLoader.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace DownLoader.Converters
{
    class TypeToTextConventer : IValueConverter
    {
        private static IDictionary<FileType, string> _descriptions = new Dictionary<FileType, string>
        {
            { FileType.Music, "Музыка" },
            { FileType.None, "Не выбрано" },
            { FileType.Picture, "Картинка" },
            { FileType.Program, "Программа" },
            { FileType.Video, "Видео" },
            { FileType.Zip, "Архив" }
        };

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is FileType))
                return null;
            var _enumval = Enum.GetValues(typeof(FileType)).Cast<FileType>();
            return _enumval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoscamCleanup
{
    class Camera3VideoGrouper : FileGrouper
    {
        public Camera3VideoGrouper() : base("Camera 3 Video")
        {
        }

        public override DateTime GetDateFromFileName(string fileName)
        {
            string dateString = Path.GetFileName(fileName).Substring(8, 8);
            DateTime date = DateTime.ParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture);
            return date;
        }
    }
}

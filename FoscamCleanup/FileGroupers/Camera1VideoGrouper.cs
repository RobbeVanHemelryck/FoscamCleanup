using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoscamCleanup
{
    class Camera1VideoGrouper : FileGrouper
    {
        public Camera1VideoGrouper() : base("Camera 1 Video")
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

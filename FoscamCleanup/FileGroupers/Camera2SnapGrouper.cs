using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoscamCleanup
{
    class Camera2SnapGrouper : FileGrouper
    {
        public Camera2SnapGrouper() : base("Camera 2 Snap")
        {
        }

        public override DateTime GetDateFromFileName(string fileName)
        {
            string dateString = Path.GetFileName(fileName).Substring(0, 8);
            DateTime date = DateTime.ParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture);
            return date;
        }
    }
}

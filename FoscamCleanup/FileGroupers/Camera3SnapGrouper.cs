using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoscamCleanup
{
    class Camera3SnapGrouper : FileGrouper
    {
        public Camera3SnapGrouper(): base("Camera 3 Snap")
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

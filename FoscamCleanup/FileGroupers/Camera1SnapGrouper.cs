using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoscamCleanup
{
    class Camera1SnapGrouper : FileGrouper
    {
        public Camera1SnapGrouper(): base("Camera 1 Snap")
        {
        }

        public override DateTime GetDateFromFileName(string fileName)
        {
            string dateString = Path.GetFileName(fileName).Substring(9, 8);
            DateTime date = DateTime.ParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture);
            return date;
        }

        public override bool IsRelevantFile(string fileName)
        {
            return Path.GetFileName(fileName).StartsWith("Schedule");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoscamCleanup
{
    abstract class FileGrouper
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Name { get; private set; }

        public FileGrouper()
        {
        }

        public FileGrouper(string name)
        {
            Name = name;
        }

        public bool AppliesTo(string name) => name == Name;
        public abstract DateTime GetDateFromFileName(string fileName);
        public virtual bool IsRelevantFile(string fileName) => true;
    }
}


/*
 * ,
  {
    "name": "Camera 1 Video",
    "source": "C:\\Users\\Robbe\\Desktop\\Foscam test - Copy\\Source\\Camera 1 Video",
    "destination": "C:\\Users\\Robbe\\Desktop\\Foscam test - Copy\\Destination\\Camera 1 Video"
  },
  {
    "name": "Camera 2 Snap",
    "source": "C:\\Users\\Robbe\\Desktop\\Foscam test - Copy\\Source\\Camera 2 Snap",
    "destination": "C:\\Users\\Robbe\\Desktop\\Foscam test - Copy\\Destination\\Camera 2 Snap"
  }
 * */

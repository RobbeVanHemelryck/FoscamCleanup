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
        public bool IsDecimatable { get; set; }

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
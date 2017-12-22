using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvDteSample
{
    internal class PackageConstants
    {
        private static Guid _outputWindowGuid = new Guid("927274A6-822C-4337-8BB8-32978FC7BB0E");
        public static Guid OutputWindowGuid
        {
            get { return _outputWindowGuid; }
        }

        public const string OutputWindowTitle = "Sample Output Window";
    }
}

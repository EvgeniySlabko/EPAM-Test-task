using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// String manager.
    /// </summary>
    public static class StringManager
    {
        /// <summary>
        /// Resource manager.
        /// </summary>
        public static readonly ResourceManager Rm = new (Path, Assembly.GetExecutingAssembly());

        private const string Path = "FileCabinetApp.Resource.Strings";
    }
}

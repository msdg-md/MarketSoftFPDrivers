namespace Phoenix.Devices.Printers.DatecsMD
{
    using System;
    using System.Reflection;
    using System.Resources;

    public class DatecsStrings
    {
        private static ResourceManager LocRM;

        static DatecsStrings()
        {
            string baseName = "Properties.DatecsStrings";
            LocRM = new ResourceManager(baseName, Assembly.GetExecutingAssembly());
        }

        public static string GetString(int msg) => 
            LocRM.GetString(msg.ToString());
    }
}


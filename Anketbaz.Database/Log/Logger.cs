using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace Anketbaz.Database.Log
{
    public static class Log
    {

        public static void Info(string e)
        {
            Console.WriteLine(e);
            WriteFile(e);
        }
        public static void Error(string e)
        {
            Console.WriteLine(e);
            WriteFile(e);
        }
        private static void WriteFile(string log)
        {
            {
                DateTime date = DateTime.Now;


                string dir = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\Logs";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    GrantAccess(dir);
                }
                string file = dir + "\\" + date.ToString("yyyyMMdd") + ".txt";
                if (!File.Exists(file))
                    File.Create(file).Dispose();

                StringBuilder stringB = new StringBuilder();
                stringB.AppendLine(string.Format("oOo ----------------------- oOo"));

                stringB.AppendLine(DateTime.Now.ToString("HH:mm:ss.fff"));
                stringB.AppendLine(log);
                File.AppendAllText(file, stringB.ToString());

            }

        }

        private static void GrantAccess(string fullPath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(fullPath);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        }
    }
}

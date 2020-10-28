// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
//using AMWE_RealTime_Server.Models;
//using System;
//using System.Linq;

//namespace AMWE_RealTime_Server
//{
//    public static class DbInitializer
//    {
//        public static void Initialize(VersionsContext context)
//        {
//            context.Database.EnsureCreated();

//            // Look for any students.
//            if (context.Versions.Any())
//            {
//                return;   // DB has been seeded
//            }

//            var versions = new Models.Version[]
//            {
//                new Models.Version()
//            };
//            foreach (Models.Version s in versions)
//            {
//                context.Versions.Add(s);
//            }
//            context.SaveChanges();
//        }
//    }
//}
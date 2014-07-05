using System;
using BuildManager.Data;
using System.Linq;


namespace BuildManager.Helpers
{
    public class EnlistmentDbOps
    {

        private const int smallDbTableCount = 2;

        public static void PopulateDbSmall(EnlistmentDC dc , bool submit)
        {

            BuildType bt = AddBuildTypeEntry(dc, submit, 1, "Full Build 1");
            AddBuildTypeEntry(dc, submit, 2, "Full Build 2");

            BuildXml bx = AddBuildXmlTypeEntry(dc, submit, 1, @"C:\FilePath1\Config.xml");
            AddBuildXmlTypeEntry(dc, submit, 2, @"C:\FilePath1\Config.xml");

            Command c = AddCommandTypeEntry(dc, submit, 1, "Rebuild", "Wm Rebuild");
            AddCommandTypeEntry(dc, submit, 2, "Build", "Wm Build");

            User u = AddUserTypeEntry ( dc , submit , 1 , "Dave" , "Hoover");
            AddUserTypeEntry ( dc , submit , 2 , "John" , "Wang");


            Email e = AddEmailTypeEntry(dc, submit, 1, 2, true, "johnwan@microsoft.com");
            AddEmailTypeEntry(dc, submit, 2, u, true, "dahoover@microsft.com");

            Enlistment1 en = AddEnlistmentTypeEntry(dc, submit, 1, @"F:\Enlistment1");
            AddEnlistmentTypeEntry(dc, submit, 2, @"F:\Enlistment2");

            PhoneSKU p = AddPhoneSkuTypeEntry(dc, submit, 1, "CEPC");
            AddPhoneSkuTypeEntry(dc, submit, 2, "XDE");

            Configuration cfg = AddConfigurationTypeEntry(dc, submit, 1, 2, 2, 2, 2, 2, 2, 2);
            AddConfigurationTypeEntry(dc, submit, 2 , u , e , p , c , bx , en , bt );

        }

        public static bool VerifySmallPopulation(EnlistmentDC dc)
        {
            int  cfgCount = (from c in dc.Configurations select c).Count();
            int btCount = (from c in dc.BuildTypes select c).Count();
            int bxCount = (from c in dc.BuildXmls select c).Count();
            int cmdCount = (from c in dc.Commands select c).Count();
            int userCount = (from c in dc.Users select c).Count();
            int emailCount = (from c in dc.Emails select c).Count();
            int skuCount = (from c in dc.PhoneSKUs select c).Count();
            int enlistCount = (from c in dc.Enlistments select c).Count();
            int s = smallDbTableCount;
            if (cfgCount != s || btCount != s || bxCount != s || cmdCount != s ||
                userCount != s || emailCount != s || skuCount != s || enlistCount != s)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool ModifySmallPopulationEntry(EnlistmentDC dc)
        {
            var cfg = from c in dc.Configurations
                      where c.PhoneSKU.Name == "CEPC"
                      select c;
            foreach ( var x in cfg)
            {
                x.PhoneSKU.Name = "CEPC1";
            }
            dc.SubmitChanges();

            var mod = from c in dc.Configurations
                      where c.PhoneSKU.Name == "CEPC1"
                      select c;

            if (mod != null && mod.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool DeleteEntriesFromSmallPopulationEntry(EnlistmentDC dc)
        {
            var cfg = from c in dc.Configurations
                      where c.PhoneSKU.Name == "CEPC"
                      select c;
            dc.Configurations.DeleteAllOnSubmit(cfg);
            dc.SubmitChanges();

            var mod = from c in dc.Configurations
                      where c.PhoneSKU.Name == "CEPC"
                      select c;

            if (mod != null && mod.Count() > 0)
            {
                return false;
            }
            else
            {
                return true;
            }


          

        }



        public static BuildType AddBuildTypeEntry(EnlistmentDC dc, bool submit , short id, string name  )
        {
            BuildType o = new BuildType { ID = id, Name = name };
            dc.BuildTypes.InsertOnSubmit(o);
            if (submit)
            { 
            dc.SubmitChanges();
            }
            return o;
        }

        public static BuildXml AddBuildXmlTypeEntry(EnlistmentDC dc, bool submit, short id, string filePath)
        {
            BuildXml o = new BuildXml { FileID = id, FilePath = filePath };
            dc.BuildXmls.InsertOnSubmit(o);
            if (submit)
            {
                dc.SubmitChanges();
            }
            return o;
        }

        public static Command AddCommandTypeEntry(EnlistmentDC dc, bool submit, short id, string command, string commandLine)
        {
            Command o = new Command { CommandID = id, Command1 = command, CommandLine = commandLine };
            dc.Commands.InsertOnSubmit(o);
            if (submit)
            {
                dc.SubmitChanges();
            }
            return o;
        }

        public static Email AddEmailTypeEntry(EnlistmentDC dc, bool submit, short emailID, short userid, bool primaryAccount, string emailAccount)
        {
            Email o = new Email { EmailID = emailID, EmailAccount = emailAccount, Primary = primaryAccount, UserID = (short)userid };
            dc.Emails.InsertOnSubmit(o);
            if (submit)
            {
                dc.SubmitChanges();
            }
            return o;
        }

        public static Email AddEmailTypeEntry(EnlistmentDC dc, bool submit, short emailID, User u, bool primaryAccount, string emailAccount)
        {
            Email o = new Email { EmailID = emailID, EmailAccount = emailAccount, Primary = primaryAccount , User = u};
            dc.Emails.InsertOnSubmit(o);
            if (submit)
            {
                dc.SubmitChanges();
            }
            return o;
        }



        public static Enlistment1 AddEnlistmentTypeEntry(EnlistmentDC dc, bool submit, short id, string path)
        {
            Enlistment1 o = new Enlistment1 { Id = id, Path = path };
            dc.Enlistments.InsertOnSubmit(o);
            if (submit)
            {
                dc.SubmitChanges();
            }
            return o;
        }

        public static PhoneSKU AddPhoneSkuTypeEntry(EnlistmentDC dc, bool submit, short id, string name)
        {
            PhoneSKU o = new PhoneSKU { ID = id, Name = name};
            dc.PhoneSKUs.InsertOnSubmit(o);
            if (submit)
            {
                dc.SubmitChanges();
            }
            return o;
        }

        public static User AddUserTypeEntry(EnlistmentDC dc, bool submit, short id, string firstName, string lastName)
        {
            User o = new User { UserID = id , FirstName = firstName , LastName = lastName};
            dc.Users.InsertOnSubmit(o);
            if (submit)
            {
                dc.SubmitChanges();
            }
            return o;
        }

        public static Configuration AddConfigurationTypeEntry(
            EnlistmentDC dc,
            bool submit,
            short id,
            short userId,
            short emailId,
            short phoneSkuId,
            short commandId,
            short buildXmlId,
            short enlistmentId,
            short dailyBuild)
        {
            Configuration o = new Configuration { ID = id, UserID = userId, 
                EmailID = emailId, PhoneSkuID = phoneSkuId, CommandID = commandId, 
                BuildXmlID = buildXmlId, EnlistmentID = enlistmentId, DailyBuild = dailyBuild };
            dc.Configurations.InsertOnSubmit(o);
            if (submit)
            {
                dc.SubmitChanges();
            }
            return o;
        }

         public static Configuration AddConfigurationTypeEntry(
            EnlistmentDC dc,
            bool submit,
             short id,
            User u ,
            Email e ,
            PhoneSKU p ,
            Command c ,
            BuildXml b ,
            Enlistment1 en ,
            BuildType bt )
        {
            Configuration o = new Configuration
            {
                ID = id,
                User = u ,
                Email = e ,
                PhoneSKU = p ,
                Command = c ,
                BuildXml = b ,
                Enlistment1 = en ,
                BuildType = bt 
            };
            dc.Configurations.InsertOnSubmit(o);
            if (submit)
            {
                dc.SubmitChanges();
            }
            return o;


        }


    }
}

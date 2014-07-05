using System;
using System.Collections.Generic;
using System.IO;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {

            string comsosPath   = @"cosmos://cosmos11-prod-cy2/vol13/vc/location.exec/local/Orion/Prod/Nokia/ObsAccept/2014/03/ObsAccept_2014_03_12.log_bucket0";
            string comsosStream = @"http://be.cosmos11.osdinfra.net:88/cosmos/location.exec/local/Orion/Prod/Nokia/ObsAccept/2014/03/ObsAccept_2014_03_12.log_bucket0";

            string cosmosPathName  = Path.GetFileName(comsosPath);
            string cosmosStreamName = Path.GetFileName(comsosStream);



            string path1 = @"d:\archives\";
            string path2 = "2001";
            string path3 = "media";
            string path4 = "images";
            string combinedPath = Path.Combine(path1, path2, path3, path4);
            Console.WriteLine(combinedPath);

        }
    }
}

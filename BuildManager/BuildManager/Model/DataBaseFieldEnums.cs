using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildManager.Model
{
    public class PhoneSkuInfo
    {
        // Temporary work around to identify Apollo SKUs verses Win7 Skus.
        public const Int32 ApolloSku = 10;

        public const string E600Sku = "E600 ARMV7 Release";

        public const string XdeSku = "Windows Phone Emulator x86 Release";

        public const string ApolloDeviceFreSku = "device x86fre";

        public const string ApolloDeviceChkSku = "device x86chk";

        public const string ApolloDeviceArmFreSku = "device armfre";

        public const string ApolloDeviceArmChkSku = "device armchk";


        public static string GetApolloBuildSkuTypeForLogFiles ( string phoneSku)
        {
            string skutype = "x86fre";
            if (!String.IsNullOrEmpty(phoneSku))
            {
                string[] skuInfo = phoneSku.Split(' ');
                if (skuInfo.Length == 2)
                {
                    skutype = skuInfo[1];
                }
            }
            return skutype;
        }
    
    }

    public enum DbCommand
    {
        Sync ,
        RebuildNoTest ,
        RebuildAll ,
        Clean ,
        Clean_Sync_Rebuild,
        Clean_Sync_RebuildNoTest,
        WpSync,
        WpRebuildNoTest,
        WpRebuildAll,
        WpClean_Sync_Rebuild,
        WpClean_Sync_RebuildNoTest

    }

    public enum DbBuildType
    {
        None ,
        Daily ,
        Weekly
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuildManager.Data;

namespace BuildManager.Helpers
{
    

    /// <summary>
    /// Comparer class for a XElement values.
    /// Required for the Distinct and Except method queries used with Linq
    /// </summary>
    internal class ConfigurationIdValueCompare : IEqualityComparer<Configuration>
    {

        /// <summary>
        /// Required interface member
        /// </summary>
        /// <param name="x">1st object</param>
        /// <param name="y">2nd object</param>
        /// <returns>true if the objects are equal</returns>
        public bool Equals(Configuration x, Configuration y)
        {

            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;
            return x.ID == y.ID;
        }

        /// <summary>
        /// Required interface member
        /// </summary>
        /// <param name="obj">System parameter</param>
        /// <returns>hashcode</returns>
        public int GetHashCode(Configuration obj)
        {

            //Check whether the object is null
            if (Object.ReferenceEquals(obj, null)) return 0;
            //Get hash code for the Value field.
            int hashNodeValue = obj.ID.GetHashCode();
            return hashNodeValue;
        }
    }

}

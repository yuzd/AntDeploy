using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace AntDeployAgent.Util
{
    public static class CodingHelper
    {
        
        public static T JsonToObject<T>(this string str)
        {
            try
            {
                var resultModel = JsonConvert.DeserializeObject<T>(str);
                return resultModel;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

    }
}

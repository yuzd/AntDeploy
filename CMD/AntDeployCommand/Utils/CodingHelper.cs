using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AntDeployCommand.Utils
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

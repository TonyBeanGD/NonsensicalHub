using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NonsensicalKit
{
    public class ReflectionHelper
    {
        /// <summary>
        /// 根据class name反射获取Type
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetTypeByTypeName(string typeName)
        {
            Assembly assembly=Assembly.GetExecutingAssembly();
            Type type = assembly.GetTypes().FirstOrDefault(t => t.Name == typeName);
            if (type==null)
            {
                throw new Exception(string.Format("Cant't find Class by class name:'{0}'",typeName));
            }
            return type;
        }
    }
}

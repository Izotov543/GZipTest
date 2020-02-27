using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest.Utils
{
    [AttributeUsage(AttributeTargets.Method)]
    class MethodNameTranslationAttribute : Attribute
    {
        public string MethodNameTranslation { get; set; }
        public MethodNameTranslationAttribute(string methodNameTranslation)
        {
            this.MethodNameTranslation = methodNameTranslation;
        }
    }
}

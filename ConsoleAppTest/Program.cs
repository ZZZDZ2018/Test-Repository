using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetAADaccesstoken;
namespace ConsoleAppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string token = GetAAdAccesstoken.GetStrToken().Result;
        }
    }
}

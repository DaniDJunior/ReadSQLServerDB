using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ReadSQLServerDB
{
    class Program
    {
        static void Main(string[] args)
        {

            //args = new string[1];
            //args[0] = "connection string de teste";
            if (args.Length == 0)
                return;
            

            var tabels = OpenDB.OpenDataBase(args[0]);

            string json = JsonConvert.SerializeObject(tabels, Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            if (args.Contains("-f"))
            {
                int cont = 0;
                while(args[cont] != "-f")
                {
                    cont++;
                }
                StreamWriter stream = new StreamWriter(args[cont + 1]);
                stream.Write(json);
                stream.Close();
            }

            if (!args.Contains("-v"))
            {
                Console.WriteLine(json);
            }

        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHH_Bot
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new AhhBot().MainAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} | Error @ Start | {ex.Message})");
            }

            Console.ReadKey();
        }
    }
}

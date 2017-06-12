using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalixtaChatBackupDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            string client = string.Empty;
            string token = string.Empty;



            if (args == null)
            {
                Console.WriteLine("Not enough params");
                Console.WriteLine("usage CalixtaChatBackupDownloader client token folder"); 
            } else
            {

            }
            Console.WriteLine("Download backups from calixta chat");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMPGrabber
{
    class Program
    {
        static void Main(string[] args)
        {
            var rmpGrabber = new RMPGrabber("rick", "sanchez");
            Console.WriteLine(rmpGrabber.GetOverallQualityRating());
        }
    }
}

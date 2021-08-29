using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpreterInterface
{
    public class Command
    {
        public string name { get; set; }
        public int flag { get; set; }
        public string order { get; set; }
        public string value { get; set; }

        public Command()
        {
            flag = 0;
            value = null;
            order = null;
        }
    }

    public class Position
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
        public double i { get; set; }
        public double j { get; set; }
        public double k { get; set; }

        public Position()
        {
            x = 0;
            y = 0;
            z = 0;
            i = 0;
            j = 0;
            k = 0;
        }
    }


    public class InterpolationCommand : Command
    {
        public string workPlane { get; set; }
        public string compensationMode { get; set; }
        public Position position { get; set; }
    }
}

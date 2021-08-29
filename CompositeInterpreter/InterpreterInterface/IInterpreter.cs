using System;
using System.Collections.Generic;

namespace InterpreterInterface
{
    public interface IInterpreter
    {
        public Dictionary<string, Command> modalTable { get; set; }
        public Dictionary<string, Command> nonModalTable { get; set; }

        public Position LastPosition { get; set; }

        public List<string> Errors { get;}

        public List<string> Warns { get;}
        public abstract void Interpret(string program, ref int nodeId);

    }
}

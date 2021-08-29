using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterpreterInterface;

namespace BasicInterpreter
{
    public class SemanticAnalysis
    {
        private readonly List<TreeNode> nodes;
        private Dictionary<string, Command> modalTable;
        private Dictionary<string, Command> nonModalTable;
        private List<string> errors;
        private List<string> warns;
        private List<Position> positions;
        private List<Command> motionCommands;

        private List<string> modalCommands = new List<string>()
        {
            "Interpolation","Diemensions","Tool Selection",
            "Spindle Speed","Feed Units","Feed Value","Settable Zero Offset",
            "Working Plane","Units","Compensation Mode","Compensation Selection"
        };
        private List<string> nonModalCommands = new List<string>()
        {
            "Suppression Zero Offset"
        };
        private List<string> motions = new List<string>()
        {
            "Change Tool","Spindle Rotation","Spindle Stop","Coolant Control","Target Position"
        };
        public List<string> Errors { get { return errors; } }
        public List<string> Warns { get { return warns; } }
        public SemanticAnalysis(List<TreeNode> nodes, ref Dictionary<string, Command> modalTable, 
            ref Dictionary<string,Command> nonModalTable, ref Position lastPos)
        {
            errors = new List<string>();
            warns = new List<string>();
            this.nodes = nodes;
            this.modalTable = modalTable;
            this.nonModalTable = nonModalTable;
            positions = new List<Position>() { lastPos };
            motionCommands = new List<Command>();
            string order;

            foreach(var node in nodes)
            {
                order = node.childNodes[0].childNodes[0].name;
            }
        }

        // node is basic block
        private void UpdateTables(TreeNode node, string order)
        {
            List<Command> motionCommandsTemp = new List<Command>();
            foreach (var child in node.childNodes)
            {
                Command _command;
                if (modalCommands.Contains(child.name))
                {
                    _command = new Command()
                    {
                        name = child.name,
                        order = order,
                        flag = 1,
                        value = child.childNodes[0].name
                    };
                    if (modalTable.Keys.Contains(_command.name))
                    {
                        modalTable[_command.name] = _command;
                    }
                    else
                    {
                        modalTable.Add(_command.name, _command);
                    }
                }
                else if(motions.Contains(child.name))
                {
                    if(child.name=="Target Position")
                    {
                        if (!modalTable.Keys.Contains("Interpolation"))
                        {
                            errors.Add("No interpolation command");
                            break;
                        }
                        else
                        {
                            Position newPos = positions[positions.Count - 1];
                            if (modalTable.Keys.Contains("Dimensions") && modalTable["Dimensions"].value == "G91")
                            {
                                if (modalTable["Interpolation"].value.Contains("2") || modalTable["Interpolation"].value.Contains("3"))
                                {
                                    foreach (var pos in child.childNodes[0].childNodes)
                                    {
                                        if (pos.name.ToLower() == "x")
                                            newPos.x = newPos.x + Convert.ToDouble(pos.childNodes[0].name);
                                        else if (pos.name.ToLower() == "y")
                                            newPos.y = newPos.y + Convert.ToDouble(pos.childNodes[0].name);
                                        else
                                            newPos.z = newPos.z + Convert.ToDouble(pos.childNodes[0].name);
                                    }
                                    foreach (var pos in child.childNodes[1].childNodes)
                                    {
                                        if (pos.name.ToLower() == "i")
                                            newPos.i = Convert.ToDouble(pos.childNodes[0].name);
                                        else if (pos.name.ToLower() == "j")
                                            newPos.j = Convert.ToDouble(pos.childNodes[0].name);
                                        else if (pos.name.ToLower() == "k")
                                            newPos.k = Convert.ToDouble(pos.childNodes[0].name);
                                        else
                                            throw new NotImplementedException();
                                    }
                                }
                                else
                                {
                                    foreach(var pos in child.childNodes)
                                    {
                                        if (pos.name.ToLower() == "x")
                                            newPos.x = newPos.x + Convert.ToDouble(pos.childNodes[0].name);
                                        else if (pos.name.ToLower() == "y")
                                            newPos.y = newPos.y + Convert.ToDouble(pos.childNodes[0].name);
                                        else
                                            newPos.z = newPos.z + Convert.ToDouble(pos.childNodes[0].name);
                                    }
                                }
                            }
                            else
                            {
                                if (modalTable["Interpolation"].value.Contains("2") || modalTable["Interpolation"].value.Contains("3"))
                                {
                                    foreach (var pos in child.childNodes[0].childNodes)
                                    {
                                        if (pos.name.ToLower() == "x")
                                            newPos.x = Convert.ToDouble(pos.childNodes[0].name);
                                        else if (pos.name.ToLower() == "y")
                                            newPos.y = Convert.ToDouble(pos.childNodes[0].name);
                                        else
                                            newPos.z = Convert.ToDouble(pos.childNodes[0].name);
                                    }
                                    foreach (var pos in child.childNodes[1].childNodes)
                                    {
                                        if (pos.name.ToLower() == "i")
                                            newPos.i = Convert.ToDouble(pos.childNodes[0].name);
                                        else if (pos.name.ToLower() == "j")
                                            newPos.j = Convert.ToDouble(pos.childNodes[0].name);
                                        else if (pos.name.ToLower() == "k")
                                            newPos.k = Convert.ToDouble(pos.childNodes[0].name);
                                        else
                                            throw new NotImplementedException();
                                    }
                                }
                                else
                                {
                                    foreach(var pos in child.childNodes)
                                    {
                                        if (pos.name.ToLower() == "x")
                                            newPos.x = Convert.ToDouble(pos.childNodes[0].name);
                                        else if (pos.name.ToLower() == "y")
                                            newPos.y = Convert.ToDouble(pos.childNodes[0].name);
                                        else
                                            newPos.z = Convert.ToDouble(pos.childNodes[0].name);
                                    }
                                    newPos.i = 0;
                                    newPos.j = 0;
                                    newPos.k = 0;
                                }
                            }
                            positions.Add(newPos);
                            _command = new Command()
                            {
                                name = "interpolation",
                                order = order,
                                flag = 1
                            };
                            motionCommandsTemp.Add(_command);
                        }
                    }
                    else
                    {
                        _command = new Command()
                        {
                            name = child.name,
                            order = order,
                            value = child.childNodes[0].name,
                            flag = 1
                        };
                        motionCommandsTemp.Add(_command);
                    }
                }
                else
                {

                }

                if (node.childNodes.IndexOf(child) == node.childNodes.Count - 1)
                {
                    WriteMotionCommand(motionCommandsTemp);
                    motionCommandsTemp.Clear();
                }
            }
        }

        private void WriteMotionCommand(List<Command>commands)
        {
            Command command;
            foreach(var item in motionCommands)
            {
                switch (item.name)
                {
                    case "interpolation":
                        command = new InterpolationCommand()
                        {
                            name = modalTable["Interpolation"].value,
                            order = item.order,
                            value = modalTable["Feed Value"].value,
                            workPlane = modalTable["Working Plane"].value,
                            compensationMode = modalTable["Compensation Mode"].value,
                            position = positions[positions.Count - 1]
                        };
                        break;
                    case "Spindle Rotation":
                        command = new Command()
                        {
                            name = item.value,
                            order = item.order,
                            value = modalTable["Spindle Speed"].value,
                        };
                        break;
                    case "Spindle Stop":
                        command = new Command()
                        {
                            name = item.value,
                            order = item.order,
                            value = "0"
                        };
                        break;
                    case "Change Tool":
                        command = new Command()
                        {
                            name = item.value,
                            order = item.order,
                            value = modalTable["Tool Selection"].value
                        };
                        break;
                    default:
                        command = new Command()
                        {
                            name = item.value,
                            order = item.order,
                        };
                        break;
                }
                motionCommands.Add(command);
                
            }
        }

    }
}

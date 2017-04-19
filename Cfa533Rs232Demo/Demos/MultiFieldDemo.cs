using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CommandLine;
using Newtonsoft.Json;
using Petrsnd.Cfa533Rs232Driver;
using Serilog;

namespace Petrsnd.Cfa533Rs232Demo.Demos
{
    internal class Field
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
    
    [Verb("multifield", HelpText = "Multiple field display, selectable with up and down arrows.")]
    internal class MultiFieldOptions
    {
        [Option(HelpText = "File containing JSON specification of fields")]
        public string File { get; set; }
    }

    internal static class MultiFieldDemo
    {
        internal class FieldDisplayer
        {
            private readonly LcdDevice _device;
            private readonly Field[] _fields;
            private int _index;

            public FieldDisplayer(LcdDevice device, Field[] fields)
            {
                _device = device;
                _fields = fields;
            }

            private void DisplayField()
            {
                Thread.Sleep(250);
                Log.Debug("Writing Index={Index}, Name={Name}, Value={Value}", _index, _fields[_index].Name,
                    _fields[_index].Value);
                _device.SetLcdContents(_fields[_index].Name, _fields[_index].Value);
            }

            public int Run()
            {
                try
                {
                    Console.WriteLine("Press any key to stop...");
                    EventHandler<KeypadActivityEventArgs> eventHandler =
                        delegate(object sender, KeypadActivityEventArgs eventArgs)
                        {
                            var updateDisplay = false;
                            switch (eventArgs.KeypadAction)
                            {
                                case KeypadAction.DownKeyRelease:
                                    _index++;
                                    Log.Debug("Increment");
                                    updateDisplay = true;
                                    break;
                                case KeypadAction.UpKeyRelease:
                                    _index--;
                                    Log.Debug("Decrement", _index);
                                    updateDisplay = true;
                                    break;
                            }
                            if (_index >= _fields.Length)
                                _index = 0;
                            if (_index < 0)
                                _index = _fields.Length - 1;
                            if (updateDisplay)
                            {
                                Log.Debug("Index={Index}", _index);
                                DisplayField();
                            }
                        };
                    _device.KeypadActivity += eventHandler;
                    DisplayField();
                    Console.ReadKey();
                    _device.KeypadActivity -= eventHandler;
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught in field displayer");
                    Console.WriteLine(ex);
                    return 1;
                }
            }
        }

        public static int Execute(LcdDevice device, MultiFieldOptions opts)
        {
            Field[] fields;
            if (!string.IsNullOrEmpty(opts.File))
            {
                if (!File.Exists(opts.File))
                    throw new ArgumentException("File does not exist!");
                var contents = File.ReadAllText(opts.File);
                fields = JsonConvert.DeserializeObject<List<Field>>(contents).ToArray();
                if (fields.Length < 1)
                    throw new ArgumentException("File didn't contain any field objects");
            }
            else
            {
                fields = new[]
                {
                    new Field {Name = "Version:", Value = "2.1.3.12334"},
                    new Field {Name = "Label1:", Value = "123.123.123.123"},
                    new Field {Name = "LabelTwo:", Value = "255.255.255.0"},
                    new Field {Name = "Long:", Value = "abcdefghijklmnopqrstuvwxyz"},
                    new Field {Name = "Empty:", Value = ""},
                };
            }
            return new FieldDisplayer(device, fields).Run();
        }
    }
}

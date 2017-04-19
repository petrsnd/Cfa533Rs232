using System;
using System.Collections.Generic;
using System.IO;
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
            private int _colIndex;

            public FieldDisplayer(LcdDevice device, Field[] fields)
            {
                _device = device;
                _fields = fields;
            }

            private string GetLineText(string text)
            {
                if (text.Length <= 16)
                    return text;
                var index = _colIndex;
                if (text.Length - _colIndex < 16)
                    index = text.Length - 16;
                text = text.Substring(index);
                if (text.Length > 16)
                    text = text.Substring(0, 15) + (char)0x7e;
                return text;
            }

            private void DisplayField()
            {
                if (_index >= _fields.Length)
                    _index = 0;
                if (_index < 0)
                    _index = _fields.Length - 1;
                var stringLength = Math.Max(_fields[_index].Name.Length, _fields[_index].Value.Length);
                if (_colIndex > (stringLength - 16))
                    _colIndex = 0;
                Log.Debug("Writing Index={Index}, Name={Name}, Value={Value}", _index, _fields[_index].Name,
                    _fields[_index].Value);
                _device.SetLcdContents(GetLineText(_fields[_index].Name), GetLineText(_fields[_index].Value));
            }

            public int Run()
            {
                try
                {
                    Console.WriteLine("Press any key to stop...");
                    EventHandler<KeypadActivityEventArgs> eventHandler =
                        delegate(object sender, KeypadActivityEventArgs eventArgs)
                        {
                            switch (eventArgs.KeypadAction)
                            {
                                case KeypadAction.DownKeyDown:
                                    _index++;
                                    _colIndex = 0;
                                    DisplayField();
                                    break;
                                case KeypadAction.UpKeyDown:
                                    _index--;
                                    _colIndex = 0;
                                    DisplayField();
                                    break;
                                case KeypadAction.RightKeyDown:
                                    _colIndex++;
                                    DisplayField();
                                    break;
                                case KeypadAction.LeftKeyDown:
                                    _colIndex--;
                                    DisplayField();
                                    break;
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
                    new Field {Name = "ReallyReallyReallyLongLabel:", Value = "abc"},
                    new Field {Name = "Suuuuuuuperrrrrlong:", Value = "abcdefghijklmnopqrstuvwxyz"},
                };
            }
            return new FieldDisplayer(device, fields).Run();
        }
    }
}

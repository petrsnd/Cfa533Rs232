namespace Petrsnd.Cfa533Rs232Driver
{
    public class KeypadActivityEventArgs
    {
        public KeypadActivityEventArgs(KeyFlags key, KeypadAction action)
        {
            Key = key;
            KeypadAction = action;
        }

        public KeyFlags Key { get; private set; }

        public KeypadAction KeypadAction { get; private set; }
    }
}

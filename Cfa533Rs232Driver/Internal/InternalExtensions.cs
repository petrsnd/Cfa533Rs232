namespace Petrsnd.Cfa533Rs232Driver.Internal
{
    internal static class InternalExtensions
    {
        public static KeyFlags ConvertToKeyFlags(this KeypadAction action)
        {
            switch (action)
            {
                case KeypadAction.UpKeyDown:
                case KeypadAction.UpKeyRelease:
                    return KeyFlags.Up;
                case KeypadAction.DownKeyDown:
                case KeypadAction.DownKeyRelease:
                    return KeyFlags.Down;
                case KeypadAction.LeftKeyDown:
                case KeypadAction.LeftKeyRelease:
                    return KeyFlags.Left;
                case KeypadAction.RightKeyDown:
                case KeypadAction.RightKeyRelease:
                    return KeyFlags.Right;
                case KeypadAction.EnterKeyDown:
                case KeypadAction.EnterKeyRelease:
                    return KeyFlags.Enter;
                case KeypadAction.CancelKeyDown:
                case KeypadAction.CancelKeyRelease:
                    return KeyFlags.Cancel;
                default:
                    return KeyFlags.Cancel;
            }
        }
    }
}

using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Tool
{
    internal interface IOp<in T>
    {
        int Run(LcdDevice device, T opts);
    }
}

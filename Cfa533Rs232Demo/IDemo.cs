using Petrsnd.Cfa533Rs232Driver;

namespace Petrsnd.Cfa533Rs232Demo
{
    internal interface IDemo<in T>
    {
        int Run(LcdDevice device, T opts);
    }
}

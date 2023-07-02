using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Interfaces
{
    public interface IHook
    {
        void Install();
        void Uninstall();
    }
}

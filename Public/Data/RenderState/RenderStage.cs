using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RGBLineCoreLib.Data
{
#if FOR_EDITOR
    [Flags]
    public enum RenderStageType : byte
    {
        Background = 0b0000_0001,
        Line = 0b0000_0010,
        Note = 0b0000_0100,
        Grid = 0b0000_1000
    }
#endif
}
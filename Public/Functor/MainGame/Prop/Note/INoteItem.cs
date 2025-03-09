using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RGBLineCoreLib
{
    public interface INoteItem : IDisposable
    {
        Guid NoteID { get; }

        void Render(in Guid noteID);
        float GetNoteXPos(in float targetFrame, in Guid attachedLineID);
    }
}
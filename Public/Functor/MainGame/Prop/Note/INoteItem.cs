using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib.Functor
{
    public interface INoteItem : IDisposable
    {
        Guid NoteID { get; }
        Transform Transform { get; }

        IRedAndBlueNote RedAndBlueNote { get; }
        IGreenNote GreenNote { get; }

        void Render(in Guid noteID);
        float GetNoteXPos(in float targetFrame, in Guid attachedLineID);
    }
}
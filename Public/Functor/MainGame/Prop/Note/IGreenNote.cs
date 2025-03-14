using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib.Functor
{
    public interface IGreenNote : IDisposable
    {
        Guid AttachedNoteID { get; }
        Transform Transform { get; }

        float CurveStartYPos { get; }

        void Render(in INoteItem noteItem);
    }
}
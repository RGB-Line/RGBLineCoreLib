using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using RGBLineCoreLib.Data;


namespace RGBLineCoreLib.Functor
{
    public interface IRedAndBlueNote : IDisposable
    {
        Guid AttachedNoteID { get; }
        Transform Transform { get; }

        void Render(in INoteItem noteItem, in StageData.NoteData.NoteType noteType);
    }
}
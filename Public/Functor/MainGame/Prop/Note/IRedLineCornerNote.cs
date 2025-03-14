using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib.Functor
{
    public interface IRedLineCornerNote : IDisposable
    {
        Guid NoteID { get; }
        Transform Transform { get; }

        void Render();
    }
}
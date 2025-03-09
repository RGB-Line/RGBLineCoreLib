using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib
{
    public interface IRedLineCornerNote : IDisposable
    {
        Transform Transform { get; }

        void Render();
    }
}
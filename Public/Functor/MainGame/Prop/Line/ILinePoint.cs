using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib
{
    public interface ILinePoint : IDisposable
    {
        Transform Transform { get; }

        void Render(in ILineItem parentLineItem, in Vector2 pos, in float minorOffsetTime, in int pointIndex);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib.Functor
{
    public interface ILineItem : IDisposable
    {
        Transform Transform { get; }
        LineRenderer LineRenderer { get; }
        CurvedLineRenderer CurvedLineRenderer { get; }

        Guid LineID { get; }

        void Render(in Guid lineID);
    }
}
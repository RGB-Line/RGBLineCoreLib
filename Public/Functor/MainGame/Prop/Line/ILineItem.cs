using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib
{
    public interface ILineItem : IDisposable
    {
        Transform Transform { get; }
        LineRenderer LineRenderer { get; }

        Guid LineID { get; }

        void Render(in Guid lineID);
    }
}
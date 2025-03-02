using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using CommonUtilLib.ThreadSafe;


namespace RGBLineCoreLib
{
    public sealed class LineManager : SingleTonForGameObject<LineManager>
    {
        public void Awake()
        {
            SetInstance(this);
        }

        internal void SpawnLineProps()
        {

        }
        internal void DespawnLineProps()
        {

        }

        protected override void Dispose(bool bisDisposing)
        {
            if (bisDisposing)
            {

            }
        }
    }
}
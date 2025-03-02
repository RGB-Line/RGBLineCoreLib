using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using CommonUtilLib.ThreadSafe;


namespace RGBLineCoreLib
{
    public sealed class NoteManager : SingleTonForGameObject<NoteManager>
    {
        public void Awake()
        {
            SetInstance(this);
        }

        internal void SpawnNoteProps()
        {

        }
        internal void DespawnNoteProps()
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
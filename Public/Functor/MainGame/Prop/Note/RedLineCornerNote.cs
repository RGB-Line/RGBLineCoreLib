using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib
{
    public class RedLineCornerNote : MonoBehaviour, IRedLineCornerNote
    {
        private Guid m_noteID = Guid.Empty;


        public Transform Transform
        {
            get
            {
                return transform;
            }
        }

        public void Render()
        {
            m_noteID = Guid.NewGuid();

            NoteManager.Instance.AddRedLineCornerNoteItem(m_noteID, this);
        }

        public void Dispose()
        {

        }
    }
}
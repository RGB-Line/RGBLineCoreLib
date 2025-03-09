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
        [SerializeField] private GameObject m_prefab_NoteItem;

        private Dictionary<Guid, INoteItem> m_noteItemTable = new Dictionary<Guid, INoteItem>();
        private Dictionary<Guid, IRedLineCornerNote> m_redLineCornerNoteTable = new Dictionary<Guid, IRedLineCornerNote>();


        public void Awake()
        {
            SetInstance(this);
        }

        public void SpawnNoteProps()
        {
            Guid[] noteIDs = StageDataInterface.NoteDataInterface.GetNoteIDs();

#if FOR_EDITOR
            foreach (Guid noteID in noteIDs)
            {
                if (m_noteItemTable.ContainsKey(noteID))
                {
                    m_noteItemTable[noteID].Dispose();
                }
            }
            m_noteItemTable.Clear();
#endif

            foreach (Guid noteID in noteIDs)
            {
                INoteItem noteItem = Instantiate(m_prefab_NoteItem, transform).GetComponent<INoteItem>();
                noteItem.Render(noteID);
                m_noteItemTable.Add(noteID, noteItem);
            }
        }
        public void DespawnNoteProps()
        {

        }

        public INoteItem GetNoteItem(in Guid targetNoteID)
        {
            return m_noteItemTable[targetNoteID];
        }

        internal void AddRedLineCornerNoteItem(in Guid noteID, in IRedLineCornerNote redLineCornerNoteItem)
        {
            m_redLineCornerNoteTable.Add(noteID, redLineCornerNoteItem);
        }
        internal void ClearRedLineCornerNoteItem()
        {
            m_redLineCornerNoteTable.Clear();
        }

        protected override void Dispose(bool bisDisposing)
        {
            if (bisDisposing)
            {

            }
        }
    }
}
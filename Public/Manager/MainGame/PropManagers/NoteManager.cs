using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using CommonUtilLib.ThreadSafe;

using RGBLineCoreLib.Functor;


namespace RGBLineCoreLib.Manager
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

        internal int RedLineCornerNoteCount
        {
            get
            {
                return m_redLineCornerNoteTable.Count;
            }
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
            // Despawn IRedLineCornerNotes
            foreach(IRedLineCornerNote redLineCornerNoteItem in m_redLineCornerNoteTable.Values)
            {
                redLineCornerNoteItem.Dispose();
                Destroy(redLineCornerNoteItem.Transform.gameObject);
            }
            m_redLineCornerNoteTable.Clear();

            // Despawn INoteItems
            foreach (INoteItem noteItem in m_noteItemTable.Values)
            {
                noteItem.Dispose();
                Destroy(noteItem.Transform.gameObject);
            }
            m_noteItemTable.Clear();
        }

        public INoteItem GetNoteItem(in Guid targetNoteID)
        {
            if(m_noteItemTable.ContainsKey(targetNoteID))
            {
                return m_noteItemTable[targetNoteID];
            }

            return null;
        }

        internal void AddRedLineCornerNoteItem(in Guid noteID, in IRedLineCornerNote redLineCornerNoteItem)
        {
            m_redLineCornerNoteTable.Add(noteID, redLineCornerNoteItem);
        }
        internal void ClearRedLineCornerNoteItem()
        {
            m_redLineCornerNoteTable.Clear();
        }

        internal IRedLineCornerNote GetRedLineCornerNote(in Guid targetNoteID)
        {
            if(m_redLineCornerNoteTable.ContainsKey(targetNoteID))
            {
                return m_redLineCornerNoteTable[targetNoteID];
            }

            return null;
        }

        protected override void Dispose(bool bisDisposing)
        {
            if (bisDisposing)
            {

            }
        }
    }
}
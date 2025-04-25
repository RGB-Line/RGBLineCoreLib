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
    /// <summary>
    /// 게임 내 Note를 관리하는 Manager Class
    /// </summary>
    public sealed class NoteManager : SingleTonForGameObject<NoteManager>
    {
        [SerializeField] private GameObject m_prefab_NoteItem = null;

        private readonly Dictionary<Guid, INoteItem> m_noteItemTable = new Dictionary<Guid, INoteItem>();
        private readonly Dictionary<Guid, IRedLineCornerNote> m_redLineCornerNoteTable = new Dictionary<Guid, IRedLineCornerNote>();


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

        /// <summary>
        /// 게임 내에 Note Item들을 생성할 때 호출하면 된다
        /// </summary>
        /// <remarks>
        /// Editor에서 Note Item들을 다시 그릴 경우에는 필히 먼저 DespawnNoteProps()을 호출하고, 그 다음에 SpawnNoteProps()을 호출해야 한다
        /// </remarks>
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
        /// <summary>
        /// 게임 내에 Note Item들을 제거할 때 호출하면 된다
        /// </summary>
        /// <remarks>
        /// Scene Unload 전에 필히 DespawnNoteProps() + GC.Collect()를 호출해 Memory 관리에 유의한다
        /// </remarks>
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

        /// <summary>
        /// 배치된 Note GameObject들 중에서 주어진 targetNoteID에 대응되는 INoteItem를 반환한다
        /// </summary>
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
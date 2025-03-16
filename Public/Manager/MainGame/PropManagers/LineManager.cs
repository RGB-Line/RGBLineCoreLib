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
    /// 게임 내 Line을 관리하는 Manager Class
    /// </summary>
    public sealed class LineManager : SingleTonForGameObject<LineManager>
    {
        [SerializeField] private GameObject m_prefab_LineItem;

        private Dictionary<Guid, ILineItem> m_lineItemTable = new Dictionary<Guid, ILineItem>();


        public void Awake()
        {
            SetInstance(this);
        }

        /// <summary>
        /// 게임 내에 Line Item들을 생성할 때 호출하면 된다
        /// </summary>
        /// <remarks>
        /// Editor에서 Line Item들을 다시 그릴 경우에는 필히 먼저 DespawnLineProps()을 호출하고, 그 다음에 SpawnLineProps()을 호출해야 한다
        /// </remarks>
        public void SpawnLineProps()
        {
            Guid[] lineIDs = StageDataInterface.LineDataInterface.GetLineIDs();

#if FOR_EDITOR
            foreach (Guid lineID in lineIDs)
            {
                if(m_lineItemTable.ContainsKey(lineID))
                {
                    m_lineItemTable[lineID].Dispose();
                }
            }
            m_lineItemTable.Clear();

            NoteManager.Instance.ClearRedLineCornerNoteItem();
#endif

            foreach (Guid lineID in lineIDs)
            {
                ILineItem lineItem = Instantiate(m_prefab_LineItem, transform).GetComponent<ILineItem>();
                lineItem.Render(lineID);
                m_lineItemTable.Add(lineID, lineItem);
            }
        }
        /// <summary>
        /// 게임 내에 Line Item들을 제거할 때 호출하면 된다
        /// </summary>
        /// <remarks>
        /// Scene Unload 전에 필히 DespawnLineProps() + GC.Collect()를 호출해 Memory 관리에 유의한다
        /// </remarks>
        public void DespawnLineProps()
        {
            foreach (ILineItem lineItem in m_lineItemTable.Values)
            {
                lineItem.Dispose();
            }
            m_lineItemTable.Clear();
        }

        /// <summary>
        /// 배치된 Line GameObject들 중에서 주어진 targetLineID에 대응되는 ILineItem를 반환한다
        /// </summary>
        public ILineItem GetLineItem(in Guid targetLineID)
        {
            return m_lineItemTable[targetLineID];
        }

        protected override void Dispose(bool bisDisposing)
        {
            if (bisDisposing)
            {

            }
        }
    }
}
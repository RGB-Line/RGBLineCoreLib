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
    public sealed class LineManager : SingleTonForGameObject<LineManager>
    {
        [SerializeField] private GameObject m_prefab_LineItem;

        private Dictionary<Guid, ILineItem> m_lineItemTable = new Dictionary<Guid, ILineItem>();


        public void Awake()
        {
            SetInstance(this);
        }

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
        public void DespawnLineProps()
        {
            foreach (ILineItem lineItem in m_lineItemTable.Values)
            {
                lineItem.Dispose();
            }
            m_lineItemTable.Clear();
        }

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
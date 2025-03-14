using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using CommonUtilLib.ThreadSafe;

using RGBLineCoreLib.Data;
using RGBLineCoreLib.Manager;


namespace RGBLineCoreLib.Functor
{
    public sealed class DeterminLine : SingleTonForGameObject<DeterminLine>
    {
        private bool m_bisStartCheck = false;


        public void Awake()
        {
            SetInstance(this);
        }
        public void FixedUpdate()
        {
            Guid curRegionID = RegionManager.Instance.GetRegionIDFromYPos(transform.position.y);
            if (curRegionID != Guid.Empty && StageDataInterface.RegionDataInterface.GetRegionData(curRegionID).CurColorType == StageData.RegionData.ColorType.Green)
            {
                ScoreManager.Instance.BIsGreenRegion = true;
            }
            else
            {
                ScoreManager.Instance.BIsGreenRegion = false;
            }
        }

        public void StartCheck()
        {
            m_bisStartCheck = true;
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if(!m_bisStartCheck)
            {
                return;
            }

            if (collision.GetComponent<IRedAndBlueNote>() != null)
            {
                ScoreManager.Instance.PushNoteCandidate(collision.GetComponent<IRedAndBlueNote>().AttachedNoteID);
            }
            if(collision.transform.parent.GetComponent<IGreenNote>() != null)
            {
                ScoreManager.Instance.PushNoteCandidate(collision.transform.parent.GetComponent<IGreenNote>().AttachedNoteID);
            }
            if(collision.GetComponent<IRedLineCornerNote>() != null)
            {
                ScoreManager.Instance.PushNoteCandidate(collision.GetComponent<IRedLineCornerNote>().NoteID);
            }
        }
        public void OnTriggerExit2D(Collider2D collision)
        {
            if(!m_bisStartCheck)
            {
                return;
            }

            if (collision.GetComponent<IRedAndBlueNote>() != null)
            {
                ScoreManager.Instance.RemoveNoteCandidate(collision.GetComponent<IRedAndBlueNote>().AttachedNoteID);
            }
            if (collision.transform.parent.GetComponent<IGreenNote>() != null)
            {
                ScoreManager.Instance.RemoveNoteCandidate(collision.transform.parent.GetComponent<IGreenNote>().AttachedNoteID);
            }
            if (collision.GetComponent<IRedLineCornerNote>() != null)
            {
                ScoreManager.Instance.RemoveNoteCandidate(collision.GetComponent<IRedLineCornerNote>().NoteID);
            }
        }

        protected override void Dispose(bool bisDisposing)
        {
            if(bisDisposing)
            {

            }
        }
    }
}
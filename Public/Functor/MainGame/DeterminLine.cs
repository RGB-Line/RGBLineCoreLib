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
    /// <summary>
    /// 게임 내 판정선 Class
    /// </summary>
    /// <remarks>
    /// Scene Load 시 DeterminLine GameObject의 경우 필히 StartCheck()를 호출해야 한다
    /// </remarks>
    public sealed class DeterminLine : SingleTonForGameObject<DeterminLine>
    {
        private bool m_bisStartCheck = false;


        public void Awake()
        {
            SetInstance(this);
        }
        public void Update()
        {
            if(!m_bisStartCheck)
            {
                return;
            }

            Guid curRegionID = RegionManager.Instance.GetRegionIDFromYPos(transform.position.y);
            if (curRegionID != Guid.Empty && StageDataInterface.RegionDataInterface.GetRegionData(curRegionID).CurColorType == StageData.RegionData.ColorType.Green)
            {
                ScoreManager.Instance.BIsGreenRegion = true;
            }
            else
            {
                ScoreManager.Instance.BIsGreenRegion = false;
            }

            if (curRegionID != Guid.Empty && StageDataInterface.RegionDataInterface.GetRegionData(curRegionID).CurColorType == StageData.RegionData.ColorType.Blue)
            {
                ScoreManager.Instance.BIsBlueRegion = true;
            }
            else
            {
                ScoreManager.Instance.BIsBlueRegion = false;
            }
        }

        /// <summary>
        /// Scene Load 시 DeterminLine GameObject의 경우 필히 StartCheck()를 호출해야 한다
        /// </summary>
        public void StartCheck()
        {
            m_bisStartCheck = true;
        }
        public void StopCheck()
        {
            m_bisStartCheck = false;
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
            if(collision.transform.parent.GetComponent<IGreenNote>() != null
                && collision.transform.parent.GetChild(0).transform == collision.transform)
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
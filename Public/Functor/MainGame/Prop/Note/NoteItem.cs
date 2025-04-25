using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using RGBLineCoreLib.Data;
using RGBLineCoreLib.Manager;


namespace RGBLineCoreLib.Functor
{
    /// <summary>
    /// 기본적인 Note를 그리기 위한 Class
    /// </summary>
    /// <remarks>
    /// Editor에서는 해당 Class를 상속받아 사용하지만, Runtime에서는 해당 Class를 직접 사용함
    /// </remarks>
    public class NoteItem : MonoBehaviour, INoteItem
    {
        [SerializeField] private Transform m_transform_RedAndBlueNote = null;
        [SerializeField] private Transform m_transform_GreenNote = null;

        private IRedAndBlueNote m_redAndBlueNote;
        private IGreenNote m_greenNote;

        private Guid m_noteID;


        public void Awake()
        {
            m_redAndBlueNote = m_transform_RedAndBlueNote.GetComponent<IRedAndBlueNote>();
            m_greenNote = m_transform_GreenNote.GetComponent<IGreenNote>();
        }

        public Guid NoteID
        {
            get
            {
                return m_noteID;
            }
        }
        public Transform Transform
        {
            get
            {
                return transform;
            }
        }

        public IRedAndBlueNote RedAndBlueNote
        {
            get
            {
                return m_redAndBlueNote;
            }
        }
        public IGreenNote GreenNote
        {
            get
            {
                return m_greenNote;
            }
        }

        public virtual void Render(in Guid noteID = default)
        {
            if (m_noteID == default && noteID == default)
            {
                throw new ArgumentException("LineID is not set.");
            }
            else if (noteID != default)
            {
                m_noteID = noteID;
            }

            switch (StageDataInterface.NoteDataInterface.GetNoteData(m_noteID).CurNoteType)
            {
                case StageData.NoteData.NoteType.Common:
                    m_redAndBlueNote.Render(this, StageData.NoteData.NoteType.Common);

                    m_greenNote.Transform.gameObject.SetActive(false);
                    break;

                case StageData.NoteData.NoteType.Double:
                    m_redAndBlueNote.Render(this, StageData.NoteData.NoteType.Double);

                    m_greenNote.Transform.gameObject.SetActive(false);
                    break;

                case StageData.NoteData.NoteType.Long:
                    m_greenNote.Render(this);

                    m_redAndBlueNote.Transform.gameObject.SetActive(false);
                    break;
            }
        }

        public float GetNoteXPos(in float targetFrame, in Guid attachedLineID)
        {
            float NoteYPos = GridManager.Instance.GetYPosFromFrame(targetFrame) +
                             GridManager.Instance.GetYPosFromFrame(StageDataInterface.LineDataInterface.GetAttachedRegionData(attachedLineID).StartOffsetFrame);

            List<int> nearestLinePosIndexes = new List<int>(2);
            LineRenderer attachedLineRenderer = LineManager.Instance.GetLineItem(attachedLineID).LineRenderer;

            // For Blue Line
            if (StageDataInterface.LineDataInterface.GetAttachedRegionData(attachedLineID).CurColorType == StageData.RegionData.ColorType.Blue)
            {
                return attachedLineRenderer.GetPosition(0).x;
            }

            for (int index = 0; index < attachedLineRenderer.positionCount; index++)
            {
                if (attachedLineRenderer.GetPosition(index).y == NoteYPos)
                {
                    return attachedLineRenderer.GetPosition(index).x;
                }
            }

            // Most nearest line pos
            int curNearestLinePosIndex = -1;
            float curNearestLinePos = float.MaxValue;
            for (int linePosIndex = 0; linePosIndex < attachedLineRenderer.positionCount; linePosIndex++)
            {
                float linePos = attachedLineRenderer.GetPosition(linePosIndex).y;
                if (linePos > NoteYPos && Mathf.Abs(linePos - NoteYPos) < Mathf.Abs(curNearestLinePos - NoteYPos))
                {
                    curNearestLinePosIndex = linePosIndex;
                    curNearestLinePos = linePos;
                }
            }
            nearestLinePosIndexes.Add(curNearestLinePosIndex);

            if (Mathf.Abs(attachedLineRenderer.GetPosition(nearestLinePosIndexes[0]).y - NoteYPos) == 0)
            {
                return attachedLineRenderer.GetPosition(curNearestLinePosIndex).x;
            }

            // Second nearest line pos
            curNearestLinePosIndex = -1;
            curNearestLinePos = float.MaxValue;
            for (int linePosIndex = 0; linePosIndex < attachedLineRenderer.positionCount; linePosIndex++)
            {
                if (nearestLinePosIndexes.Contains(linePosIndex))
                {
                    continue;
                }

                float linePos = attachedLineRenderer.GetPosition(linePosIndex).y;
                if (linePos < NoteYPos && Mathf.Abs(linePos - NoteYPos) < Mathf.Abs(curNearestLinePos - NoteYPos))
                {
                    curNearestLinePosIndex = linePosIndex;
                    curNearestLinePos = linePos;
                }
            }
            nearestLinePosIndexes.Add(curNearestLinePosIndex);

            // Mathf.Lerp를 사용하기 위한 준비
            float[] nearestLinePosGaps = new float[2];
            for (int index = 0; index < nearestLinePosIndexes.Count; index++)
            {
                nearestLinePosGaps[index] = Mathf.Abs(attachedLineRenderer.GetPosition(nearestLinePosIndexes[index]).y - NoteYPos);
            }

            return Mathf.Lerp(attachedLineRenderer.GetPosition(nearestLinePosIndexes[0]).x,
                              attachedLineRenderer.GetPosition(nearestLinePosIndexes[1]).x,
                              (nearestLinePosGaps[0] / (nearestLinePosGaps[0] + nearestLinePosGaps[1])));
        }

        public virtual void Dispose()
        {
            m_redAndBlueNote.Dispose();
            Destroy(m_redAndBlueNote.Transform.gameObject);

            m_greenNote.Dispose();
            Destroy(m_greenNote.Transform.gameObject);
        }
    }
}
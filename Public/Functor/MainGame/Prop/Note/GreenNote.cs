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
    /// 기본적인 Green Note를 그리기 위한 Class
    /// </summary>
    /// <remarks>
    /// Editor에서는 해당 Class를 상속받아 사용하지만, Runtime에서는 해당 Class를 직접 사용함
    /// </remarks>
    public class GreenNote : MonoBehaviour, IGreenNote
    {
        private INoteItem m_noteItem;

        private LineRenderer m_lineRenderer = null;

        [SerializeField] private BoxCollider2D m_startJudgeBox = null;
        [SerializeField] private BoxCollider2D m_endJudgeBox = null;


        public void Awake()
        {
            m_lineRenderer = GetComponent<LineRenderer>();
        }

        public Guid AttachedNoteID
        {
            get
            {
                return m_noteItem.NoteID;
            }
        }
        public Transform Transform
        {
            get
            {
                return transform;
            }
        }

        public float CurveStartYPos
        {
            get
            {
                return m_lineRenderer.GetPosition(0).y;
            }
        }

        public virtual void Render(in INoteItem noteItem)
        {
            m_noteItem = noteItem;

            Invoke("DrawGreenNote", 0.5f);
        }

        public virtual void Dispose()
        {
            m_noteItem = null;

            m_startJudgeBox.size = Vector2.one;
            m_endJudgeBox.size = Vector2.one;
        }

        protected virtual void DrawGreenNote()
        {
            List<Vector3> linePoses = new List<Vector3>();

            StageData.NoteData curNoteData = StageDataInterface.NoteDataInterface.GetNoteData(m_noteItem.NoteID);
            Guid attachedLineID = curNoteData.AttachedLineID;
            Vector3 startPos = new Vector3()
            {
                x = m_noteItem.GetNoteXPos(curNoteData.StartOffsetFrame, attachedLineID),
                y = GridManager.Instance.GetYPosFromFrame(StageDataInterface.NoteDataInterface.GetAttachedRegionData(m_noteItem.NoteID).StartOffsetFrame) +
                    GridManager.Instance.GetYPosFromFrame(StageDataInterface.NoteDataInterface.GetAttachedLineData(m_noteItem.NoteID).CurvedLinePoints[0].Y) +
                    GridManager.Instance.GetYPosFromFrame(curNoteData.StartOffsetFrame),
                z = -6.0f
            };
            Vector3 endPos = new Vector3()
            {
                x = m_noteItem.GetNoteXPos(curNoteData.StartOffsetFrame + curNoteData.NoteLength, attachedLineID),
                y = GridManager.Instance.GetYPosFromFrame(StageDataInterface.NoteDataInterface.GetAttachedRegionData(m_noteItem.NoteID).StartOffsetFrame) +
                    GridManager.Instance.GetYPosFromFrame(StageDataInterface.NoteDataInterface.GetAttachedLineData(m_noteItem.NoteID).CurvedLinePoints[0].Y) +
                    GridManager.Instance.GetYPosFromFrame(curNoteData.StartOffsetFrame + curNoteData.NoteLength),
                z = -6.0f
            };

            LineRenderer attachedLineRenderer = LineManager.Instance.GetLineItem(attachedLineID).LineRenderer;
            linePoses.Add(startPos);
            for (int linePosIndex = 0; linePosIndex < attachedLineRenderer.positionCount; linePosIndex++)
            {
                if (startPos.y < attachedLineRenderer.GetPosition(linePosIndex).y && attachedLineRenderer.GetPosition(linePosIndex).y < endPos.y)
                {
                    Vector3 pos = attachedLineRenderer.GetPosition(linePosIndex);
                    pos.z = -6.0f;
                    linePoses.Add(pos);
                }
            }
            linePoses.Add(endPos);

            m_lineRenderer.positionCount = linePoses.Count;
            m_lineRenderer.SetPositions(linePoses.ToArray());

            m_lineRenderer.numCapVertices = 90;

            m_startJudgeBox.transform.position = startPos;
            m_endJudgeBox.transform.position = endPos;

            StageData.StageConfigData curStageConfigData = StageDataInterface.StageConfigDataInterface.GetStageConfigData();
            StageMetadata stageMetadata = StageMetadataInterface.GetStageMetadata();
            float velocity = (GridManager.Instance.GetTotalFrameCount() * (curStageConfigData.LengthPerBit / curStageConfigData.BitSubDivision)) / stageMetadata.MusicLength;

            GameConfigData gameConfigData = GameConfigDataBuffer.Instance.ConfigData;
            m_startJudgeBox.size = new Vector2()
            {
                x = 1.0f,
                y = velocity * (gameConfigData.NoteHitJudgeStrandard.HitJudgingRanges[2] / 1000.0f) * 4.0f
            };
            m_endJudgeBox.size = new Vector2()
            {
                x = 1.0f,
                y = 0.1f
            };
        }
    }
}
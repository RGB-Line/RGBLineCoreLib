using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib
{
    public class GreenNote : MonoBehaviour, IGreenNote
    {
        private INoteItem m_noteItem;

        private LineRenderer m_lineRenderer = null;


        public void Awake()
        {
            m_lineRenderer = GetComponent<LineRenderer>();
        }

        public Transform Transform
        {
            get
            {
                return transform;
            }
        }

        public void Render(in INoteItem noteItem)
        {
            m_noteItem = noteItem;

            Invoke("DrawGreenNote", 0.5f);
        }

        private void DrawGreenNote()
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
            //m_lineRenderer.alignment = LineAlignment.TransformZ;

            //m_transform_StartJudgeBox.position = startPos;
            //m_transform_EndJudgeBox.position = endPos;

            //float velocity = (GridRenderManager.Instance.GetTotalBitCount() * (StageDataBuffer.Instance.CurStageData.Value.StageConfig.LengthPerBit / StageDataBuffer.Instance.CurStageData.Value.StageConfig.BitSubDivision)) / ChiefGameManager.Instance.CurMusicClip.length;
            //m_transform_StartJudgeBox.GetComponent<BoxCollider2D>().size = new Vector2()
            //{
            //    x = 1.0f,
            //    y = velocity * (GameConfigDataBuffer.Instance.GameConfigData.Value.noteHitJudgingStrandard.HitJudgingRanges[2] / 1000.0f) * 4.0f
            //};
            //m_transform_EndJudgeBox.GetComponent<BoxCollider2D>().size = new Vector2()
            //{
            //    x = 1.0f,
            //    y = velocity * (GameConfigDataBuffer.Instance.GameConfigData.Value.noteHitJudgingStrandard.HitJudgingRanges[2] / 1000.0f) * 4.0f
            //};

            //m_transform_StartJudgeBox.GetComponent<Editor_Note_GreenHitJudgeBox>().Init(NoteID);
            //m_transform_EndJudgeBox.GetComponent<Editor_Note_GreenHitJudgeBox>().Init(NoteID);

            //try
            //{
            //    Mesh mesh = new Mesh();
            //    m_lineRenderer.BakeMesh(mesh, true);
            //    m_meshCollider.sharedMesh = mesh;
            //}
            //catch
            //{
            //    Debug.Log("BakeMesh Error");
            //}
        }
    }
}
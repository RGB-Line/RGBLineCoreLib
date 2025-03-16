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
    /// 게임 내 Red, Green Line을 따라가는 Tracker Class
    /// </summary>
    public sealed class LineTracker : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_spriteRenderer;

        private Dictionary<Guid, float> m_regionTimeTable = new Dictionary<Guid, float>();

        private bool m_bisInitialized = false;

        private Guid m_curRegionID = Guid.Empty;


        public void Update()
        {
            if (!m_bisInitialized)
            {
                return;
            }
            
            if (m_curRegionID != Guid.Empty)
            {
                StageData.RegionData curRegionData = StageDataInterface.RegionDataInterface.GetRegionData(m_curRegionID);
                switch (curRegionData.CurColorType)
                {
                    case StageData.RegionData.ColorType.Red:
                        {
                            Guid redLineID = Guid.Empty;
                            Guid[] lineIDs = StageDataInterface.LineDataInterface.GetLineIDs();
                            foreach (Guid lineID in lineIDs)
                            {
                                StageData.LineData lineData = StageDataInterface.LineDataInterface.GetLineData(lineID);
                                if (lineData.AttachedRegionID == m_curRegionID)
                                {
                                    redLineID = lineID;
                                    break;
                                }
                            }

                            float xPos = GetCameraXPos(transform.position.y / GridManager.Instance.GetUnitFrameSize(), redLineID);
                            Camera.main.transform.position = new Vector3(xPos, Camera.main.transform.position.y, -10);

                            m_spriteRenderer.enabled = true;
                            transform.localPosition = new Vector3()
                            {
                                x = 0.0f,
                                y = -3.5f,
                                z = 5.0f
                            };
                        }
                        break;

                    case StageData.RegionData.ColorType.Green:
                        {
                            Guid greenLineID = Guid.Empty;
                            Guid[] lineIDs = StageDataInterface.LineDataInterface.GetLineIDs();
                            foreach (Guid lineID in lineIDs)
                            {
                                StageData.LineData lineData = StageDataInterface.LineDataInterface.GetLineData(lineID);
                                if (lineData.AttachedRegionID == m_curRegionID)
                                {
                                    greenLineID = lineID;
                                    break;
                                }
                            }

                            m_spriteRenderer.enabled = true;
                            transform.localPosition = new Vector3()
                            {
                                x = GetCameraXPos(transform.position.y / GridManager.Instance.GetUnitFrameSize(), greenLineID),
                                y = -3.5f,
                                z = 5.0f
                            };
                        }
                        break;

                    case StageData.RegionData.ColorType.Blue:
                        {
                            m_spriteRenderer.enabled = false;
                            transform.localPosition = new Vector3()
                            {
                                x = 0.0f,
                                y = -3.5f,
                                z = 5.0f
                            };
                        }
                        break;
                }
            }
        }

        public bool BIsInitialized
        {
            set
            {
                m_bisInitialized = value;
            }
        }
        public Guid CurRegionID
        {
            set
            {
                m_curRegionID = value;
            }
        }

        private float GetCameraXPos(in float targetFrame, in Guid lineID)
        {
            float NoteYPos = GridManager.Instance.GetYPosFromFrame(targetFrame);

            List<int> nearestLinePosIndexes = new List<int>(2);
            LineRenderer attachedLineRenderer = LineManager.Instance.GetLineItem(lineID).LineRenderer;

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
    }
}
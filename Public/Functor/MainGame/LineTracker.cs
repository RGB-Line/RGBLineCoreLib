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
        private SpriteRenderer m_spriteRenderer;

        [SerializeField] private float m_speed = 1.0f;

        private Dictionary<Guid, float> m_regionTimeTable = new Dictionary<Guid, float>();

        private bool m_bisInitialized = false;

        private Guid m_curRegionID = Guid.Empty;
        private Guid m_curLineID = Guid.Empty;


        public void Awake()
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            m_spriteRenderer.enabled = false;
        }
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
                            m_curLineID = Guid.Empty;

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
                            m_curLineID = Guid.Empty;

                            float horizontalMovement = Input.GetAxis("Horizontal");

                            transform.localPosition = new Vector3()
                            {
                                x = transform.localPosition.x + (horizontalMovement * 0.1f),
                                y = -3.5f,
                                z = 5.0f
                            };
                        }
                        break;

                    case StageData.RegionData.ColorType.Blue:
                        {
                            List<Guid> blueLineIDs = new List<Guid>();
                            Guid[] lineIDs = StageDataInterface.LineDataInterface.GetLineIDs();
                            foreach (Guid lineID in lineIDs)
                            {
                                StageData.LineData lineData = StageDataInterface.LineDataInterface.GetLineData(lineID);
                                ILineItem lineItem = LineManager.Instance.GetLineItem(lineID);
                                if (lineData.AttachedRegionID == m_curRegionID
                                    && lineItem.LineRenderer.GetPosition(0).y <= transform.position.y && transform.position.y <= lineItem.LineRenderer.GetPosition(lineItem.LineRenderer.positionCount - 1).y)
                                {
                                    blueLineIDs.Add(lineID);
                                }
                            }

                            blueLineIDs.Sort((Guid x, Guid y) =>
                            {
                                return StageDataInterface.LineDataInterface.GetLineData(x).CurvedLinePoints[0].X.CompareTo(StageDataInterface.LineDataInterface.GetLineData(y).CurvedLinePoints[0].X);
                            });

                            if(m_curLineID == Guid.Empty)
                            {
                                m_curLineID = blueLineIDs[blueLineIDs.Count / 2];
                            }
                            else if(!blueLineIDs.Contains(m_curLineID))
                            {
                                float prevLineXPos = StageDataInterface.LineDataInterface.GetLineData(m_curLineID).CurvedLinePoints[0].X;

                                float minGap = float.MaxValue;
                                foreach (Guid lineID in blueLineIDs)
                                {
                                    float curLineXPos = StageDataInterface.LineDataInterface.GetLineData(lineID).CurvedLinePoints[0].X;
                                    if (Mathf.Abs(curLineXPos - prevLineXPos) < minGap)
                                    {
                                        minGap = Mathf.Abs(curLineXPos - prevLineXPos);
                                        m_curLineID = lineID;
                                    }
                                }
                            }
                            else
                            {
                                int prevLineIndex = blueLineIDs.IndexOf(m_curLineID);
                                if (Input.GetKeyDown(KeyCode.LeftArrow) && prevLineIndex - 1 >= 0)
                                {
                                    m_curLineID = blueLineIDs[prevLineIndex - 1];
                                }
                                else if (Input.GetKeyDown(KeyCode.RightArrow) && prevLineIndex + 1 < blueLineIDs.Count)
                                {
                                    m_curLineID = blueLineIDs[prevLineIndex + 1];
                                }
                            }

                            Debug.Log(blueLineIDs.Count + " / Current Blue Line ID: " + m_curLineID);

                            m_spriteRenderer.enabled = true;
                            transform.localPosition = new Vector3()
                            {
                                x = GetCameraXPos(transform.position.y / GridManager.Instance.GetUnitFrameSize(), blueLineIDs[blueLineIDs.IndexOf(m_curLineID)]),
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
        public Guid CurLineID
        {
            get
            {
                return m_curLineID;
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
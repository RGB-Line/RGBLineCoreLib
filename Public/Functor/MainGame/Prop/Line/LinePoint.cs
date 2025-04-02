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
    /// 기본적인 Line을 그리기 위한 Class
    /// </summary>
    /// <remarks>
    /// Editor에서는 해당 Class를 상속받아 사용하지만, Runtime에서는 해당 Class를 직접 사용함
    /// </remarks>
    [RequireComponent(typeof(CurvedLinePoint))]
    public class LinePoint : MonoBehaviour, ILinePoint
    {
        [SerializeField] private GameObject m_prefab_RedLineCornerNote;

        private ILineItem m_parentLineItem;
        private int m_pointIndex = -1;


        public Transform Transform
        {
            get
            {
                return transform;
            }
        }

        public virtual void Render(in ILineItem parentLineItem, in Vector2 pos, in float minorOffsetTime, in int pointIndex)
        {
            m_parentLineItem = parentLineItem;
            m_pointIndex = pointIndex;

            transform.position = new Vector3()
            {
                x = pos.x,
                y = GridManager.Instance.GetYPosFromFrame(pos.y + minorOffsetTime)
                    + GridManager.Instance.GetYPosFromFrame(StageDataInterface.LineDataInterface.GetAttachedRegionData(m_parentLineItem.LineID).StartOffsetFrame),
                z = -5.0f
            };

            StageData.RegionData.ColorType attachedRegionColor = StageDataInterface.LineDataInterface.GetAttachedRegionData(m_parentLineItem.LineID).CurColorType;
            switch(attachedRegionColor)
            {
                case StageData.RegionData.ColorType.Red:
                    if (0 < m_pointIndex && m_pointIndex < StageDataInterface.LineDataInterface.GetLineData(m_parentLineItem.LineID).CurvedLinePoints.Count - 1)
                    {
                        IRedLineCornerNote redLineCornerNote = Instantiate(m_prefab_RedLineCornerNote, transform).GetComponent<IRedLineCornerNote>();

                        // TODO - 레드 라인 특수키 변경 → 기존 레드 라인 꺾이는 부분 노트 클릭 키 변경
                        bool bisToLeft = (StageDataInterface.LineDataInterface.GetLineData(m_parentLineItem.LineID).CurvedLinePoints[m_pointIndex + 1].X
                                          - StageDataInterface.LineDataInterface.GetLineData(m_parentLineItem.LineID).CurvedLinePoints[m_pointIndex].X <= 0.0f ? true : false);
                        redLineCornerNote.Render(bisToLeft);
                        redLineCornerNote.Transform.position = transform.position;
                    }
                    break;

                case StageData.RegionData.ColorType.Green:
                    transform.position = new Vector3()
                    {
                        x = transform.position.x,
                        y = transform.position.y,
                        z = 5.0f
                    };
                    break;
            }
        }

        public virtual void Dispose()
        {

        }
    }
}
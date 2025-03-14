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

        public void Render(in ILineItem parentLineItem, in Vector2 pos, in float minorOffsetTime, in int pointIndex)
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
                        redLineCornerNote.Render();
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

        public void Dispose()
        {

        }
    }
}
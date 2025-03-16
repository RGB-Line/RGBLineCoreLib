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
    [RequireComponent(typeof(CurvedLineRenderer))]
    public class LineItem : MonoBehaviour, ILineItem
    {
        [Serializable] public struct LineMaterials
        {
            public Material m_material_RedLine;
            public Material m_material_GreenLine;
            public Material m_material_BlueLine;

            public Material m_material_Outline;
            public Material m_material_DashLine;
        }


        [SerializeField] private GameObject m_prefab_LinePoint;
        [SerializeField] private LineMaterials m_lineMaterials;

        private LineRenderer m_lineRenderer = null;
        private CurvedLineRenderer m_curvedLineRenderer = null;
        private MeshCollider m_meshCollider = null;

        private Stack<ILinePoint> m_linePoints = new Stack<ILinePoint>();

        private Guid m_lineID = Guid.Empty;

        private bool m_bisMeshColliderUpdatable = false;


        public void Awake()
        {
            m_lineRenderer = GetComponent<LineRenderer>();
            m_curvedLineRenderer = GetComponent<CurvedLineRenderer>();
            m_meshCollider = GetComponent<MeshCollider>();
        }
        public void FixedUpdate()
        {
            if (m_bisMeshColliderUpdatable && StageDataInterface.LineDataInterface.GetAttachedRegionData(m_lineID).CurColorType == StageData.RegionData.ColorType.Green)
            {
                UpdateMeshCollider();
            }
        }
        public virtual void OnMouseEnter()
        {
            if (StageDataInterface.LineDataInterface.GetAttachedRegionData(m_lineID).CurColorType == StageData.RegionData.ColorType.Green)
            {
                ScoreManager.Instance.BIsMouseOnGreenLine = true;
            }
            else
            {
                ScoreManager.Instance.BIsMouseOnGreenLine = false;
            }
        }
        public virtual void OnMouseExit()
        {
            ScoreManager.Instance.BIsMouseOnGreenLine = false;
        }

        public Transform Transform
        {
            get
            {
                return transform;
            }
        }
        public LineRenderer LineRenderer
        {
            get
            {
                return m_lineRenderer;
            }
        }

        public Guid LineID
        {
            get
            {
                return m_lineID;
            }
        }

        public virtual void Render(in Guid lineID = default)
        {
            if(m_lineID == default && lineID == default)
            {
                throw new ArgumentException("LineID is not set.");
            }
            else if (lineID != default)
            {
                m_lineID = lineID;
            }

#if FOR_EDITOR
            Dispose();
#endif

            List<HalfFloatVector2> points = null;
            List<float> minorOffsetTimes = null;

#if FOR_EDITOR
            points = (StageDataInterface.LineDataInterface.GetLineData(m_lineID).CurvedLinePoints.ToArray().Clone() as HalfFloatVector2[]).ToList();
            minorOffsetTimes = (StageDataInterface.LineDataInterface.GetLineData(m_lineID).MinorOffsetTimes.ToArray().Clone() as float[]).ToList();
#else
            points = StageDataInterface.LineDataInterface.GetLineData(m_lineID).CurvedLinePoints;
            minorOffsetTimes = StageDataInterface.LineDataInterface.GetLineData(m_lineID).MinorOffsetTimes;
#endif

            for(int index = 0; index < points.Count; index++)
            {
                ILinePoint linePoint = Instantiate(m_prefab_LinePoint, transform).GetComponent<ILinePoint>();

                Vector2 pointPos = new Vector2(points[index].X, points[index].Y);
                if(StageDataInterface.LineDataInterface.GetAttachedRegionData(m_lineID).CurColorType == StageData.RegionData.ColorType.Green)
                {
                    if (index == 0 || index == 1)
                    {
                        pointPos.y += StageDataInterface.LineDataInterface.GetAttachedRegionData(m_lineID).MinorOffsetTime;
                    }
                    else if (index == points.Count - 2 || index == points.Count - 1)
                    {
                        Guid nextRegionID = StageDataInterface.RegionDataInterface.GetNextRegionID(StageDataInterface.LineDataInterface.GetAttachedRegionID(m_lineID));

                        if (nextRegionID != Guid.Empty)
                        {
                            pointPos.y += StageDataInterface.RegionDataInterface.GetRegionData(nextRegionID).MinorOffsetTime;
                        }
                    }
                }
                else
                {
                    if(index == 0)
                    {
                        pointPos.y += StageDataInterface.LineDataInterface.GetAttachedRegionData(m_lineID).MinorOffsetTime;
                    }
                    else if(index == points.Count - 1)
                    {
                        Guid nextRegionID = StageDataInterface.RegionDataInterface.GetNextRegionID(StageDataInterface.LineDataInterface.GetAttachedRegionID(m_lineID));

                        if (nextRegionID != Guid.Empty)
                        {
                            pointPos.y += StageDataInterface.RegionDataInterface.GetRegionData(nextRegionID).MinorOffsetTime;
                        }
                    }
                }

                linePoint.Render(this, pointPos, minorOffsetTimes[index], index);
                m_linePoints.Push(linePoint);
            }

            // Adjest LineRenderer
            m_lineRenderer.numCapVertices = 90;
            m_lineRenderer.alignment = LineAlignment.TransformZ;

            // Draw Line Renderer Specificly
            StageData.LineData lineData = StageDataInterface.LineDataInterface.GetLineData(m_lineID);
            switch (lineData.CurLineSmoothType)
            {
                case StageData.LineData.LineSmoothType.Linear:
                    m_curvedLineRenderer.lineSegmentSize = 100.0f;
                    break;

                case StageData.LineData.LineSmoothType.Curved:
                    m_curvedLineRenderer.lineSegmentSize = 0.1f;
                    break;
            }
            m_curvedLineRenderer.lineWidth = lineData.LineWidth;

            switch (StageDataInterface.LineDataInterface.GetAttachedRegionData(m_lineID).CurColorType)
            {
                case StageData.RegionData.ColorType.Red:
                    m_lineRenderer.material = m_lineMaterials.m_material_RedLine;
                    break;

                case StageData.RegionData.ColorType.Green:
                    m_lineRenderer.material = m_lineMaterials.m_material_GreenLine;
                    break;

                case StageData.RegionData.ColorType.Blue:
                    m_lineRenderer.material = m_lineMaterials.m_material_BlueLine;
                    break;
            }

            // Invoke Callbacks
            if(StageDataInterface.LineDataInterface.GetAttachedRegionData(m_lineID).CurColorType == StageData.RegionData.ColorType.Green)
            {
                Invoke("RenderGreenOutline", 0.5f);
                Invoke("RenderGreenCenterLine", 0.5f);
                Invoke("EnableMeshColliderUpdate", 0.5f);
            }
        }
        public virtual void Dispose()
        {
            while (true)
            {
                if (m_linePoints.Count == 0)
                {
                    break;
                }

                ILinePoint linePointItem = m_linePoints.Pop();
                linePointItem.Dispose();

                Destroy(linePointItem.Transform.gameObject);
            }
        }

        private void UpdateMeshCollider()
        {
            Mesh mesh = new Mesh();
            m_lineRenderer.BakeMesh(mesh, true);
            m_meshCollider.sharedMesh = mesh;
        }
        private void RenderGreenOutline()
        {
            GameObject outline = new GameObject("Outline");
            outline.transform.SetParent(LineManager.Instance.transform);

            LineRenderer outlineRenderer = outline.AddComponent<LineRenderer>();
            outlineRenderer.material = m_lineMaterials.m_material_Outline;
            outlineRenderer.positionCount = m_lineRenderer.positionCount;
            outlineRenderer.sortingOrder = 0;
            outlineRenderer.numCapVertices = 90;

            Vector3[] positions = new Vector3[m_lineRenderer.positionCount];
            m_lineRenderer.GetPositions(positions);
            outlineRenderer.SetPositions(positions);

            outlineRenderer.startWidth = m_lineRenderer.startWidth + 0.3f;
            outlineRenderer.endWidth = m_lineRenderer.endWidth + 0.3f;
        }
        private void RenderGreenCenterLine()
        {
            GameObject centerLine = new GameObject("CenterLine");
            centerLine.transform.SetParent(LineManager.Instance.transform);

            LineRenderer centerlineRenderer = centerLine.AddComponent<LineRenderer>();
            centerlineRenderer.material = m_lineMaterials.m_material_DashLine;
            centerlineRenderer.positionCount = m_lineRenderer.positionCount;
            centerlineRenderer.sortingOrder = 0;
            centerlineRenderer.numCapVertices = 90;

            Vector3[] positions = new Vector3[m_lineRenderer.positionCount];
            m_lineRenderer.GetPositions(positions);
            centerlineRenderer.SetPositions(positions);

            centerlineRenderer.startWidth = 0.04f;
            centerlineRenderer.endWidth = 0.04f;
        }
        private void EnableMeshColliderUpdate()
        {
            m_bisMeshColliderUpdatable = true;
        }
    }
}
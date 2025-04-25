using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib.Functor
{
    [RequireComponent(typeof(CurvedLineRenderer), typeof(LineRenderer))]
    public sealed class BackgroundGradientEffectLightPath : MonoBehaviour
    {
        [SerializeField] private GameObject m_prefab_CurvedLinePoint;

        private MeshRenderer m_backgroundMeshRenderer;
        private Transform m_lightTransform;

        private CurvedLineRenderer m_curvedLineRenderer;
        private LineRenderer m_lineRenderer;

        private int m_curLinePointIndex = 0;
        private float m_curLineTupleProgress = 0.0f;

        private BackgroundGradientSetting m_setting;


        public void Awake()
        {
            m_curvedLineRenderer = GetComponent<CurvedLineRenderer>();
            m_lineRenderer = GetComponent<LineRenderer>();
            m_lineRenderer.enabled = false;
        }
        public void Update()
        {
            if (m_curLineTupleProgress + 0.1f > 1.0f)
            {
                if (m_curLinePointIndex + 1 < m_lineRenderer.positionCount)
                {
                    m_curLinePointIndex++;
                }
                else
                {
                    m_curLinePointIndex = 0;
                }
                m_curLineTupleProgress = 0.0f;
            }
            else
            {
                m_curLineTupleProgress += m_setting.LightSpeed;
            }

            if (m_curLinePointIndex + 1 < m_lineRenderer.positionCount)
            {
                m_lightTransform.position = new Vector3()
                {
                    x = Mathf.Lerp(m_lineRenderer.GetPosition(m_curLinePointIndex).x, m_lineRenderer.GetPosition(m_curLinePointIndex + 1).x, m_curLineTupleProgress),
                    y = Mathf.Lerp(m_lineRenderer.GetPosition(m_curLinePointIndex).y, m_lineRenderer.GetPosition(m_curLinePointIndex + 1).y, m_curLineTupleProgress),
                };
            }
        }

        internal void Init(in BackgroundGradientSetting setting, in MeshRenderer backgroundMeshRenderer, in Transform lightTransform)
        {
            m_setting = setting;

            m_backgroundMeshRenderer = backgroundMeshRenderer;
            m_lightTransform = lightTransform;

            // Create Line
            int curvedLinePointCount = UnityEngine.Random.Range(m_setting.MinCurvedLinePointCount, m_setting.MaxCurvedLinePointCount);
            Vector2 startLightPos = Vector2.zero;
            for (int index = 0; index < curvedLinePointCount; index++)
            {
                Vector2 curvedLinePointPose = Vector2.zero;

                if (index == 0)
                {
                    while (true)
                    {
                        curvedLinePointPose = new Vector2(
                            UnityEngine.Random.Range(-m_backgroundMeshRenderer.bounds.size.x / 2.0f - m_setting.HorizontalLineRange, m_backgroundMeshRenderer.bounds.size.x / 2.0f + m_setting.HorizontalLineRange),
                            UnityEngine.Random.Range(-m_backgroundMeshRenderer.bounds.size.y / 2.0f - m_setting.VerticalLineRange, m_backgroundMeshRenderer.bounds.size.y / 2.0f + m_setting.VerticalLineRange)
                        );

                        if ((curvedLinePointPose.x < -m_backgroundMeshRenderer.bounds.size.x / 2.0f || m_backgroundMeshRenderer.bounds.size.x / 2.0f < curvedLinePointPose.x)
                            && (curvedLinePointPose.y < -m_backgroundMeshRenderer.bounds.size.y / 2.0f || m_backgroundMeshRenderer.bounds.size.y / 2.0f < curvedLinePointPose.y))
                        {
                            break;
                        }
                    }

                    startLightPos = curvedLinePointPose;
                }
                else
                {
                    curvedLinePointPose = new Vector2(
                        UnityEngine.Random.Range(-m_backgroundMeshRenderer.bounds.size.x / 2.0f - m_setting.HorizontalLineRange, m_backgroundMeshRenderer.bounds.size.x / 2.0f + m_setting.HorizontalLineRange),
                        UnityEngine.Random.Range(-m_backgroundMeshRenderer.bounds.size.y / 2.0f - m_setting.VerticalLineRange, m_backgroundMeshRenderer.bounds.size.y / 2.0f + m_setting.VerticalLineRange)
                    );
                }

                Instantiate(m_prefab_CurvedLinePoint, new Vector3(curvedLinePointPose.x, curvedLinePointPose.y, 0.0f), Quaternion.identity, transform);
            }
            Instantiate(m_prefab_CurvedLinePoint, new Vector3(startLightPos.x, startLightPos.y, 0.0f), Quaternion.identity, transform);
        }
    }
}
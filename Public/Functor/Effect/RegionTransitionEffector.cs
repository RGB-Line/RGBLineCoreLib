using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib.Functor
{
    /// <summary>
    /// Region의 Red, Green, Blue의 색상 전환 효과를 담당하는 Class이다
    /// </summary>
    public sealed class RegionTransitionEffector : MonoBehaviour
    {
        [SerializeField] private List<Material> m_mats;

        [SerializeField] private MeshRenderer m_meshRenderer;
        [SerializeField] private MeshRenderer m_BlurMeshRenderer;

        [SerializeField] private RegionFadeoutEffect m_fadeOut;
        [SerializeField] private float m_timeout;

        private int m_curMatIndex;

        private bool m_bisNowTransitioning = false;


        public void Awake()
        {
            m_fadeOut.gameObject.SetActive(false);
            m_fadeOut.Timeout = m_timeout;
        }

        public Material[] Materials
        {
            get
            {
                return m_mats.ToArray().Clone() as Material[];
            }
        }
        public float Timeout
        {
            get
            {
                return m_timeout;
            }
        }

        public bool BIsNowTransitioning
        {
            get
            {
                return m_bisNowTransitioning;
            }
        }

        public void StartTransition(in int matIndex, in Vector2 effectStartPos)
        {
            if(m_bisNowTransitioning)
            {
                return;
            }

            m_curMatIndex = matIndex;

            Color curBaseColor = m_mats[m_curMatIndex].GetColor("_BaseColor");

            m_fadeOut.CalculateFadeout(curBaseColor, effectStartPos);
            m_fadeOut.gameObject.SetActive(true);

            m_bisNowTransitioning = true;
            Invoke("FadeIn", m_timeout);
        }

        private void FadeIn()
        {
            m_meshRenderer.material = m_mats[m_curMatIndex];
            if (m_BlurMeshRenderer != null)
            {
                m_BlurMeshRenderer.material.SetColor("_BaseColor", m_mats[m_curMatIndex].GetColor("_BaseColor"));
            }

            m_fadeOut.gameObject.SetActive(false);
            m_bisNowTransitioning = false;
        }
    }
}
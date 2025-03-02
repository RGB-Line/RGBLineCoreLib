using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib
{
    public sealed class RegionTransitionEffector : MonoBehaviour
    {
        [SerializeField] private List<Material> m_mats;

        [SerializeField] private MeshRenderer m_meshRenderer;
        [SerializeField] private MeshRenderer m_BlurMeshRenderer;

        [SerializeField] private RegionFadeoutEffect m_fadeOut;
        [SerializeField] private float m_timeout;

        private int m_curMatIndex;


        public void Awake()
        {
            m_fadeOut.gameObject.SetActive(false);
            m_fadeOut.Timeout = m_timeout;
        }

        public float Timeout
        {
            get
            {
                return m_timeout;
            }
        }

        public void StartTransition(in int matIndex, in Vector2 effectStartPos)
        {
            m_curMatIndex = matIndex;

            Color curBaseColor = m_mats[m_curMatIndex].GetColor("_BaseColor");

            m_fadeOut.CalculateFadeout(curBaseColor, effectStartPos);
            m_fadeOut.gameObject.SetActive(true);

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
        }
    }
}
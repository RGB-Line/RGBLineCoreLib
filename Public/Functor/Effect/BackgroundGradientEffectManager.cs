using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib.Functor
{
    [RequireComponent(typeof(MeshRenderer))]
    public sealed class BackgroundGradientEffectManager : MonoBehaviour
    {
        [Header("Basic Settings")]
        [SerializeField] private MeshRenderer m_meshRenderer = null;
        [SerializeField] private MeshRenderer m_BlurMeshRenderer = null;

        [SerializeField] private BackgroundGradientFadeoutEffect m_fadeOut = null;
        [SerializeField] private float m_timeout = 0.5f;

        [Header("Light Path Prefab")]
        [SerializeField] private Transform m_lightPathParent = null;
        [SerializeField] private Transform m_lightParent = null;

        [SerializeField] private BackgroundGradientSetting m_setting;

        private int m_curMatIndex;
        private int m_nextMatIndex;

        private bool m_bisNowTransitioning = false;

        private List<Transform> m_lightTransforms = new List<Transform>();


        public void Awake()
        {
            m_fadeOut.gameObject.SetActive(false);
            m_fadeOut.Timeout = m_timeout;

            // Light Init
            for (int lightIndex = 0; lightIndex < m_setting.LightCount; lightIndex++)
            {
                GameObject light = new GameObject("Light");
                light.transform.SetParent(m_lightParent);
                m_lightTransforms.Add(light.transform);

                BackgroundGradientEffectLightPath lightPath = Instantiate(m_setting.Prefab_BackgroundGradientLightPath, m_lightPathParent).GetComponent<BackgroundGradientEffectLightPath>();
                lightPath.Init(m_setting, m_meshRenderer, light.transform);
            }
        }
        public void Update()
        {
            m_meshRenderer.material.SetInt("_LightCount", m_setting.LightCount);
            Vector4[] lightPoses = new Vector4[m_lightTransforms.Count];
            for (int i = 0; i < m_lightTransforms.Count; i++)
            {
                lightPoses[i] = new Vector4()
                {
                    x = m_lightTransforms[i].localPosition.x / m_meshRenderer.bounds.size.x + 0.5f,
                    y = m_lightTransforms[i].localPosition.y / m_meshRenderer.bounds.size.y + 0.5f,
                };
            }
            m_meshRenderer.material.SetVectorArray("_LightPoses", lightPoses);

            Vector4[] lightColors = new Vector4[m_lightTransforms.Count];
            for (int i = 0; i < m_lightTransforms.Count; i++)
            {
                switch(m_curMatIndex)
                {
                    case 0:
                        lightColors[i] = new Vector4(m_setting.RedLightColors[i].r, m_setting.RedLightColors[i].g, m_setting.RedLightColors[i].b, m_setting.RedLightColors[i].a);
                        break;

                    case 1:
                        lightColors[i] = new Vector4(m_setting.GreenLightColors[i].r, m_setting.GreenLightColors[i].g, m_setting.GreenLightColors[i].b, m_setting.GreenLightColors[i].a);
                        break;

                    case 2:
                        lightColors[i] = new Vector4(m_setting.BlueLightColors[i].r, m_setting.BlueLightColors[i].g, m_setting.BlueLightColors[i].b, m_setting.BlueLightColors[i].a);
                        break;
                }
            }
            m_meshRenderer.material.SetVectorArray("_LightColors", lightColors);

            m_fadeOut.UpdateLightStatus(lightPoses, lightColors);

            if (m_BlurMeshRenderer != null)
            {
                m_BlurMeshRenderer.material.SetColor("_BaseColor", m_setting.Mats[m_curMatIndex].GetColor("_BaseColor"));

                m_BlurMeshRenderer.material.SetFloat("_ColorSpread", m_setting.Mats[m_curMatIndex].GetFloat("_ColorSpread"));
                m_BlurMeshRenderer.material.SetFloat("_ColorEdgySmoothness", m_setting.Mats[m_curMatIndex].GetFloat("_ColorEdgySmoothness"));
                m_BlurMeshRenderer.material.SetFloat("_LightColorHighlighting", m_setting.Mats[m_curMatIndex].GetFloat("_LightColorHighlighting"));

                m_BlurMeshRenderer.material.SetFloat("_LightCount", m_setting.LightCount);
                m_BlurMeshRenderer.material.SetVectorArray("_LightPoses", lightPoses);
                m_BlurMeshRenderer.material.SetVectorArray("_LightColors", lightColors);
            }
        }

        public Material[] Materials
        {
            get
            {
                return m_setting.Mats.ToArray().Clone() as Material[];
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
            if (m_bisNowTransitioning)
            {
                return;
            }

            if(matIndex == m_curMatIndex)
            {
                return;
            }

            //m_curMatIndex = matIndex;
            m_nextMatIndex = matIndex;

            //Color curBaseColor = m_setting.Mats[m_curMatIndex].GetColor("_BaseColor");

            m_fadeOut.CalculateFadeout(new BackgroundGradientFadeoutEffect.FadeoutInput()
            {
                TargetBaseColor = m_setting.Mats[matIndex].GetColor("_BaseColor"),
                EffectStartPos = effectStartPos,

                ColorSpread = m_setting.Mats[matIndex].GetFloat("_ColorSpread"),
                ColorEdgySmoothness = m_setting.Mats[matIndex].GetFloat("_ColorEdgySmoothness"),
                LightColorHighlighting = m_setting.Mats[matIndex].GetFloat("_LightColorHighlighting"),

                LightCount = m_setting.LightCount
            });
            //m_fadeOut.CalculateFadeout(curBaseColor, effectStartPos);
            m_fadeOut.gameObject.SetActive(true);

            m_bisNowTransitioning = true;
            Invoke("FadeIn", m_timeout);
        }

        private void FadeIn()
        {
            m_curMatIndex = m_nextMatIndex;

            m_meshRenderer.material = m_setting.Mats[m_curMatIndex];
            //if (m_BlurMeshRenderer != null)
            //{
            //    m_BlurMeshRenderer.material.SetColor("_BaseColor", m_setting.Mats[m_curMatIndex].GetColor("_BaseColor"));

            //    m_BlurMeshRenderer.material.SetFloat("_ColorSpread", m_setting.Mats[m_curMatIndex].GetFloat("_ColorSpread"));
            //    m_BlurMeshRenderer.material.SetFloat("_ColorEdgySmoothness", m_setting.Mats[m_curMatIndex].GetFloat("_ColorEdgySmoothness"));
            //    m_BlurMeshRenderer.material.SetFloat("_LightColorHighlighting", m_setting.Mats[m_curMatIndex].GetFloat("_LightColorHighlighting"));

            //    m_BlurMeshRenderer.material.SetFloat("_LightCount", m_setting.LightCount);
            //    m_BlurMeshRenderer.material.SetVectorArray("_LightPoses", m_setting.Mats[m_curMatIndex].GetVectorArray("_LightPoses"));
            //    m_BlurMeshRenderer.material.SetVectorArray("_LightColors", m_setting.Mats[m_curMatIndex].GetVectorArray("_LightColors"));
            //}

            m_fadeOut.gameObject.SetActive(false);
            m_bisNowTransitioning = false;
        }
    }
}
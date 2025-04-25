using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib.Functor
{
    [RequireComponent(typeof(MeshRenderer))]
    public sealed class BackgroundGradientFadeoutEffect : MonoBehaviour
    {
        public struct FadeoutInput
        {
            public Color TargetBaseColor;
            public Vector2 EffectStartPos;

            public float ColorSpread;
            public float ColorEdgySmoothness;
            public float LightColorHighlighting;

            public int LightCount;
        }


        [SerializeField] private MeshRenderer m_meshRenderer;

        private bool m_bisStartEffect;

        private float m_timeout;
        private float m_curTime;

        private Color m_prevBaseColor;
        private Color m_targetBaseColor;

        private Vector2 m_effectStartPos;

        private float m_colorSpread;
        private float m_colorEdgySmoothness;
        private float m_lightColorHighlighting;

        private int m_lightCount;
        private Vector4[] m_lightPoses;
        private Vector4[] m_lightColors;


        public void Update()
        {
            if (m_bisStartEffect)
            {
                //Debug.Log("RegionFadeoutEffect Update");

                m_curTime += Time.deltaTime;

                m_meshRenderer.material.SetVectorArray("_LightPoses", m_lightPoses);
                m_meshRenderer.material.SetVectorArray("_LightColors", m_lightColors);

                m_meshRenderer.material.SetFloat("_Radius", Mathf.Lerp(10000.0f, 0.0f, (m_curTime / m_timeout)));

                if (m_curTime >= m_timeout)
                {
                    m_bisStartEffect = false;
                }
            }
        }

        internal float Timeout
        {
            set
            {
                m_timeout = value;
            }
        }

        public void CalculateFadeout(in FadeoutInput input)
        {
            m_meshRenderer.material.SetFloat("_Radius", 0.0f);
            m_curTime = 0.0f;

            m_prevBaseColor = m_targetBaseColor;
            m_targetBaseColor = input.TargetBaseColor;

            //m_effectStartPos = input.EffectStartPos;

            //m_colorSpread = input.ColorSpread;
            //m_colorEdgySmoothness = input.ColorEdgySmoothness;
            //m_lightColorHighlighting = input.LightColorHighlighting;

            //m_lightCount = input.LightCount;

            m_meshRenderer.material.SetColor("_BaseColor", m_targetBaseColor);
            m_meshRenderer.material.SetColor("_PrevBaseColor", m_prevBaseColor);
            m_meshRenderer.material.SetFloat("_EffectStartPos_X", input.EffectStartPos.x);
            m_meshRenderer.material.SetFloat("_EffectStartPos_Y", input.EffectStartPos.y);

            m_meshRenderer.material.SetFloat("_ColorSpread", input.ColorSpread);
            m_meshRenderer.material.SetFloat("_ColorEdgySmoothness", input.ColorEdgySmoothness);
            m_meshRenderer.material.SetFloat("_LightColorHighlighting", input.LightColorHighlighting);

            m_meshRenderer.material.SetInt("_LightCount", input.LightCount);

            m_bisStartEffect = true;
        }

        internal void UpdateLightStatus(in Vector4[] lightPoses, in Vector4[] lightColors)
        {
            m_lightPoses = lightPoses;
            m_lightColors = lightColors;
        }
    }
}
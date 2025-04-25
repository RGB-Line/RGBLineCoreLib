using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib.Functor
{
    [CreateAssetMenu(fileName = "BackgroundGradientSetting", menuName = "RGBLine/BackgroundGradientSetting")]
    public sealed class BackgroundGradientSetting : ScriptableObject
    {
        [Header("Basic Settings")]
        public List<Material> Mats = null;

        [Header("Red Light Colors")]
        public Color[] RedLightColors = null;

        [Header("Green Light Colors")]
        public Color[] GreenLightColors = null;

        [Header("Blue Light Colors")]
        public Color[] BlueLightColors = null;

        [Header("Light Path Prefab")]
        public GameObject Prefab_BackgroundGradientLightPath = null;

        [Header("Light Count")]
        public int LightCount = 2;

        [Header("Curved Line Point Count")]
        public int MinCurvedLinePointCount = 5;
        public int MaxCurvedLinePointCount = 10;

        [Header("Curved Line Point Range")]
        public float HorizontalLineRange = 5.0f;
        public float VerticalLineRange = 5.0f;

        [Header("Background Light Speed")]
        public float LightSpeed = 0.1f;
    }
}
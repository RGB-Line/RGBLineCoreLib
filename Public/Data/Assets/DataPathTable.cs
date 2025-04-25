using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using UnityEngine;


namespace RGBLineCoreLib.Data
{
    /// <summary>
    /// Stage와 Config 관련 파일이 저장되는 위치와 확장자를 지정한다
    /// </summary>
    [CreateAssetMenu(fileName = "DataPathTable", menuName = "RGBLine/DataPathTable", order = 1)]
    internal class DataPathTable : ScriptableObject
    {
        [Header("Stage")]
        public string StagesBasePath = null;

        public string StageDataFilePathTemplate = null;
        public string StageDataFileExtension = null;

        public string StageMetaDataFilePathTemplate = null;
        public string StageMetaDataFileExtension = null;

        [Header("Config")]
        public string ConfigBasePath = null;

        public string ConfigFileName = null;
        public string ConfigFileExtension = null;
    }
}
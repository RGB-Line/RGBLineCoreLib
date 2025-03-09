using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib
{
    public sealed class StageLoadInfoDTO : MonoBehaviour
    {
        public string StageName;
        public StageMetadata.MajorDifficultyLevel MajorDifficulty;
    }
}
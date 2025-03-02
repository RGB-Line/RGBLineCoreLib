using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RGBLineCoreLib
{
    public static class StageMetadataInterface
    {
        public static StageMetadata GetStageMetadata()
        {
            return StageMetadataBuffer.Instance.StageMetadata;
        }

        public static bool TryLoadStageMetadata(in string targetStageName, in StageMetadata.MajorDifficultyLevel majorDifficulty)
        {
            return StageMetadataBuffer.Instance.TryLoadStageMetadata(targetStageName, majorDifficulty);
        }
    }
}
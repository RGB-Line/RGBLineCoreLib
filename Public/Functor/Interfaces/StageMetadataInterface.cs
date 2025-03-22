using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RGBLineCoreLib.Data;


namespace RGBLineCoreLib.Functor
{
    /// <summary>
    /// StageMetadata의 내용에 접근하기 위한 Interface
    /// </summary>
    public static class StageMetadataInterface
    {
        /// <summary>
        /// 현재 Load된 StageMetadata를 반환한다
        /// </summary>
        public static StageMetadata GetStageMetadata()
        {
            return StageMetadataBuffer.Instance.StageMetadata;
        }

        /// <summary>
        /// DataPathTable의 내용을 바탕으로 Load를 시도한다
        /// </summary>
        /// <remarks>
        /// StageMetadataInterface.GetStageMetadata()를 호출하기 전에 필히 호출해야만 한다
        /// </remarks>
        /// <returns>
        /// Load에 성공했을 경우 true, 실패했을 경우 false
        /// </returns>
        public static bool TryLoadStageMetadata(in string targetStageName, in StageMetadata.MajorDifficultyLevel majorDifficulty)
        {
            return StageMetadataBuffer.Instance.TryLoadStageMetadata(targetStageName, majorDifficulty);
        }
        public static void DisposeStageMetadata()
        {
            StageMetadataBuffer.Instance.Dispose();
        }

        public static bool BIsStageMetadataValid()
        {
            return StageMetadataBuffer.Instance.StageMetadata.BIsValid();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RGBLineCoreLib.Data;


namespace RGBLineCoreLib.Functor
{
    internal static class StageMetadataExtension
    {
        internal static bool BIsValid(this StageMetadata targetStageMetadata)
        {
            if (string.IsNullOrEmpty(targetStageMetadata.Title))
            {
                throw new Exception("StageMetadata Title is null or empty.");
                return false;
            }
            else if (string.IsNullOrEmpty(targetStageMetadata.Artist))
            {
                throw new Exception("StageMetadata Artist is null or empty.");
                return false;
            }

            if (targetStageMetadata.MusicLength <= 0.0f)
            {
                throw new Exception("StageMetadata MusicLength is less than or equal to 0.0f.");
                return false;
            }

            if (!Enum.IsDefined(typeof(StageMetadata.MajorDifficultyLevel), targetStageMetadata.MajorDifficulty))
            {
                throw new Exception("StageMetadata MajorDifficulty is not defined.");
                return false;
            }
            if (!(1 <= targetStageMetadata.MinorDifficulty && targetStageMetadata.MinorDifficulty <= 10))
            {
                throw new Exception("StageMetadata MinorDifficulty is not in range of 1 to 10.");
                return false;
            }

            if (targetStageMetadata.BestScore < 0 || targetStageMetadata.BestScore > 1000000)
            {
                throw new Exception("StageMetadata BestScore is not in range of 0 to 1000000.");
                return false;
            }

            if (targetStageMetadata.LobbyMusicStartTime < 0.0f)
            {
                throw new Exception("StageMetadata LobbyMusicStartTime is less than 0.0f.");
                return false;
            }
            else if (targetStageMetadata.LobbyMusicEndTime < targetStageMetadata.LobbyMusicStartTime || targetStageMetadata.LobbyMusicEndTime > targetStageMetadata.MusicLength)
            {
                throw new Exception("StageMetadata LobbyMusicEndTime is less than LobbyMusicStartTime or greater than MusicLength.");
                return false;
            }
            else if (targetStageMetadata.LobbyMusicEndTime - targetStageMetadata.LobbyMusicStartTime < 2.0f * targetStageMetadata.LobbyMusicFadeOutTime)
            {
                throw new Exception("StageMetadata LobbyMusicEndTime - LobbyMusicStartTime is less than 2.0f * LobbyMusicFadeOutTime.");
                return false;
            }

            return true;
        }

        internal static void Dispose(this StageMetadata targetStageMetadata)
        {
            targetStageMetadata.Title = null;
            targetStageMetadata.Artist = null;

            targetStageMetadata.MusicLength = 0.0f;

            targetStageMetadata.MajorDifficulty = StageMetadata.MajorDifficultyLevel.Easy;
            targetStageMetadata.MinorDifficulty = 0;

            targetStageMetadata.BestScore = 0;

            targetStageMetadata.LobbyMusicStartTime = 0.0f;
            targetStageMetadata.LobbyMusicEndTime = 0.0f;
            targetStageMetadata.LobbyMusicFadeOutTime = 0.0f;
        }
    }
}
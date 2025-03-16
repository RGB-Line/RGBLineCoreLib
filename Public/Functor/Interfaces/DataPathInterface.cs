using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using RGBLineCoreLib.Data;
using RGBLineCoreLib.Manager;


namespace RGBLineCoreLib.Functor
{
    /// <summary>
    /// DataPathTable의 내용에 접근하기 위해 제공되는 Interface
    /// </summary>
    public static class DataPathInterface
    {
        /// <summary>
        /// StageData File의 경로를 반환한다
        /// </summary>
        public static string GetStageDataPath(in string stageName, in StageMetadata.MajorDifficultyLevel majorDifficulty)
        {
            string path = Path.Combine(Application.streamingAssetsPath, DataPathManager.Instance.DataPathTable.StagesBasePath);
            path = Path.Combine(path, stageName);
            switch (majorDifficulty)
            {
                case StageMetadata.MajorDifficultyLevel.Easy:
                    path = Path.Combine(path, "Easy");
                    break;

                case StageMetadata.MajorDifficultyLevel.Normal:
                    path = Path.Combine(path, "Normal");
                    break;

                case StageMetadata.MajorDifficultyLevel.Hard:
                    path = Path.Combine(path, "Hard");
                    break;
            }
            path = Path.Combine(path, DataPathManager.Instance.DataPathTable.StageDataFilePathTemplate + stageName);
            path = Path.ChangeExtension(path, DataPathManager.Instance.DataPathTable.StageDataFileExtension);

            return path;
        }
        /// <summary>
        /// Stage의 Music File의 경로를 반환한다
        /// </summary>
        public static string GetStageMusicPath(in string stageName, in string musicExtention)
        {
            string path = Path.Combine(Application.streamingAssetsPath, DataPathManager.Instance.DataPathTable.StagesBasePath);
            path = Path.Combine(path, stageName);
            path = Path.Combine(path, stageName + musicExtention);

            return path;
        }
        /// <summary>
        /// StageMetadata File의 경로를 반환한다
        /// </summary>
        public static string GetStageMetadataPath(in string stageName, in StageMetadata.MajorDifficultyLevel majorDifficulty)
        {
            string path = Path.Combine(Application.streamingAssetsPath, DataPathManager.Instance.DataPathTable.StagesBasePath);
            path = Path.Combine(path, stageName);
            switch (majorDifficulty)
            {
                case StageMetadata.MajorDifficultyLevel.Easy:
                    path = Path.Combine(path, "Easy");
                    break;

                case StageMetadata.MajorDifficultyLevel.Normal:
                    path = Path.Combine(path, "Normal");
                    break;

                case StageMetadata.MajorDifficultyLevel.Hard:
                    path = Path.Combine(path, "Hard");
                    break;
            }
            path = Path.Combine(path, DataPathManager.Instance.DataPathTable.StageMetaDataFilePathTemplate + stageName);
            path = Path.ChangeExtension(path, DataPathManager.Instance.DataPathTable.StageMetaDataFileExtension);

            return path;
        }

        /// <summary>
        /// 모든 Stage 이름의 목록을 반환한다
        /// </summary>
        public static string[] GetStageNames()
        {
            string[] stageNames = Directory.GetDirectories(Path.Combine(Application.streamingAssetsPath, DataPathManager.Instance.DataPathTable.StagesBasePath));
            for (int index = 0; index < stageNames.Length; index++)
            {
                stageNames[index] = Path.GetFileName(stageNames[index]);
            }

            return stageNames;
        }

        /// <summary>
        /// GameConfigData File의 경로를 반환한다
        /// </summary>
        public static string GetConfigPath()
        {
            string path = Path.Combine(Application.streamingAssetsPath, DataPathManager.Instance.DataPathTable.ConfigBasePath);
            path = Path.Combine(path, DataPathManager.Instance.DataPathTable.ConfigFileName);
            path = Path.ChangeExtension(path, DataPathManager.Instance.DataPathTable.ConfigFileExtension);

            return path;
        }
    }
}
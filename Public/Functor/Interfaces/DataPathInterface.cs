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
    public static class DataPathInterface
    {
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
        public static string GetStageMusicPath(in string stageName, in string musicExtention)
        {
            string path = Path.Combine(Application.streamingAssetsPath, DataPathManager.Instance.DataPathTable.StagesBasePath);
            path = Path.Combine(path, stageName);
            path = Path.Combine(path, stageName + musicExtention);

            return path;
        }
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

        public static string[] GetStageNames()
        {
            string[] stageNames = Directory.GetDirectories(Path.Combine(Application.streamingAssetsPath, DataPathManager.Instance.DataPathTable.StagesBasePath));
            for (int index = 0; index < stageNames.Length; index++)
            {
                stageNames[index] = Path.GetFileName(stageNames[index]);
            }

            return stageNames;
        }

        public static string GetConfigPath()
        {
            string path = Path.Combine(Application.streamingAssetsPath, DataPathManager.Instance.DataPathTable.ConfigBasePath);
            path = Path.Combine(path, DataPathManager.Instance.DataPathTable.ConfigFileName);
            path = Path.ChangeExtension(path, DataPathManager.Instance.DataPathTable.ConfigFileExtension);

            return path;
        }
    }
}
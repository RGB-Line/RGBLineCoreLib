using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RGBLineCoreLib.Data;


namespace RGBLineCoreLib.Functor
{
    /// <summary>
    /// StageData의 내용에 접근하기 위해 제공되는 Interface
    /// </summary>
    public static class StageDataInterface
    {
        /// <summary>
        /// 현재 StageData의 RegionData에 접근하기 위한 Interface
        /// </summary>
        public static class RegionDataInterface
        {
            #region Getter
            /// <summary>
            /// 주어진 targetRegionID에 해당하는 RegionData를 반환한다
            /// </summary>
            /// <remarks>
            /// 만약 해당하는 RegionData가 없을 경우 KeyNotFoundException을 발생시킨다
            /// </remarks>
            /// <exception cref="KeyNotFoundException"></exception>
            public static StageData.RegionData GetRegionData(in Guid targetRegionID)
            {
                return StageDataBuffer.Instance.StageData.RegionDataTable[targetRegionID];
            }

            /// <summary>
            /// 현재 StageData에 존재하는 모든 RegionData의 RegionID를 반환한다
            /// </summary>
            public static Guid[] GetRegionIDs()
            {
                return StageDataBuffer.Instance.StageData.RegionDataTable.Keys.ToArray();
            }

            /// <summary>
            /// curRegionID 기준으로 바로 다음에 위치하는, 즉, curRegionID에 해당하는 Region보다 위쪽에 위치하는 RegionData의 RegionID를 반환한다
            /// </summary>
#if FOR_EDITOR
            public
#else
            internal
#endif
            static Guid GetNextRegionID(in Guid curRegionID)
            {
                Guid nextRegionID = Guid.Empty;
                Guid[] regionIDs = GetRegionIDs();

                StageData.RegionData curRegionData = GetRegionData(curRegionID);
                foreach (Guid regionID in regionIDs)
                {
                    StageData.RegionData candidateRegionData = GetRegionData(regionID);
                    if (candidateRegionData.StartOffsetFrame > curRegionData.StartOffsetFrame)
                    {
                        nextRegionID = regionID;
                        break;
                    }
                }

                return nextRegionID;
            }

            internal static int GetRegionDataIndex(in Guid targetRegionID)
            {
                return StageDataBuffer.Instance.StageData.RegionDataTable.Keys.ToList().IndexOf(targetRegionID);
            }
            internal static bool BIsRegionDataValid(in Tuple<Guid, StageData.RegionData> targetRegionData)
            {
                if (targetRegionData.Item1 == Guid.Empty)
                {
                    return false;
                }

                int targetRegionDataIndex = GetRegionDataIndex(targetRegionData.Item1);
                if (targetRegionDataIndex == 0 && targetRegionData.Item2.StartOffsetFrame != 0)
                {
                    return false;
                }
                else if (StageDataBuffer.Instance.StageData.RegionDataTable.Values.ToList()[targetRegionDataIndex - 1].StartOffsetFrame > targetRegionData.Item2.StartOffsetFrame)
                {
                    return false;
                }

                if (targetRegionData.Item2.MinorOffsetTime < -0.5f || 0.5f < targetRegionData.Item2.MinorOffsetTime)
                {
                    return false;
                }

                if (!Enum.IsDefined(typeof(StageData.RegionData.ColorType), targetRegionData.Item2.CurColorType))
                {
                    return false;
                }

                return true;
            }
#endregion

#if FOR_EDITOR
            #region Setter
            public static bool TryAddRegionData(in Guid targetRegionID, in StageData.RegionData targetRegionData)
            {
                if (StageDataBuffer.Instance.StageData.RegionDataTable.ContainsKey(targetRegionID))
                {
                    return false;
                }

                StageDataBuffer.Instance.StageData.RegionDataTable.Add(targetRegionID, targetRegionData);
                return true;
            }
            public static bool TryRemoveRegionData(in Guid targetRegionID)
            {
                if (!StageDataBuffer.Instance.StageData.RegionDataTable.ContainsKey(targetRegionID))
                {
                    return false;
                }

                StageDataBuffer.Instance.StageData.RegionDataTable.Remove(targetRegionID);
                return true;
            }

            public static bool TrySetRegionData(in Guid targetRegionID, in StageData.RegionData targetRegionData)
            {
                if (!StageDataBuffer.Instance.StageData.RegionDataTable.ContainsKey(targetRegionID))
                {
                    return false;
                }

                StageDataBuffer.Instance.StageData.RegionDataTable[targetRegionID] = targetRegionData;
                return true;
            }
            #endregion
#endif
        }
        /// <summary>
        /// 현재 StageData의 LineData에 접근하기 위한 Interface
        /// </summary>
        public static class LineDataInterface
        {
            #region Getter
            /// <summary>
            /// 주어진 targetLineID에 해당하는 Line이 속한 Region의 RegionID를 반환한다
            /// </summary>
            /// <remarks>
            /// 만약 해당하는 LineID가 없을 경우 KeyNotFoundException을 발생시킨다
            /// </remarks>
            /// <exception cref="KeyNotFoundException"></exception>
            public static Guid GetAttachedRegionID(in Guid targetLineID)
            {
                return StageDataBuffer.Instance.StageData.LineDataTable[targetLineID].AttachedRegionID;
            }
            /// <summary>
            /// 주어진 targetLineID에 해당하는 Line이 속한 Region의 RegionData를 반환한다
            /// </summary>
            /// <remarks>
            /// 만약 해당하는 LineID가 없을 경우 KeyNotFoundException을 발생시킨다
            /// </remarks>
            /// <exception cref="KeyNotFoundException"></exception>
            public static StageData.RegionData GetAttachedRegionData(in Guid targetLineID)
            {
                return StageDataBuffer.Instance.StageData.RegionDataTable[StageDataBuffer.Instance.StageData.LineDataTable[targetLineID].AttachedRegionID];
            }

            /// <summary>
            /// 주어진 targetLineID에 해당하는 LineData를 반환한다
            /// </summary>
            /// <remarks>
            /// 만약 해당하는 LineID가 없을 경우 KeyNotFoundException을 발생시킨다
            /// </remarks>
            /// <exception cref="KeyNotFoundException"></exception>
            public static StageData.LineData GetLineData(in Guid targetLineID)
            {
                return StageDataBuffer.Instance.StageData.LineDataTable[targetLineID];
            }

            /// <summary>
            /// 현재 StageData에 존재하는 모든 LineData의 LineID를 반환한다
            /// </summary>
            public static Guid[] GetLineIDs()
            {
                return StageDataBuffer.Instance.StageData.LineDataTable.Keys.ToArray();
            }

            internal static bool BIsLineDataValid(in Tuple<Guid, StageData.LineData> targetLineData)
            {
                if(targetLineData.Item1 == Guid.Empty)
                {
                    return false;
                }

                if (targetLineData.Item2.AttachedRegionID == Guid.Empty)
                {
                    return false;
                }
                else if(!RegionDataInterface.GetRegionIDs().Contains(targetLineData.Item2.AttachedRegionID))
                {
                    return false;
                }

                if (targetLineData.Item2.CurvedLinePoints.Count < 2)
                {
                    return false;
                }
                int prevLinePointYPos = -1;
                foreach (HalfFloatVector2 point in targetLineData.Item2.CurvedLinePoints)
                {
                    if (point.Y < prevLinePointYPos)
                    {
                        return false;
                    }
                    prevLinePointYPos = point.Y;
                }

                if (targetLineData.Item2.CurvedLinePoints.Count != targetLineData.Item2.MinorOffsetTimes.Count)
                {
                    return false;
                }
                foreach (float minorOffsetTime in targetLineData.Item2.MinorOffsetTimes)
                {
                    if (minorOffsetTime < -0.5f || 0.5f < minorOffsetTime)
                    {
                        return false;
                    }
                }

                if (targetLineData.Item2.LineWidth < 0.0f)
                {
                    return false;
                }

                if (!Enum.IsDefined(typeof(StageData.LineData.LineSmoothType), targetLineData.Item2.CurLineSmoothType))
                {
                    return false;
                }

                return true;
            }
            #endregion

#if FOR_EDITOR
            #region Setter
            public static bool TryAddLineData(in Guid targetLineID, in StageData.LineData targetLineData)
            {
                if (StageDataBuffer.Instance.StageData.LineDataTable.ContainsKey(targetLineID))
                {
                    return false;
                }

                StageDataBuffer.Instance.StageData.LineDataTable.Add(targetLineID, targetLineData);
                return true;
            }
            public static bool TryRemoveLineData(in Guid targetLineID)
            {
                if (!StageDataBuffer.Instance.StageData.LineDataTable.ContainsKey(targetLineID))
                {
                    return false;
                }

                StageDataBuffer.Instance.StageData.LineDataTable.Remove(targetLineID);
                return true;
            }

            public static bool TrySetLineData(in Guid targetLineID, in StageData.LineData targetLineData)
            {
                if (!StageDataBuffer.Instance.StageData.LineDataTable.ContainsKey(targetLineID))
                {
                    return false;
                }

                StageDataBuffer.Instance.StageData.LineDataTable[targetLineID] = targetLineData;
                return true;
            }
            #endregion
#endif
        }
        /// <summary>
        /// 현재 StageData의 NoteData에 접근하기 위한 Interface
        /// </summary>
        public static class NoteDataInterface
        {
            #region Getter
            /// <summary>
            /// 주어진 targetNoteID에 해당하는 Note가 속한 Region의 RegionID를 반환한다
            /// </summary>
            /// <remarks>
            /// 만약 해당하는 NoteID가 없을 경우 KeyNotFoundException을 발생시킨다
            /// </remarks>
            /// <exception cref="KeyNotFoundException"></exception>
            public static Guid GetAttachedRegionID(in Guid targetNoteID)
            {
                return StageDataBuffer.Instance.StageData.LineDataTable[StageDataBuffer.Instance.StageData.NoteDataTable[targetNoteID].AttachedLineID].AttachedRegionID;
            }
            /// <summary>
            /// 주어진 targetNoteID에 해당하는 Note가 속한 Region의 RegionData를 반환한다
            /// </summary>
            /// <remarks>
            /// 만약 해당하는 NoteID가 없을 경우 KeyNotFoundException을 발생시킨다
            /// </remarks>
            /// <exception cref="KeyNotFoundException"></exception>
            public static StageData.RegionData GetAttachedRegionData(in Guid targetNoteID)
            {
                return StageDataBuffer.Instance.StageData.RegionDataTable[StageDataBuffer.Instance.StageData.LineDataTable[StageDataBuffer.Instance.StageData.NoteDataTable[targetNoteID].AttachedLineID].AttachedRegionID];
            }

            /// <summary>
            /// 주어진 targetNoteID에 해당하는 Note가 속한 Line의 LineID를 반환한다
            /// </summary>
            /// <remarks>
            /// 만약 해당하는 NoteID가 없을 경우 KeyNotFoundException을 발생시킨다
            /// </remarks>
            /// <exception cref="KeyNotFoundException"></exception>
            public static Guid GetAttachedLineID(in Guid targetNoteID)
            {
                return StageDataBuffer.Instance.StageData.NoteDataTable[targetNoteID].AttachedLineID;
            }
            /// <summary>
            /// 주어진 targetNoteID에 해당하는 Note가 속한 Line의 LineData를 반환한다
            /// </summary>
            /// <remarks>
            /// 만약 해당하는 NoteID가 없을 경우 KeyNotFoundException을 발생시킨다
            /// </remarks>
            /// <exception cref="KeyNotFoundException"></exception>
            public static StageData.LineData GetAttachedLineData(in Guid targetNoteID)
            {
                return StageDataBuffer.Instance.StageData.LineDataTable[StageDataBuffer.Instance.StageData.NoteDataTable[targetNoteID].AttachedLineID];
            }

            /// <summary>
            /// 주어진 targetNoteID에 해당하는 Note가 존재하는지 확인한다
            /// </summary>
            /// <returns>
            /// 존재한다면 true, 그렇지 않다면 false
            /// </returns>
            public static bool BIsNoteIDValid(in Guid targetNoteID)
            {
                return StageDataBuffer.Instance.StageData.NoteDataTable.ContainsKey(targetNoteID);
            }
            /// <summary>
            /// 주어진 targetLineID에 해당하는 NoteData를 반환한다
            /// </summary>
            /// <remarks>
            /// 만약 해당하는 NoteID가 없을 경우 KeyNotFoundException을 발생시킨다
            /// </remarks>
            /// <exception cref="KeyNotFoundException"></exception>
            public static StageData.NoteData GetNoteData(in Guid targetNoteID)
            {
                return StageDataBuffer.Instance.StageData.NoteDataTable[targetNoteID];
            }

            /// <summary>
            /// 현재 StageData에 존재하는 모든 NoteData의 NoteID를 반환한다
            /// </summary>
            public static Guid[] GetNoteIDs()
            {
                return StageDataBuffer.Instance.StageData.NoteDataTable.Keys.ToArray();
            }
            #endregion

#if FOR_EDITOR
            #region Setter
            public static bool TryAddNoteData(in Guid targetNoteID, in StageData.NoteData targetNoteData)
            {
                if (StageDataBuffer.Instance.StageData.NoteDataTable.ContainsKey(targetNoteID))
                {
                    return false;
                }

                StageDataBuffer.Instance.StageData.NoteDataTable.Add(targetNoteID, targetNoteData);
                return true;
            }
            public static bool TryRemoveNoteData(in Guid targetNoteID)
            {
                if (!StageDataBuffer.Instance.StageData.NoteDataTable.ContainsKey(targetNoteID))
                {
                    return false;
                }

                StageDataBuffer.Instance.StageData.NoteDataTable.Remove(targetNoteID);
                return true;
            }

            public static bool TrySetNoteData(in Guid targetNoteID, in StageData.NoteData targetNoteData)
            {
                if (!StageDataBuffer.Instance.StageData.NoteDataTable.ContainsKey(targetNoteID))
                {
                    return false;
                }

                StageDataBuffer.Instance.StageData.NoteDataTable[targetNoteID] = targetNoteData;
                return true;
            }
            #endregion
#endif
        }
        /// <summary>
        /// 현재 StageData의 StageConfigData에 접근하기 위한 Interface
        /// </summary>
        public static class StageConfigDataInterface
        {
            #region Getter
            /// <summary>
            /// 현재 StageData의 StageConfigData를 반환한다
            /// </summary>
            public static StageData.StageConfigData GetStageConfigData()
            {
                return StageDataBuffer.Instance.StageData.StageConfig;
            }
            #endregion

#if FOR_EDITOR
            #region Setter
            public static void SetStageConfigData(in StageData.StageConfigData targetStageConfigData)
            {
                StageDataBuffer.Instance.StageData = new StageData()
                {
                    RegionDataTable = StageDataBuffer.Instance.StageData.RegionDataTable,
                    LineDataTable = StageDataBuffer.Instance.StageData.LineDataTable,
                    NoteDataTable = StageDataBuffer.Instance.StageData.NoteDataTable,
                    StageConfig = targetStageConfigData
                };
            }
            #endregion
#endif
        }


#if FOR_EDITOR
        public static bool TrySaveStageData(in string targetStageName, in StageMetadata.MajorDifficultyLevel majorDifficulty)
        {
            return StageDataBuffer.Instance.TrySaveStageData(targetStageName, majorDifficulty);
        }
#endif
        /// <summary>
        /// DataPathTable의 내용을 바탕으로 Load를 시도한다
        /// </summary>
        /// <returns>
        /// 만약 Load에 성공했을 경우 true, 실패했을 경우 false
        /// </returns>
        public static bool TryLoadStageData(in string targetStageName, in StageMetadata.MajorDifficultyLevel majorDifficulty)
        {
            return StageDataBuffer.Instance.TryLoadStageData(targetStageName, majorDifficulty);
        }
        /// <summary>
        /// StageData를 Dispose한다
        /// </summary>
        public static void DisposeStageData()
        {
            StageDataBuffer.Instance.Dispose();
        }

        /// <summary>
        /// StageData가 유효한지 확인한다
        /// </summary>
        /// <returns>
        /// 유효하다면 true, 그렇지 않다면 false
        /// </returns>
        public static bool BIsStageDataValid()
        {
            return StageDataBuffer.Instance.StageData.BIsValid();
        }
    }
}
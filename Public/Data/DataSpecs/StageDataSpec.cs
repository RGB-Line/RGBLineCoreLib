using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RGBLineCoreLib.Data
{
    /// <summary>
    /// 각 Stage의 내부 구성 데이터를 담는 구조체
    /// </summary>
    /// <remarks>
    /// DTO로서만 사용되며, 별도의 Interface를 거치지 않은 접근은 권장하지 않음
    /// </remarks>
    public struct StageData
    {
        /// <summary>
        /// 각 Region의 데이터를 담는 구조체
        /// </summary>
        public struct RegionData
        {
            /// <summary>
            /// 세 가지 Mode를 구분하는 Flag
            /// </summary>
            public enum ColorType : byte
            {
                Red, Green, Blue
            }


            /// <summary>
            /// 해당 Region이 시작하는 Frame 단위 위치를 의미
            /// </summary>
            public int StartOffsetFrame;
            /// <summary>
            /// RegionData.StartOffsetFrame을 기준으로 FrameUnitSize * MinorOffsetTime 만큼 이동한 위치를 의미. 즉, 세부 위치 조정을 담당
            /// </summary>
            /// <remarks>
            /// 허용 범위는 [-0.5, 0.5]
            /// </remarks>
            public float MinorOffsetTime;

            /// <summary>
            /// 해당 Region의 Mode를 의미
            /// </summary>
            public ColorType CurColorType;
        }
        /// <summary>
        /// 각 Line의 데이터를 담는 구조체
        /// </summary>
        public struct LineData
        {
            /// <summary>
            /// 해당 Line의 부드러운 정도를 나타내는 Flag
            /// </summary>
            public enum LineSmoothType : byte
            {
                /// <summary>
                /// 딱딱하게 꺽이는 선분들의 형태로 표현
                /// </summary>
                Linear,
                /// <summary>
                /// 부드럽게 꺽이는 곡선의 형태로 표현
                /// </summary>
                Curved
            }


            /// <summary>
            /// 해당 Line이 속한 Region의 ID
            /// </summary>
            public Guid AttachedRegionID;

            /// <summary>
            /// X - 좌우 위치, Y - 해당 Line Point가 위치하는 Frame 단위 위치
            /// </summary>
            /// <remarks>
            /// 기본 가정으로 CurvedLinePoints는 Y 좌표 기준으로 정렬되어 있으며, Y좌표는 헤당 Line이 속한 Region의 StartOffsetFrame에 더해져 최종 위치를 결정
            /// </remarks>
            public List<HalfFloatVector2> CurvedLinePoints;
            /// <summary>
            /// LineData.CurvedLinePoints에 보조적으로, Line Point의 Y축 위치 세부 조정을 의미
            /// </summary>
            /// <remarks>
            /// 기본 가정으로 MinorOffsetTimes의 모든 요소는 CurvedLinePoints의 요소와 1:1 대응하며, MinorOffsetTimes의 각 요소의 하용 범위는 [-0.5, 0.5]
            /// </remarks>
            public List<float> MinorOffsetTimes;
            /// <summary>
            /// 해당 Line의 두께를 의미
            /// </summary>
            public float LineWidth;

            /// <summary>
            /// 해당 Line이 직선적인지, 곡선적인지를 묘사
            /// </summary>
            public LineSmoothType CurLineSmoothType;
        }
        /// <summary>
        /// 각 Note의 데이터를 담는 구조체
        /// </summary>
        public struct NoteData
        {
            /// <summary>
            /// 세 가지 Note Type을 구분하는 Flag
            /// </summary>
            public enum NoteType : byte
            {
                Common,
                Flip,
                Long
            }
            /// <summary>
            /// 해당 Note가 Flip Note일 때, Flip Note의 방향을 나타내는 Flag
            /// </summary>
            public enum FlipNoteDirection
            {
                Left, Right
            }


            /// <summary>
            /// 해당 Note가 속한 Line의 ID
            /// </summary>
            public Guid AttachedLineID;

            /// <summary>
            /// 해당 Note가 시작하는 Frame 단위 위치를 의미
            /// </summary>
            /// <remarks>
            /// Y좌표는 해당 Note가 속한 Line의 첫 번쩨 CurvedLinePoints 요소의 Y좌표에 더해져 최종 위치를 결정
            /// </remarks>
            public int StartOffsetFrame;
            /// <summary>
            /// NoteData.StartOffsetFrame을 기준으로 FrameUnitSize * MinorOffsetTime 만큼 이동한 위치를 의미. 즉, 세부 위치 조정을 담당
            /// </summary>
            /// <remarks>
            /// 허용 범위는 [-0.5, 0.5]
            /// </remarks>
            public float MinorOffsetTime;

            /// <summary>
            /// 해당 Nore가 Long Note일 때, Note의 길이를 의미
            /// </summary>
            public float NoteLength;
            /// <summary>
            /// 해당 Note가 Flip Note일 때, Flip Note의 방향을 나타냄
            /// </summary>
            public FlipNoteDirection flipNoteDirection;

            /// <summary>
            /// 해당 Note의 Type을 나타냄
            /// </summary>
            public NoteType CurNoteType;
        }
        /// <summary>
        /// 각 Stage의 설정 데이터를 담는 구조체
        /// </summary>
        public struct StageConfigData
        {
            /// <summary>
            /// 해당 Stage의 BPM을 나타냄
            /// </summary>
            /// <remarks>
            /// BPM이 크면 화면 진행 속도가 빨라지며, BPM이 작으면 화면 진행 속도가 느려짐
            /// </remarks>
            public int BPM;
            /// <summary>
            /// BPM을 기준으로 한 Frame을 세부적으로 몇 번 더 나눌 것인지를 나타냄. 즉, 새로운 세부 Frame을 만들어내는 배율을 의미
            /// </summary>
            public int BitSubDivision;
            /// <summary>
            /// 각 Frame이 In-Game에서 어느 정도의 길이를 가지는지를 나타냄
            /// </summary>
            /// <remarks>
            /// LengthPerBit가 크면 화면 진행 속도가 빨라지며, LengthPerBit가 작으면 화면 진행 속도가 느려짐
            /// </remarks>
            public float LengthPerBit;
            /// <summary>
            /// 해당 Stage의 Music이 시작하는 시간을 나타냄
            /// </summary>
            public float MusicStartOffsetTime;
        }


        /// <summary>
        /// RegionData와 그에 대응하는 ID를 담은 Table
        /// </summary>
        /// <remarks>
        /// 기본 가정으로 RegionDataTable는 StartOffsetFrame 기준으로 정렬되어 있음
        /// </remarks>
        public Dictionary<Guid, RegionData> RegionDataTable;
        /// <summary>
        /// LineData와 그에 대응하는 ID를 담은 Table
        /// </summary>
        /// <remarks>
        /// 기본 가정으로 LineDataTable는 정렬되지 않은 상태
        /// </remarks>
        public Dictionary<Guid, LineData> LineDataTable;
        /// <summary>
        /// NoteData와 그에 대응하는 ID를 담은 Table
        /// </summary>
        /// <remarks>
        /// 기본 가정으로 NoteDataTable은 정렬되지 않은 상태
        /// </remarks>
        public Dictionary<Guid, NoteData> NoteDataTable;

        public StageConfigData StageConfig;
    }
}
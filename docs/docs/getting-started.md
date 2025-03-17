# Getting Started

본 라이브러리를 활용하기 위해서는 다음의 단계를 따르는 것이 좋다

1. Dependency에 해당하는 dll을 구해 Unity Project에 Import한다
2. <code>RGBLineCoreLib.dll</code>을 Unity Project에 Import한다

dll을 plugin으로서 Unity Project에 Import할 때에는 <a href="https://docs.unity3d.com/6000.0/Documentation/Manual/plug-ins.html">Unity의 "Integrating third-party code libraries (plug-ins)" 문서</a>에 따라 Assets 폴더 아래에 Plugins 폴더를 만든 뒤, Pligins 아래에 모든 dll을 두는 것이 좋다

RGBLineCoreLib의 Dependency 목록은 다음과 같다

```
UnityEngine.dll
CommonUtilLib.dll
Newtonsoft.Json.dll
```

이 중, <code>UnityEngine.dll</code>은 따로 Import할 필요 없으며, <code>CommonUtilLib.dll</code>은 다음의 <a href="https://github.com/GameProj-Forgotten/CommonUtilLib/releases">CommonUtilLib Github Repo</a>에서 구할 수 있는 것을 사용하면 된다

## 기본적인 활용법

### 데이터 읽기/쓰기

RGBLineCoreLib에서는 Data를 담고 관리하는 핵심 Buffer는 대게 직접 접근이 불가하며, 이는 가장 기초적인 Data 무결성을 유지하기 위한 조치이다

때문에 대신하여 적절한 Interface Class를 두어 간접적으로 Data에 접근할 수 있게 하고 있다

RGB Line Project의 핵심 Data 종류는 다음과 같다
```
RGB Line Core Datas
├── StageData
│   ├── RegionData
│   ├── LineData
│   ├── NoteData
│   └── StageConfigData
├── StageMetadata
└── GameConfigData
```

각각의 Data 종류에 대해 제공되는 Interface Class는 다음과 같다

```
RGB Line Core Datas
├── StageData               - StageDataInterface
│   ├── RegionData          - StageDataInterface.RegionDataInterface
│   ├── LineData            - StageDataInterface.LineDataInterface
│   ├── NoteData            - StageDataInterface.NoteDataInterface
│   └── StageConfigData     - StageDataInterface.StageConfigDataInterface
├── StageMetadata           - StageMetadataInterface
└── GameConfigData          - GameConfigDataBuffer
```

<code>GameConfigData</code>에 대해서는 예외적으로 Buffer에 직접 접근하는 것을 허용한다. 다만, Editor에서는 <code>GameConfigData</code>를 직접 조작할 일이 없을 것으라 예상됨으로 크게 신경쓸 필요는 없다

각 Interface Class가 제공하는 기능은 본 문서의 API 탭을 참고하길 바란다

### 클래스 상속

RGBLineCoreLib의 거의 모든 Class는 sealed, 즉, 상속 불가하게끔 구현되어 있다. 다만, 다음의 것들에 대해서만 예외적으로 상속이 가능하게 제약이 풀려있으며, Editor에서는 이를 바탕으로 구현하면 될 것이다

```
public class LineItem : MonoBehaviour, ILineItem
public class LinePoint : MonoBehaviour, ILinePoint

public class NoteItem : MonoBehaviour, INoteItem
public class RedAndBlueNote : MonoBehaviour, IRedAndBlueNote
public class GreenNote : MonoBehaviour, IGreenNote
public class RedLineCornerNote : MonoBehaviour, IRedLineCornerNote
```

위의 Class들에는 여러 virtual Method들이 포함되어 있는데, 이들의 기본 구현 사항은 Main Game에서 사용되는 것과 같은 사양이다

따라서 Editor 측에서는 요구사항에 따라 다음과 같은 선택지가 있을 것이다

1. Main Game에서 보여지는 것과 똑같이 Rendering되기만을 바란다면 override를 하지 않아도 된다
2. Main Game에서 보여지는 것과 같되, 여기서 추가적인 요소들을 추가하고 싶다면 override 한 뒤, base의 virtual 버전의 Method를 다시 호출해주면 될 것이다
3. 원한다면 각 virtual Method들을 처음부터 다시 구현하고 base의 virtual 버전의 Method는 호출하지 않으면 된다. 이때 각 기능의 구현은 <a href="https://github.com/RGB-Line/RGBLineCoreLib">RGBLineCoreLib Github Repo</a>의 원본 코드를 참고하면 편할 것이다
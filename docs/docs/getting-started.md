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

이 중, <code>UnityEngine.dll</code>은 따로 Import할 필요 없으며, <code>CommonUtilLib.dll</code>은 다음의 <a href="https://github.com/GameProj-Forgotten/CommonUtilLib/releases">CommonUtilLib Github Repo Release Page</a>에서 구할 수 있는 최신의 것을 사용하면 된다

<code>Newtonsoft.Json.dll</code>은 <a href="https://www.newtonsoft.com/json">Newtonsofe Homepage</a>에서 다운로드 받으면 될 것이다

<code>RGBLineCoreLib.dll</code>과 <code>RGBLineCoreLib_ForEditor.dll</code>은 <a href="https://github.com/RGB-Line/RGBLineCoreLib/releases">RGBLineCoreLib Github Repo Release Page</a>에서 구할 수 있는 최신의 것을 사용하면 된다
# Editor Dev Manual

## 권장 기본 개발 방향

Editor를 편히 개발하려면 기본적으로 Main Game의 주 Game Screen의 기능을 내포하도록 하는 것이 편할 것이다. 즉, <code>RegionManager</code>, <code>LineManager</code>, <code>NoteManager</code>, <code>ScoreManager</code>, <code>GridManager</code>를 배치하고, <code>ChiefMainGameManager</code>를 조금 수정하여 사용하는 것이다

이러한 바탕 위에 Editor용 기능 --- Data Set 관련 기능을 추가하면 되는 것이다

## 주의점

Prototype 버전의 Editor의 경우 Codebase가 현재의 RGBLineCoreLib로 옮겨지면서 Prototype의 Code가 맞지 않는 경우가 생긴다

따라서 Prototype의 Code를 그대로 적용하는 것은 위험하다

## Rendering에 추가 장식 등 추가

기본적으로 Editor의 경우 다음의 class들에 기능을 추가 가능하도록 열려있다

```
LineItem
├── [RequireComponent(typeof(CurvedLineRenderer))] class LineItem : MonoBehaviour, ILineItem
├── [RequireComponent(typeof(CurvedLinePoint))] class LinePoint : MonoBehaviour, ILinePoint
└── class RedLineCornerNote : MonoBehaviour, IRedLineCornerNote

NoteItem
├── class NoteItem : MonoBehaviour, INoteItem
├── class RedAndBlueNote : MonoBehaviour, IRedAndBlueNote
└── class GreenNote : MonoBehaviour, IGreenNote
```

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
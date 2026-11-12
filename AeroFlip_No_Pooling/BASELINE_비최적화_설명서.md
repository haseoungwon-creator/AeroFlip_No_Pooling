# AeroFlip 비최적화 기준 버전 (AeroShift 비교용)

이 프로젝트는 사용자가 제공한 `AeroFlip-main.zip`을 기반으로 **게임 로직은 유지하되 게임플레이 오브젝트 풀링과 되감기 Circular Buffer를 제거**한 비교용 프로젝트입니다.

## 수정한 항목

| 대상 | 기존 | 비최적화 기준 버전 |
|---|---|---|
| 구름 | `CloudPool`에서 800개 사전 생성·재사용 | `CloudFactory.CreateCloud()`에서 매번 `Instantiate`, 범위를 지나면 `Destroy` |
| 일반/안전 맵 | `MapPool`에서 타입별 사전 생성·재사용 | `MapFactory.CreateMap` / `CreateSafeMap`에서 즉시 생성, 사용 후 `Destroy` |
| 차원 전환 벽 | `WallPool` 한 개 오브젝트 재사용 | `WallFactory`에서 매번 생성·삭제 |
| 미사일 | `MissilePool`의 Queue 재사용 | `MissileFactory.CreateMissile`/`DestroyMissile`로 생성·삭제 |
| 미사일 경고 UI | `MissileWarningPool`의 Queue 재사용 | `MissileWarningFactory`에서 생성·삭제 |
| 되감기 스킬 | 고정 길이 배열 Circular Buffer | 일반 `List<PositionData>`에 추가, 오래된 항목 `RemoveAt(0)` |
| 구름 목록 | `List<GameObject>(800)`로 초기 용량 예약 | 기본 `new List<GameObject>()` 사용 |
| TEST 씬 | 풀링 테스트 스크립트 활성화 | 비풀링 테스트 스크립트만 활성화 |

추가로 사용하지 않던 `MapManager_Unoptimized` 컴포넌트와 별도 풀링 테스트 스크립트는 삭제했습니다. 기존 풀 스크립트 GUID는 새 Factory 스크립트로 이전하여 씬의 프리팹 참조가 유지되도록 했습니다. `Main.unity` 및 Recovery 씬의 직렬화된 컴포넌트/필드 이름도 변경했습니다.

`AeroOptimizationExperiment`, `AeroFlipGCAllocationCapture`, `PerformanceMonitor` 등은 **측정·관찰용 코드**이므로 삭제하지 않았습니다. 이들은 풀링 최적화 코드가 아닙니다.

## 프로젝트 열기

1. Unity Hub에서 `AeroFlip` 폴더를 Unity 프로젝트로 엽니다. 원본 프로젝트 버전은 **Unity 6000.3.15f1**입니다.
2. `Assets/Scenes/Main.unity`를 열고 Console의 컴파일 오류와 Missing Script 여부를 확인하세요.
3. Play에서 구름 생성/삭제, 3D 맵 순환, 2D 차원 이동과 복귀, 미사일 경고/발사/삭제, 게임 오버/재시작, 되감기 스킬을 확인하세요.
4. `Assets/Scenes/TEST.unity`에는 비풀링 큐브 스포너만 활성화되어 있습니다. 이 씬은 비교용 실험입니다.

> 주의: 이 파일은 소스 및 Unity YAML의 정적 참조 검증을 거쳤지만 Unity 에디터에서 직접 실행·빌드 검증한 결과물은 아닙니다. 필요하면 Unity에서 발생한 Console 로그를 기반으로 추가 검증이 필요합니다.

## AeroShift와 성능 비교 조건

- **동일한 PC/노트북, 해상도, VSync, TargetFrameRate, 빌드 방식(Player/Editor), 품질/URP 옵션**으로 측정하세요. 서로 다른 기기에서 측정한 FPS는 직접 비교하지 않습니다.
- 두 게임의 **생성 수, 이동 속도, 게임플레이 시간 및 2D/3D 상태**를 가능한 범위에서 맞추세요. 현재 프로젝트는 AeroFlip이고, AeroShift는 별도 프로젝트이므로 두 게임의 에셋/맵/연출이 다르면 총 FPS의 차이를 최적화 효과로 단정할 수 없습니다.
- Profiler에서 CPU Main Thread, GC Allocated In Frame, GC Used Memory, Total Used Memory, GameObject Count, Draw Calls, Batches와 Instantiate/Destroy 관련 스파이크를 관찰하세요.
- 초기 생성 구간과 안정 구간을 구분해 측정하세요. 원본 풀링은 시작 시 미리 생성해 초기 CPU/메모리를 더 사용했고, 이 버전은 플레이 중 생성·삭제합니다.
- 특히 **풀링 전/후 자체 효과**를 엄밀히 증명하려면 *같은 AeroFlip 콘텐츠*에 비풀링/풀링을 적용한 A/B 측정이 필요합니다. AeroShift와 AeroFlip을 그대로 비교하면 프로젝트 차이가 혼재합니다.

## 변경 파일

- `Assets/Script/System/CloudSpawner.cs`, `CloudFactory.cs`
- `Assets/Script/World/MapManager.cs`, `MapFactory.cs`, `WallFactory.cs`
- `Assets/Script/Missile/Missile.cs`, `MissileSpawner.cs`, `MissileWarning.cs`, `MissileFactory.cs`, `MissileWarningFactory.cs`
- `Assets/Script/Skill/PlayerRewind.cs`
- `Assets/Scenes/Main.unity`, `Assets/Scenes/TEST.unity`, `Assets/_Recovery/0 (1).unity`

원본의 `Aero.unitypackage`는 **기존 스크립트가 포함된 과거 스냅샷**이므로 제공 ZIP에는 넣지 않았습니다. 원본 unitypackage를 다시 Import하면 비최적화 코드가 덮어써질 수 있습니다.

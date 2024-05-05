## 1주차 (5강/86강 수강률 5.81% : 38,346원)
### 요약
- 프로젝트 생성
- 패키지 매니징
- 각종 코어 매니저 생성
- 간단한 메인 Scene

### 1. 프로젝트 생성
- 기본 2D 프로젝트 생성
- 디렉토리 생성(@suffix는 addressable 패키지 매니저 사용을 위함)
  - @Resources
    - 예하 Data, Font, Prefabs, Sounds, Sprites(Spines) 등의 리소스 저장 디렉토리
    - Data는 Excel(csv) 원본 데이터 저장하는 디렉토리
  - @Scenes
    - 게임 Scene 저장 디렉토리
  - @Scripts
    - 예하 Contents, Controllers, Data, Editor, Managers, Scenes, UI, Utils 등의 C# 스크립트 저장 디렉토리
    - Data는 원본 데이터의 형태 정의 등을 위한 디렉토리
    - Editor은 유니티 에디터에 추가기능을 적용하기 위한 디렉토리 (csv파일 json 변환 등)
    - Managers는 각 리소스에 대응하는 호출 재생 삭제 관리 등 매니징 스크립트를 위한 디렉토리
    - Scenes는 각 게임 Scene에 할당되는 스크립트.
    - UI는 각 씬에 할당된 UI를 핸들링(이벤트 바인딩, active 여부 수정 등)  하기 위한 스크립트.
    - Utils는 공통 메서드나 이넘 등을 위한 디렉토리
### 2. 패키지 매니징
- addressable 설치
  - 리소스 빌드 효율성 관련 패키지
  - 원하는 위치에서 리소스를 호출 할 수 있게 하여 로컬에 에셋을 함께 저장할 필요를 줄여줌(웹서버 등)
  - 이는 빌드에 리소스를 모두 담을 필요가 없게 함.(리소스 변경 등을 위한 추가 빌드 필요 없음. - 앱스토어 심사 패스)
  - 이는 운영에서 실시간 패치에 큰 도움이 됨.
- spine-unity 설치
  - spine 사용을 위한 패키지
  - sprites가 전통적인 2D 이미징이지만 한 동작을 새로 만들때마다 큰 작업을 필요로 함.
  - spine은 하나의 이미지를 골격단위로 분리하여 만드는 애니메이션으로 동작을 수정하기 용이함.
- newtonsoft-json 설치
  - Data리소스를 위한 Json 직렬화 등 관리 패키지
### 3. 코어 매니저 생성
- Data 매니저 : Scripts/Data를 참조하여 Resources/Data의 데이터를 로드하고 사용하는 매니저.
- Pool 매니저 : prefab 다루기 위한 매니저
- Resource 매니저 : 리소스 호출을 위한 매니저
- Scene 매니저 : 씬 이동, 이름확인 등을 위한 매니저
- Sound 매니저 : Sound 리소스 호출 및 재생 중지 매니저
- UI 매니저 : 캔버스 생성 및 UI 관리 매니저
- 코어 매니저 : 위의 6가지 매니저를 포함하는 인스턴스 생성
### 4. 간단한 title Scene
- 메인 Scene의 역할
  - Resource 매니저를 이용하여 addressable에 등록된 preLoad Resource 호출하기
    ```c#
    Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) => {
      Debug.Log($"{key} {count}/{totalCount}");
    }
    ```
  - 호출이 완료되면 touch to start 노출하기
  - touch to start가 보일때 화면 누르면 다음 페이지로 넘어가기

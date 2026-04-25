# GEMINI_GUIDE.md

이 가이드는 AI 코딩 어시스턴트가 이 프로젝트의 구조와 설계를 이해하고, 일관성 있는 코드를 작성할 수 있도록 돕기 위해 작성되었습니다.

## 1. 프로젝트 개요 및 기술 스택
본 프로젝트는 **2D Top-down RPG** 게임으로, 고성능 C++ 서버와 Unity 클라이언트로 구성되어 있습니다.

- **Client**: Unity (C#), 2D Sprite 기반, Universal Render Pipeline (URP)
- **Server**: C++ (Windows IOCP), Multi-threaded
- **Networking**: TCP/IP, Google FlatBuffers (Serialization)
- **Database**: MySQL (ODBC 연결)
- **Architecture**: Job Queue 기반 비동기 로직 처리, Service-Repository 패턴 기반 DB 연동

---

## 2. 폴더 구조 및 역할

### [Root]
- `Client/`: Unity 클라이언트 프로젝트
- `Server/`: 서버 솔루션 폴더

### [Client/Assets]
- `01.Scenes/`: 게임의 주요 씬 (Login, Register, GameScene)
- `02.Scripts/`: 주요 게임 로직 및 네트워크 코드
  - `NetworkManager.cs`: TCP 연결 및 데이터 송수신 관리
  - `PacketHandler.cs`: 수신된 패킷의 ID에 따른 처리 로직 (FlatBuffers 디코딩)
  - `Player/Move/Action.cs`: 플레이어 컨트롤 및 물리 로직
- `flatbuffers/`: FlatBuffers 관련 라이브러리 및 생성된 스키마 코드

### [Server/ServerCore]
- 서버의 기반 프레임워크 (재사용 가능한 핵심 라이브러리)
- `IocpCore.h/cpp`: IOCP 핵심 엔진
- `Session.h/cpp`: 네트워크 세션 관리 (Send/Recv)
- `DBConnectionPool.h/cpp`: 데이터베이스 연결 관리
- `ThreadManager.h/cpp`: 워커 스레드 관리
- `Job.h/cpp`: 비동기 작업을 위한 Job Queue 시스템

### [Server/GameServer]
- 실제 게임 비즈니스 로직
- `Room.h/cpp`: 게임 월드/방 단위 로직 관리 (플레이어 입장, 이동, 패킷 전송)
- `Player/User.h/cpp`: 유저 및 캐릭터 데이터 구조체
- `ClientPacketHandler.h/cpp`: 클라이언트로부터 온 패킷 처리 및 응답 생성
- `*Service.h/cpp`: 비즈니스 로직 (예: LoginService, InventoryService)
- `*Repository.h/cpp`: DB 접근 로직 (CRUD)

---

## 3. 네트워크 통신 방식 및 패킷 구조

### 통신 프로토콜
- **TCP/IP**를 사용하며, 고성능 비동기 I/O를 위해 Windows **IOCP**를 활용합니다.
- 데이터 직렬화(Serialization)는 **Google FlatBuffers**를 사용하여 성능과 보안성을 확보합니다.

### 패킷 구조 (PacketHeader)
모든 패킷은 고정된 크기의 헤더로 시작합니다.
```cpp
struct PacketHeader {
    uint16 size; // 패킷 전체 크기
    uint16 id;   // 패킷 식별자 (ID)
};
```
헤더 이후에는 FlatBuffers로 직렬화된 데이터가 뒤따릅니다.

### 패킷 처리 흐름
1. **Client -> Server**:
   - `NetworkManager.SendPacket` 호출 -> FlatBuffers 빌드 -> 헤더 추가 -> TCP 전송
   - Server `IocpCore`에서 수신 -> `GameSession`에 데이터 전달 -> `ClientPacketHandler`에서 ID 확인 후 대응하는 `Handle_XXX` 함수 실행
2. **Server -> Client**:
   - `SendBuffer` 생성 -> FlatBuffers 빌드 -> `Session->Send()` 호출
   - Client `NetworkManager.OnReceiveData` -> `PacketHandler.HandlePacket`에서 처리

---

## 4. 서버 핵심 아키텍처

### IOCP (Input/Output Completion Port)
- 다수의 워커 스레드가 `IocpCore::Dispatch`를 호출하여 비동기 입출력을 처리합니다.
- 입출력 완료 시 `OnRecv` / `OnSend` 콜백이 트리거됩니다.

### Job Queue 기반 로직 처리
- 멀티스레드 환경에서 데이터 경합(Race Condition)을 방지하기 위해 직접적인 Lock 사용을 지양하고 **Job Queue** 패턴을 사용합니다.
- `Room` 객체는 자체적인 Job Queue를 가지며, `FlushJob()`을 통해 순차적으로 로직을 처리합니다.
- 모든 게임 로직(이동, 전투, 채팅 등)은 Job으로 캡슐화되어 해당 구역(Room 등)의 큐에 push된 후 처리됩니다.

### 데이터베이스 (DB Layer)
- **Service-Repository** 패턴을 사용합니다.
- `Service`: 비즈니스 로직 검증 (예: 아이템 사용 가능 여부 확인)
- `Repository`: 순수 DB 쿼리 실행 (예: 아이템 개수 업데이트)
- DB 작업은 블로킹을 방지하기 위해 별도의 DB 스레드 또는 비동기 방식으로 처리하는 것을 지향합니다.

---

## 5. 코딩 가이드라인

1. **메모리 관리**: 서버에서는 스마트 포인터(`shared_ptr`, `unique_ptr`) 사용을 생활화하며, `ServerCore`에 정의된 `MakeShared` 등을 활용합니다.
2. **패킷 추가 시**:
   - `ClientPacketHandler.h`의 `enum`에 새로운 `PKT_ID` 정의
   - `Handle_XXX` 함수 선언 및 `Init()`에 등록
   - Unity 클라이언트의 `PacketHandler.cs` 및 `NetworkManager.cs`에 대응 로직 추가
3. **스레드 안전성**: 게임 로직을 수정할 때는 해당 로직이 어떤 스레드에서 실행되는지 반드시 확인하고, 필요 시 Job Queue를 통해 처리하도록 설계합니다.

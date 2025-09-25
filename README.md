# 👯‍♀️2D Top down MMORPG

'바람의 나라' 일부를 클론 코딩합니다.

<br/>

## 🥅 Goal

- Windows IOCP 방식의 서버로 수많은 유저가 원활한 게임을 진행할 수 있습니다.
- 유니티를 사용해서 유저가 직접 동작하는 모습을 볼 수 있습니다.

<br/>

## 😎 Preview

<html>
<table>
  <tr>
    <th>
      첫 메인 화면
    </th>
    <th>
      회원가입 화면
    </th>
    <th>
      로그인 후 캐릭터 화면
    </th>
  </tr>
  <tr>
    <td>
      <img width="100%" alt="Main Image" src="https://github.com/user-attachments/assets/7cb6a4fd-f2a6-47b1-bf4f-862a23439a4f" />
    </td>
    <td>
      <img width="100%" alt="Signup Image" src="https://github.com/user-attachments/assets/94db0ef7-302a-441b-98eb-e1dbcbda69ac" />
    </td>
    <td>
      <img width="100%" alt="Character Select Image" src="https://github.com/user-attachments/assets/2ef8865d-194f-4812-b7de-e058d372a164" />
    </td>
   </tr> 
  <tr>
    <th>
      캐릭터 생성 화면
    </th>
    <th>
      게임 입장 후 화면
    </th>
    <th>
      엔피씨 클릭 화면
    </th>
  </tr>
  <tr>
    <td>
      <img width="100%" alt="Create Char Image" src="https://github.com/user-attachments/assets/8df2a0a0-ee7d-442c-b9e2-15c8ba3a81c4" />
    </td>
    <td>
      <img width="100%" alt="Enter Game Image" src="https://github.com/user-attachments/assets/b8a22fca-a31e-4e00-a48a-1906aaa2b592" />
    </td>
    <td>
      <img width="100%" alt="Npc Click Image" src="https://github.com/user-attachments/assets/8cdef54b-4b8a-4388-85c5-367665dc75f5" />
    </td>
   </tr>
   <tr>
    <th>
      물품 구매 화면
    </th>
    <th>
      물품 판매 화면
    </th>
    <th>
      동기화 화면
    </th>
  </tr>
  <tr>
    <td>
      <img width="100%" alt="Npc Buy Image" src="https://github.com/user-attachments/assets/79a91a81-da7e-4a9c-bc07-085698287a6b" />
    </td>
    <td>
      <img width="100%" alt="Npc Sell Image" src="https://github.com/user-attachments/assets/dca309c9-ae9a-4f89-afaa-aa2fb17cdfdc" />
    </td>
    <td>
      <img width="100%" alt="Sync Player Image" src="https://github.com/user-attachments/assets/af4705fe-2c13-4808-8867-61f8651f4803" />
    </td>
    <tr>
    <th>
      다른 유저 클릭 화면
    </th>
    <th>
      거래창 아이템 추가 화면
    </th>
    <th>
      거래 수락 화면
    </th>
    <th>
    </th>
  </tr>
  <tr>
    <td>
      <img width="100%" alt="Click Other Player Image" src="https://github.com/user-attachments/assets/c0f0259e-6f61-48c9-bde6-989b9da49384" />
    </td>
    <td>
      <img width="100%" alt="Trade Additem Image" src="https://github.com/user-attachments/assets/ad106d82-4212-4f25-96b1-83feba7a9e85" />
    </td>
    <td>
      <img width="100%" alt="Accept Trade Image" src="https://github.com/user-attachments/assets/6232f584-df4d-4a7a-be99-bf0aa4f938c8" />
    </td>
   </tr>
  <tr>
    <th>
      거래 완료 후 화면
    </th>
    <th>
    </th>
  </tr>
  <tr>
    <td>
      <img width="100%" alt="Success Trade Image" src="https://github.com/user-attachments/assets/21b2dbb3-98f7-455b-b815-fad5e5a62510" />
    </td>
    <td>
    </td>
    <td>
    </td>
   </tr> 
</table>
</html>

<br/>

## 🏆 Advanced Achivements

### client

* 타일맵을 사용한 맵 제작
* 아이템 데이터 관리는 ScriptableObject 클래스 사용
* 골드나 아이템 갯수 등 소유한 양보다 많은지 검사 후 데이터 전송
* 사용자 경험을 위해 아이템이나 버튼 클릭 시 색상 변경 적용
* 캐릭터 이동 기능 구현
* NPC에게 아이템 구매 및 판매 기능 구현
* 다른 캐릭터 프리팹 생성 및 이동 애니메이션 적용
* 다른 캐릭터 클릭 후 정보 보기 및 거래 기능 구현
* 서버로 부터 받은 데이터로 내 정보, 채팅 메시지, 이동 적용

### server

* 서버 IOCP 적용 및 패킷 설계
* 데이터 관리는 MySQL 데이터베이스 사용
* ERD Diagram으로 테이블 설계 및 연관관계 매핑
* 회원 가입 및 로그인 기능 구현
* 캐릭터 생성 및 캐릭터 게임 입장 기능 구현
* 캐릭터 정보 및 인벤토리 정보 응답 기능 구현
* 다른 유저 정보 응답 기능 구현
* 이동 동기화 및 채팅 기능 구현
* 유저 간에 아이템 거래 기능 구현

<br/>

## 📚 Project Details Wiki

* [Wiki 페이지]([https://www.notion.so/Pre-Project-3d380dd015e54a7b8ce2a30d03a9af27?p=871197baab2c4951a8456c87a0aed09e&pm=c](https://github.com/paquantum/2d-game-project/wiki/Project-details)

<br/>

## 🕹 Skiils
category|skills
:---:|:---:
Client| Unity, C#
Server| Visual Studio C/C++, MySQL
Library| Flatbuffers

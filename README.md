# 👯‍♀️2D Top down MMORPG

'바람의 나라' 일부를 클론 코딩합니다.

<br/><br/>

## 🚀 Visit Website

[배포링크](http://pre-project-023.s3-website.ap-northeast-2.amazonaws.com/1/15)

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
      회원 가입 페이지
    </th>
    <th>
      로그인 페이지
    </th>
    <th>
      로그아웃 페이지
    </th>
  </tr>
  <tr>
    <td>
      <img src="https://user-images.githubusercontent.com/63777183/200190495-9f4b0c33-eb46-4eab-bd3e-ce9f550d7ee0.png"  alt="signup page" width = 100% >
    </td>
    <td>
      <img src="https://user-images.githubusercontent.com/63777183/200190514-970c1207-5f74-4d08-bc07-597662df7797.png" alt="login page" width = 100%>
    </td>
    <td>
      <img src="https://user-images.githubusercontent.com/63777183/200190534-68804e40-2cc2-4409-a7d3-a1b16079d37e.png" alt="logout" width = 100%>
    </td>
   </tr> 
  <tr>
    <th>
      메인 페이지
    </th>
    <th>
      질문 상세 페이지
    </th>
    <th>
      질문 작성 페이지
    </th>
  </tr>
  <tr>
    <td>
      <img src="https://user-images.githubusercontent.com/63777183/200190163-66eb599a-1b58-4768-8a3c-42ecae3d2932.png"  alt="main page" width = 100%>
    </td>
    <td>
      <img src="https://user-images.githubusercontent.com/63777183/200190204-1788c84b-34fa-47b9-b950-18c35114de6a.png" alt="detail page" width = 100%>
    </td>
    <td>
      <img src="https://user-images.githubusercontent.com/63777183/200190180-2b7cbe20-fd62-4306-800e-aac9e42d2cfb.png" alt="write page" width = 100%>
    </td>
   </tr>
   <tr>
    <th>
      질문 수정 페이지
    </th>
    <th>
      답변 수정 페이지
    </th>
    <th>
      마이 페이지
    </th>
  </tr>
  <tr>
    <td>
      <img src="https://user-images.githubusercontent.com/63777183/200190647-08187c5c-cd7e-49f8-b3ff-256833000151.png"  alt="edit question page" width = 100%>
    </td>
    <td>
      <img src="https://user-images.githubusercontent.com/63777183/200190668-8025990f-977a-4085-863e-425a6b816770.png" alt="edit answer page" width = 100%>
    </td>
    <td>
      <img src="https://user-images.githubusercontent.com/63777183/200190687-b03c2de6-27f2-49d0-98e7-8ba5de43f99f.png" alt="my info page" width = 100%>
    </td>
    <tr>
    <th>
      질문 검색 페이지
    </th>
    <th>
      Not Found 페이지
    </th>
    <th>
    </th>
  </tr>
  <tr>
    <td>
      <img src="https://user-images.githubusercontent.com/63777183/200190906-d8875dfe-96bb-4e3e-aab3-132e9ca1958a.png"  alt="search page" width = 100%>
    </td>
    <td>
      <img src="https://user-images.githubusercontent.com/63777183/200190943-bb6f8a8b-3c1a-42ce-82e2-e7e33b66577f.png" alt="404 page" width = 100%>
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

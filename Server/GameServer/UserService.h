#pragma once
#include "User.h"
#include "UserRepository.h"

class UserService
{
private:
	UserService() {} // private 생성자
public:
	// 복사 생성자와 복사 대입 연산자를 삭제하여 단일 인스턴스 보장
	UserService(const UserService&) = delete;
	UserService& operator=(const UserService&) = delete;

	// 전역적으로 접근 가능한 정적 메서드로 인스턴스를 반환
	static UserService& GetInstance()
	{
		static UserService instance; // 정적 지역 변수, 프로그램 종료 시까지 유지
		return instance;
	}

	bool CreateUser(const string& email, const string& pwd, const string& name);
	User LoginUser(const string& email, const string& pwd);

	bool VerifyExistsUser(wstring& wEmail, UserRepository& userRepository);
	bool VerifyLoginUser(const string& email, const string& pwd, UserRepository& userRepository);

private:
	USE_LOCK;
};


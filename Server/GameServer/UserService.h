#pragma once
#include "User.h"
#include "UserRepository.h"

class UserService
{
private:
	UserService() {} // private ������
public:
	// ���� �����ڿ� ���� ���� �����ڸ� �����Ͽ� ���� �ν��Ͻ� ����
	UserService(const UserService&) = delete;
	UserService& operator=(const UserService&) = delete;

	// ���������� ���� ������ ���� �޼���� �ν��Ͻ��� ��ȯ
	static UserService& GetInstance()
	{
		static UserService instance; // ���� ���� ����, ���α׷� ���� �ñ��� ����
		return instance;
	}

	bool CreateUser(const string& email, const string& pwd, const string& name);
	User LoginUser(const string& email, const string& pwd);

	bool VerifyExistsUser(wstring& wEmail, UserRepository& userRepository);
	bool VerifyLoginUser(const string& email, const string& pwd, UserRepository& userRepository);

private:
	USE_LOCK;
};


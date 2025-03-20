#include "pch.h"
#include "UserService.h"

// �ش� ������ �����ϴ��� ����
bool UserService::VerifyExistsUser(wstring& wEmail, UserRepository& userRepository)
{
	User user = userRepository.FindByEmail(wEmail);
	if (user.GetEmail() == ConvertChar::WideToUtf8(wEmail))
		return true;
	return false;
}

// DB�� ����� �̸���, ��й�ȣ�� ��ġ�ϴ��� �� ����, ���� ��ȣȭ �ʿ�
bool UserService::VerifyLoginUser(const string& email, const string& pwd, UserRepository& userRepository)
{
	User user = userRepository.FindByEmail(ConvertChar::ServerStringToWstring(email));
	if (user.GetEmail() == email && user.GetPassword() == pwd)
		return true;
	return false;
}

// ���� ����
bool UserService::CreateUser(const string& email, const string& pwd, const string& name)
{
	UserRepository& userRepository = UserRepository::GetInstance();
	wstring wEmail = ConvertChar::ServerStringToWstring(email);

	WRITE_LOCK;
	// ����ڰ� �����ϴ��� DB���� �����ϴ� �κ�
	if (VerifyExistsUser(wEmail, userRepository))
	{
		// true�� �̹� �����ϴ� ȸ����
		return false;
	}
	else
	{
		// ����ڰ� �����Ƿ� DB�� �߰���
		userRepository.CreateUser(email, pwd, name);
	}

	return true;
}

// ���� �α���
User UserService::LoginUser(const string& email, const string& pwd)
{
	UserRepository& userRepository = UserRepository::GetInstance();
	WRITE_LOCK;
	if (VerifyLoginUser(email, pwd, userRepository))
	{
		User user = userRepository.FindByEmail(ConvertChar::ServerStringToWstring(email));
		return user;
	}
	return User();
}

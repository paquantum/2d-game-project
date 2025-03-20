#include "pch.h"
#include "UserService.h"

// 해당 유저가 존재하는지 검증
bool UserService::VerifyExistsUser(wstring& wEmail, UserRepository& userRepository)
{
	User user = userRepository.FindByEmail(wEmail);
	if (user.GetEmail() == ConvertChar::WideToUtf8(wEmail))
		return true;
	return false;
}

// DB에 저장된 이메일, 비밀번호와 일치하는지 비교 검증, 추후 암호화 필요
bool UserService::VerifyLoginUser(const string& email, const string& pwd, UserRepository& userRepository)
{
	User user = userRepository.FindByEmail(ConvertChar::ServerStringToWstring(email));
	if (user.GetEmail() == email && user.GetPassword() == pwd)
		return true;
	return false;
}

// 유저 생성
bool UserService::CreateUser(const string& email, const string& pwd, const string& name)
{
	UserRepository& userRepository = UserRepository::GetInstance();
	wstring wEmail = ConvertChar::ServerStringToWstring(email);

	WRITE_LOCK;
	// 사용자가 존재하는지 DB에서 검증하는 부분
	if (VerifyExistsUser(wEmail, userRepository))
	{
		// true면 이미 존재하는 회원임
		return false;
	}
	else
	{
		// 사용자가 없으므로 DB에 추가함
		userRepository.CreateUser(email, pwd, name);
	}

	return true;
}

// 유저 로그인
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

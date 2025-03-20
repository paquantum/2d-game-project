#pragma once
#include "User.h";
#include "DBConnectionPool.h"
#include "DBBind.h"
#include "ConvertChar.h"

class UserRepository
{
private:
	UserRepository() {}
public:
	UserRepository(const UserRepository&) = delete;
	UserRepository& operator=(const UserRepository) = delete;

	static UserRepository& GetInstance()
	{
		static UserRepository userRepository;
		return userRepository;
	}

	bool CreateUser(const string& email, const string& pwd, const string& name);
	User FindByEmail(wstring wEmail);
	User FindById(int64 userId);
	bool UpdateUser(int64 userId);
	bool DeleteUser(int64 userId);
};


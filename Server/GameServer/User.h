#pragma once

class User
{
public:
	User() {}
	User(const string& email, const string& password, const string& name, const string& phone) :
		email(email), password(password), name(name), phone(phone)
	{
		//user_id = 0;
	}

	void SetUserId(int64 user_id)
	{
		this->userId = user_id;
	}
	int64 GetId()
	{
		return this->userId;
	}
	void SetUserName(string name)
	{
		this->name = name;
	}
	string GetName()
	{
		return this->name;
	}
	void SetUserEmail(string email)
	{
		this->email = email;
	}
	string GetEmail()
	{
		return this->email;
	}
	void SetUserPassword(string password)
	{
		this->password = password;
	}
	string GetPassword()
	{
		return this->password;
	}
	void SetUserePhone()
	{
		this->phone = phone;
	}

private:
	int64	userId = 0;
	string	name;
	string	email;
	string	password;
	string	phone;
};

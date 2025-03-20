#include "pch.h"
#include "UserRepository.h"

// 유저 생성
bool UserRepository::CreateUser(const string& email, const string& pwd, const string& name)
{
	std::wstring wEmail = ConvertChar::ServerStringToWstring(email);
	std::wstring wPwd = ConvertChar::ServerStringToWstring(pwd);
	std::wstring wName = ConvertChar::ServerStringToWstring(name);
	//TIMESTAMP_STRUCT date = { 2025, 1, 18 };
	
	DBConnection* dbConn = GDBConnectionPool->Pop();
	if (dbConn == nullptr) std::cerr << "CreateUser, dbConn is nullptr" << endl;

	DBBind<3, 0> dbBind(*dbConn,
		L"INSERT INTO gameserver.user (email, password, name) VALUES(?, ?, ?)");

	dbBind.BindParam(0, wEmail.c_str());
	dbBind.BindParam(1, wPwd.c_str());
	dbBind.BindParam(2, wName.c_str());
	//dbBind.BindParam(3, date);
	ASSERT_CRASH(dbBind.Execute());

	GDBConnectionPool->Push(dbConn);

	return true;
}

// 이메일로 유저 조회
User UserRepository::FindByEmail(wstring wEmail)
{
	DBConnection* dbConn = GDBConnectionPool->Pop();
	if (dbConn == nullptr) std::cerr << "FindByEmail, dbConn is nullptr" << endl;

	DBBind<1, 4> dbBind(*dbConn,
		L"SELECT user_id, email, password, name FROM gameserver.user WHERE email = (?)");

	WCHAR email[100] = { 0 };
	memcpy(email, wEmail.c_str(), sizeof(email));
	dbBind.BindParam(0, email);

	int64 outUserId;
	WCHAR outEmail[100];
	WCHAR outPwd[100];
	WCHAR outName[100];
	dbBind.BindCol(0, outUserId);
	dbBind.BindCol(1, outEmail);
	dbBind.BindCol(2, outPwd);
	dbBind.BindCol(3, outName);
	ASSERT_CRASH(dbBind.Execute());

	dbConn->Fetch();
	User user;
	cout << "로그인 유저 ID: " << user.GetId() << endl;
	if (wcscmp(outEmail, wEmail.c_str()) == 0)
	{
		user.SetUserId(outUserId);
		user.SetUserName(ConvertChar::WideToUtf8(outName));
		user.SetUserEmail(ConvertChar::WideToUtf8(outEmail));
		user.SetUserPassword(ConvertChar::WideToUtf8(outPwd));
		GDBConnectionPool->Push(dbConn);
		return user;
	}

	GDBConnectionPool->Push(dbConn);

	return User();
}

// userId로 유저가 있는지 조회
User UserRepository::FindById(int64 userId)
{
	DBConnection* dbConn = GDBConnectionPool->Pop();
	if (dbConn == nullptr) std::cerr << "FindById, dbConn is nullptr" << endl;

	DBBind<1, 4> dbBind(*dbConn,
		L"SELECT user_id, email, password, name FROM gameserver.user WHERE user_id = (?)");

	int64 id = userId;
	dbBind.BindParam(0, id);

	int64 outUserId;
	WCHAR outEmail[100];
	WCHAR outPwd[100];
	WCHAR outName[100];
	dbBind.BindCol(0, outUserId);
	dbBind.BindCol(1, outEmail);
	dbBind.BindCol(2, outPwd);
	dbBind.BindCol(3, outName);
	ASSERT_CRASH(dbBind.Execute());

	dbConn->Fetch();
	bool result = false;
	User user;
	cout << "로그인 유저 ID: " << user.GetId() << endl;
	if (userId == outUserId)
	{
		user.SetUserId(outUserId);
		user.SetUserName(ConvertChar::WideToUtf8(outName));
		user.SetUserEmail(ConvertChar::WideToUtf8(outEmail));
		user.SetUserPassword(ConvertChar::WideToUtf8(outPwd));
		result = true;
	}

	GDBConnectionPool->Push(dbConn);

	return user;
}

bool UserRepository::UpdateUser(int64 userId)
{
    return false;
}

bool UserRepository::DeleteUser(int64 userId)
{
    return false;
}

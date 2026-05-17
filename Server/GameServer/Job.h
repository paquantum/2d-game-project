#pragma once

/*
* 튜플의 개념 및 사용방법
* auto tup = std::tuple<int32, int32>(1, 2);
* auto val0 = std::get<0>(tup); // 1
* auto val1 = std::get<1>(tup); // 2
*/

/*
* seq, gen_seq 개념
* auto s = gen_seq<3>(); // seq<0, 1, 2> 타입이 만들어짐
* gen_seq를 보면 gen_seq<...> 를 상속 받고 있는데
* gen_seq<3> -> gen_seq<2, 2> 를 받고 있고 Remains는 없는 상태
* gen_seq<2, 2> -> gen_seq<1, 1, 2> 를 받고 있고 Remains는 2
* gen_seq<1, 1, 2> -> gen_seq<0, 0, 1, 2> 를 받고 있고 Remains는 1, 2
* 이후에는 gen_seq<0, Remains...> 가 호출되면서 seq<0, 1, 2> 를 상속받은 형태
* 최종적으로 gen_seq<3>은 seq<0, 1, 2>를 상속받고 있다는 것
* gen_seq<3>은 길이 3짜리 seq를 만들어 주세요~ 라는 것
*/

// C++11 apply, 직접 만들어보기
template<int... Remains>
struct seq
{
};

template<int N, int... Remains>
struct gen_seq : gen_seq<N - 1, N - 1, Remains...>
{
};

template<int... Remains>
struct gen_seq<0, Remains...> : seq<Remains...>
{
};

template<typename Ret, typename... Args>
void xapply(Ret(*func)(Args...), std::tuple<Args...>& tup)
{
	xapply_helper(func, gen_seq<sizeof...(Args)>(), tup); // 인자 갯수에 따라 시퀀스 생성
}

template<typename F, typename... Args, int... ls>
void xapply_helper(F func, seq<ls...>, std::tuple<Args...>& tup)
{
	// func 라는 함수를 호출할 건데 인자를 (std::get<ls>(tup)...) 준 것
	// 즉, 여기서는 std::get<0>(tup), std::get<1>(tup), ... 이런식으로 인자 갯수에 따라 get이 호출되도록 하는 것
	(func)(std::get<ls>(tup)...); // 인자 갯수에 상관없이 처리 가능
}

// 멤버 함수 호출용으로 하나 더 추가
template<typename T, typename Ret, typename... Args>
void xapply(T* obj, Ret(T::*func)(Args...), std::tuple<Args...>& tup)
{
	xapply_helper(obj, func, gen_seq<sizeof...(Args)>(), tup); // 인자 갯수에 따라 시퀀스 생성
}

template<typename T, typename F, typename... Args, int... ls>
void xapply_helper(T* obj, F func, seq<ls...>, std::tuple<Args...>& tup)
{
	(obj->*func)(std::get<ls>(tup)...); // 인자 갯수에 상관없이 처리 가능
}

// 지난 시간에 한 걸 보면, 어떤 행위를 인자와 묶어서 넘겨주는 행위였고
// 이게 함수자의 역할과 유사하다, 어떤 클래스가 사실상 함수의 역할을 한다고 볼 수 있다
// void HealValue(int64 target, int32 value) { cout << .... ; }
// HealValue를 잡으로 만드는게 오늘의 목표임!
// 함수와 인자를 들고 있어야 함
// 템플릿 마법을 사용하려고 할 때, C++11 이전?에서는 인자 갯수에 따라 맞춰야 해서 골머리가 아팠지만
// 모던 C++에서는 가변인자 템플릿을 사용해서, 인자 갯수에 상관없이 만들 수 있다
// 또한 Job을 만들어서 사용하는 곳에서 알아서 HealValue를 호출할 수 있도록 만들고 싶은 것
// 이를 위해 함수 포인터를 이용하는 것
// 함수자 (Functor)
class IJob // 2세대 IJob으로 변경
{
public:
	virtual void Execute() {}
};

template<typename Ret, typename... Args>
class FuncJob : public IJob
{
	// Ret를 반환하고 인자를 여러개 받는다고 함수 타입 정의
	using FuncType = Ret(*)(Args...); // 함수 포인터 타입 정의
public:
	FuncJob(FuncType func, Args... args) : _func(func), _tuple(args...) // 생성자에서 함수 포인터를 받아서 저장
	{
	}

	// Ret operator()(Args... args) // 인자 여러개 받을 수 있도록
	//Ret operator()() // 내부 인자 사용으로 비움
	//{
	//	// TODO : 함수자 호출 시의 행동
	//	
	//	// _func(_tuple) 처럼 사용은 안됨, 어떻게 써야 하냐?
	//	// C++ 17 사용 방식으로 std::apply을 이용해서 튜플에 저장된 인자들을 함수 포인터에 전달하는 방식
	//	std::apply(_func, _tuple); // 튜플에 저장된 인자들을 함수 포인터에 전달하여 호출
	//	//_func(args...); // 실제 함수 포인터를 호출하면서 인자 전달
	//}

	virtual void Execute() override
	{
		//_func(args...);
		// std::apply(_func, _tuple); // 튜플에 저장된 인자들을 함수 포인터에 전달하여 호출
		// 그렇다면 apply 사용 가능한 C++ 17 이전에는 어떻게 처리 했나??
		// 템플릿 흑마법을 사용해서 처리 했던것을 한 번 체험해봄
		xapply(_func, _tuple); // 템플릿 마법을 이용해서 구현한 apply 함수);
	}

private:
	FuncType _func; // 실제 함수 포인터를 저장하는 멤버 변수
	// 함수를 만들어 놓고 인자를 사용해야 하는데, 어떻게 저장해 놓을까?
	// Args... _args 같은건 없을까?
	// 우회해서 C++11 에서 사용하는 방식
	std::tuple<Args...> _tuple; // 인자들을 튜플로 저장, 가변인자 템플릿과 함께 사용
};

/*
* 기존 FuncJob에서 using FuncType = Ret(*)(Args...); 부분은 모든 함수를 다 받을 수 있는게 아님
* 전역 함수, 클래스 안에 static이 선언된 함수는 상관 없지만, 
* 클래스 안에 선언된 일반 멤버 함수는 Ret(T::*)(Args...) 형태의 함수 포인터로 받아야 함
* T 클래스 안에 포함된 * 멤버 함수이다를 가리킴
*/
template<typename T, typename Ret, typename... Args>
class MemberJob : public IJob
{
	using FuncType = Ret(T::*)(Args...);
public:
	MemberJob(T* obj, FuncType func, Args... args) : _obj(obj), _func(func), _tuple(args...)
	{}

	// return 값이 필요하면 사용해도 되지만 보통 대부분 응답받기보다 실행하는게 목적이라 void
	virtual void Execute() override
	{
		xapply(_obj, _func, _tuple);
	}

private:
	T*					_obj;
	FuncType			_func;
	std::tuple<Args...> _tuple;
};
/*
* class Knight 
* {
* public:
* 	void HealMe(int32 healValue) { cout << "힐 " << healValue << "만큼!"; }
* }
* MemberJob 사용 예시
* Knight k1;
* MemberJob job2(&k1, &Knight::HealMe, 100); // k1 객체의 HealMe 멤버 함수를 호출하는 잡 생성
* job2.Excute(); // k1 객체의 HealMe 멤버 함수가 100이라는 인자로 호출됨
*/

//////////////

//class HealJob : public IJob
//{
//public:
//	virtual void Execute() override
//	{
//		cout << _target << "한테 힐 " << _healValue << "만큼!";
//	}
//
//public:
//	uint64 _target = 0;
//	uint32 _healValue = 0;
//};

//using JobRef = shared_ptr<IJob>;
// 순차적으로 처리하게끔 유도하도록
class JobQueue
{
public:
	void Push(JobRef job) // 잡을 넣을 때
	{
		WRITE_LOCK;
		_jobs.push(job);		
	}

	JobRef Pop() // 잡을 꺼내서 사용할 때
	{
		WRITE_LOCK;
		if (_jobs.empty())
			return nullptr;

		JobRef ret = _jobs.front();
		_jobs.pop();
		return ret;
	}

private:
	USE_LOCK; // 일단 락을 사용할 거여서
	queue<JobRef> _jobs;
};
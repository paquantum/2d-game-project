#pragma once

class IJob
{
public:
	virtual void Execute() { }
};

class HealJob : public IJob
{
public:
	virtual void Execute() override
	{
		cout << _target << "한테 힐 " << _healValue << "만큼!";
	}

public:
	uint64 _target = 0;
	uint32 _healValue = 0;
};

using JobRef = shared_ptr<IJob>;
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
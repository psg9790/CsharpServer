using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{

    class Program
    {
        // 상호배제

        static object _lock = new object();         // 1) Monitor 기반
        static SpinLock _lock2 = new SpinLock();    // 2) 구현되어 있는 스핀락 같은 경우, 성공할 때까지 시도하다가 실패하는 시간이 길어지면 양보하는 방식으로 바꿈
        //static Mutex _lock3 = new Mutex();          // 3) 잘 안씀


        // 안바뀔 확률이 99.999999...%
        // RWLock Reader Writer Lock
        static ReaderWriterLockSlim _lock3 = new ReaderWriterLockSlim();    // 4) 쓰기 비율이 매우 낮을 때
        class Reward
        {

        }

        static Reward GetRewardById(int id)
        {
            _lock3.EnterReadLock();

            _lock3.ExitReadLock();
            return null;
        }
        static void AddReward(Reward reward)
        {
            _lock3.EnterWriteLock();

            _lock3.ExitWriteLock();
        }

        static void Main(string[] args)
        {
            // 1)
            lock(_lock)
            {

            }

            // 2)
            bool lockTaken = false;
            try
            {
                _lock2.Enter(ref lockTaken);
            }
            finally
            {
                if (lockTaken)
                    _lock2.Exit();
            }
        }
    }
}

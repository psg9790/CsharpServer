using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{

    class Program
    {
        static int number = 0;
        static object _obj = new object();

        static void Thread_1()
        {
            for (int i = 0; i < 1000000; i++)
            {
                // 상호배제 Mutual Exclusive

                lock(_obj)
                {
                    number++;
                }
                /*Monitor.Enter(_obj);    // 문을 잠그는 행위

                number++;   // 코드가 길면 관리하기 어려워짐
                // return;  // !!!!!!!!!!!!!!
                // 데드락 DeadLock
                // 예상치 못한 상황에서도 문제이기 때문에 대응하기 힘듬 (ex. divide by zero)

                Monitor.Exit(_obj);     // 잠금을 풀어준다*/

            }
        }
        static void Thread_2()
        {
            for (int i = 0; i < 1000000; i++)
            {
                lock(_obj)
                {
                    number--;
                }
                /*Monitor.Enter(_obj);

                number--;

                Monitor.Exit(_obj);*/
            }
        }

        static void Main(string[] args)
        {

            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }
    }
}

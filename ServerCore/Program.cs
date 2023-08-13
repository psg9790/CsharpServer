using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{

    class Program
    {
        static int number = 0;

        static void Thread_1()
        {
            // atomic : 원자성
            
            // 골드 -= 100
            // 인벤 += 검
            // 둘 중 하나라도 누락되면 안됨

            for (int i = 0; i < 1000000; i++)
            {
                int afterValue =  Interlocked.Increment(ref number);  // 원자적으로 연산함으로써 문제를 해결하지만, 성능의 손해가 큼
                // 연산 후의 값을 반환해주어 정확한 값임이 보장됨
                /*number++;*/
            }
        }
        static void Thread_2()
        {
            for (int i = 0; i < 1000000; i++)
            {
                Interlocked.Decrement(ref number);
                /*number--;*/
            }
        }

        static void Main(string[] args)
        {
            // sudo code
            /*number++;*/
            /*int temp = number;
            temp += 1;
            number = temp;*/

            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }
    }
}

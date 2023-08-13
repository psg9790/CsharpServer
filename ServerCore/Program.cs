using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    // 메모리 배리어
    // A) 코드 재배치 억제
    // B) 가시성

    // 1) Full MemoryBarrier : Store/Load 둘 다 막는다
    // 2) Store MemoryBarrier : Store만 막는다
    // 3) Load MemoryBarrier : Load만 막는다

    class Program
    {
        // 하드웨어 최적화 실습
        static int x = 0;
        static int y = 0;
        static int r1 = 0;
        static int r2 = 0;

        static void Thread_1()
        {
            y = 1;  // Store y

            // --------------------- 순서를 바꿀 수 없게됨
            Thread.MemoryBarrier(); // 실제 메모리를 갱신하는 개념

            r1 = x; // Load x
        }

        static void Thread_2()
        {
            x = 1;  // Store x

            // --------------------- 순서를 바꿀 수 없게됨
            Thread.MemoryBarrier();

            r2 = y;   // Load y
        }

        static void Main(string[] args)
        {
            int count = 0;
            while (true)
            {
                count++;
                x = y = r1 = r2 = 0;

                Task t1 = new Task(Thread_1);
                Task t2 = new Task(Thread_2);
                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);

                if (r1 == 0 && r2 == 0) // 원래 나오면 안됨
                    break;
            }
            Console.WriteLine($"{count}번만에 빠져나옴!");
        }
    }
}

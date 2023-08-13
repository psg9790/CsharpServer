using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {

        static void Main(string[] args)
        {
            int[,] arr = new int[10000, 10000];

            {
                long now = DateTime.Now.Ticks;
                for(int i = 0; i < 10000; i++)
                {
                    for(int j = 0; j <10000; j++)
                    {
                        arr[i,j] = 1;
                    }
                }
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(i,j) 순서 걸린 시간 {end - now}");
            }
            {
                long now = DateTime.Now.Ticks;
                for (int i = 0; i < 10000; i++)
                {
                    for (int j = 0; j < 10000; j++)
                    {
                        arr[j, i] = 1;
                    }
                }
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(j,i) 순서 걸린 시간 {end - now}");
            }

            // Spacial Locality (공간적 캐시 접근)으로 인해 첫번째 반복문이 두번째 반복문보다 훨씬 빠르다.
        }
    }
}

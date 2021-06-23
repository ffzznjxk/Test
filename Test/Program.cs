using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static int tickeds = 100;
        static void Main(string[] args)
        {
            //UseBackThread();
            //UseParmThread();
            //UserThreadPool();
            //UseCancellationToken();
            Thread thread1 = new Thread(SaleTicketThread1);
            Thread thread2 = new Thread(SaleTicketThread2);
            thread1.Start();
            thread2.Start();
            Thread.Sleep(4000);
            Console.ReadKey();
        }

        private static void SaleTicketThread1()
        {
            while (tickeds > 0)
            {
                Console.WriteLine($"线程1出票：{tickeds--}");
            }
        }

        private static void SaleTicketThread2()
        {
            while (tickeds > 0)
            {
                Console.WriteLine($"线程2出票：{tickeds--}");
            }
        }

        private static void UseCancellationToken()
        {
            Console.WriteLine("主线程运行");
            CancellationTokenSource cts = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(callBack, cts.Token);
            Console.WriteLine("按下回车键来取消操作");
            Console.Read();
            cts.Cancel();
            Console.ReadKey();
        }

        private static void callBack(object state)
        {
            CancellationToken token = (CancellationToken)state;
            Console.WriteLine("开始计数");
            Count(token, 1000);
        }

        private static void Count(CancellationToken token, int v)
        {
            for (int i = 0; i < v; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("计数取消");
                    return;
                }
                else
                {
                    Console.WriteLine($"计数为：{i}");
                    Thread.Sleep(300);
                }
            }
            Console.WriteLine("计数完成");
        }

        private static void UserThreadPool()
        {
            Console.WriteLine($"主线程Id = {Thread.CurrentThread.ManagedThreadId}");
            ThreadPool.QueueUserWorkItem(CallBackWorkItem);
            ThreadPool.QueueUserWorkItem(CallBackWorkItem, "work");
            Thread.Sleep(3000);
            Console.WriteLine("从主线程退出");
            Console.ReadKey();
        }

        private static void CallBackWorkItem(object state)
        {
            Console.WriteLine("线程池开始执行");
            if (state != null)
            {
                Console.WriteLine($"线程池线程ID = {Thread.CurrentThread.ManagedThreadId} " +
                    $"传入的参数为 {state}");
            }
            else
            {
                Console.WriteLine($"线程池线程ID = {Thread.CurrentThread.ManagedThreadId}");
            }
        }

        private static void UseParmThread()
        {
            Thread parmThread = new Thread(new ParameterizedThreadStart(Worker));
            parmThread.Name = "线程1";
            parmThread.Start("123");
            Console.WriteLine("从主线程返回");
            Console.ReadKey();
        }

        private static void Worker(object obj)
        {
            Thread.Sleep(1000);
            Console.WriteLine($"传入的参数为{obj}");
            Console.WriteLine($"{Thread.CurrentThread.Name}返回");
        }

        private static void UseBackThread()
        {
            Thread backThread = new Thread(Warker);
            backThread.IsBackground = true;
            backThread.Start();
            backThread.Join();
            Console.WriteLine("从主线程退出");
            Console.ReadKey();
        }

        private static void Warker()
        {
            Thread.Sleep(1000);
            Console.WriteLine("从后台线程退出");
        }
    }
}

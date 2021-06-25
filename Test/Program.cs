using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    class Program
    {
        static int tickeds = 100;
        static object gloalObj = new object();
        //private static List<Product> ProductList = new List<Product>();
        static void Main(string[] args)
        {
            //UseBackThread();
            //UseParmThread();
            //UserThreadPool();
            //UseCancellationToken();
            //UseTwoThreads();
            //UseLockTime();

            //ParalleTest();
            //ParalleForTest();
            //ParalleForeachTest();
            //ParalleStopBreakTest();

            //ParallelInvokeMethod();
            //ParallelForMethod();
            //ParallelForMethod1();
            //ParallelInvokeException();
            //OrderByTest();
            //TaskTest1();
            //TaskTest2();
            //TaskTest3();
            //TaskTest4();
            //TaskTest5();
            //TaskTest6();
            SpinLock();
            Console.ReadLine();
        }

        private static void SpinLock()
        {
            SpinLock slock = new SpinLock(false);
            long sum1 = 0;
            long sum2 = 0;
            Parallel.For(0, 100000, i =>
            {
                sum1 += i;
            });

            Parallel.For(0, 100000, i =>
            {
                bool lockTaken = false;
                try
                {
                    slock.Enter(ref lockTaken);
                    sum2 += i;
                }
                finally
                {
                    if (lockTaken)
                        slock.Exit(false);
                }
            });

            Console.WriteLine("Num1的值为:{0}", sum1);
            Console.WriteLine("Num2的值为:{0}", sum2);
        }

        private static void TaskTest6()
        {
            Task[] tasks = new Task[2];
            tasks[0] = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task 1 Start running...");
                while (true)
                {
                    System.Threading.Thread.Sleep(1000);
                }
                Console.WriteLine("Task 1 Finished!");
            });
            tasks[1] = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task 2 Start running...");
                System.Threading.Thread.Sleep(2000);
                Console.WriteLine("Task 2 Finished!");
            });

            Task.WaitAll(tasks, 5000);
            for (int i = 0; i < tasks.Length; i++)
            {
                if (tasks[i].Status != TaskStatus.RanToCompletion)
                {
                    Console.WriteLine("Task {0} Error!", i + 1);
                }
            }
        }

        private static void TaskTest5()
        {
            Task.Factory.StartNew(() =>
            {
                var t1 = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Task 1 running...");
                    return 1;
                });
                t1.Wait(); //等待任务一完成
                var t3 = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Task 3 running...");
                    return t1.Result + 3;
                });
                var t4 = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Task 2 running...");
                    return t1.Result + 2;
                }).ContinueWith(task =>
                {
                    Console.WriteLine("Task 4 running...");
                    return task.Result + 4;
                });
                Task.WaitAll(t3, t4);  //等待任务三和任务四完成
                var result = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Task Finished! The result is {0}", t3.Result + t4.Result);
                });
            });
        }

        private static void TaskTest4()
        {
            var pTask = Task.Factory.StartNew(() =>
            {
                var cTask = Task.Factory.StartNew(() =>
                {
                    System.Threading.Thread.Sleep(2000);
                    Console.WriteLine("Childen task finished!");
                }, TaskCreationOptions.AttachedToParent);
                Console.WriteLine("Parent task finished!");
            });
            pTask.Wait();
            Console.WriteLine("Flag");
        }

        private static void TaskTest3()
        {
            var pTask = Task.Factory.StartNew(() =>
            {
                var cTask = Task.Factory.StartNew(() =>
                {
                    System.Threading.Thread.Sleep(2000);
                    Console.WriteLine("Childen task finished!");
                });
                Console.WriteLine("Parent task finished!");
            });
            pTask.Wait();
            Console.WriteLine("Flag");
        }

        private static void TaskTest2()
        {
            var task1 = new Task(() =>
            {
                Console.WriteLine("Task 1 Begin");
                System.Threading.Thread.Sleep(2000);
                Console.WriteLine("Task 1 Finish");
            });
            var task2 = new Task(() =>
            {
                Console.WriteLine("Task 2 Begin");
                System.Threading.Thread.Sleep(3000);
                Console.WriteLine("Task 2 Finish");
            });

            task1.Start();
            task2.Start();
            Task.WaitAll(task1, task2);
            Console.WriteLine("All task finished!");
        }

        private static void TaskTest1()
        {
            var task1 = new Task(() =>
            {
                Console.WriteLine("Begin");
                System.Threading.Thread.Sleep(2000);
                Console.WriteLine("Finish");
            });
            Console.WriteLine("Before start:" + task1.Status);
            task1.Start();
            Console.WriteLine("After start:" + task1.Status);
            task1.Wait();
            Console.WriteLine("After Finish:" + task1.Status);
        }

        public static void OrderByTest()
        {
            Stopwatch stopWatch = new Stopwatch();
            List<Custom> customs = new List<Custom>();
            for (int i = 0; i < 2000000; i++)
            {
                customs.Add(new Custom() { Name = "Jack", Age = 21, Address = "NewYork" });
                customs.Add(new Custom() { Name = "Jime", Age = 26, Address = "China" });
                customs.Add(new Custom() { Name = "Tina", Age = 29, Address = "ShangHai" });
                customs.Add(new Custom() { Name = "Luo", Age = 30, Address = "Beijing" });
                customs.Add(new Custom() { Name = "Wang", Age = 60, Address = "Guangdong" });
                customs.Add(new Custom() { Name = "Feng", Age = 25, Address = "YunNan" });
            }

            stopWatch.Restart();
            var groupByAge = customs.GroupBy(item => item.Age).ToList();
            foreach (var item in groupByAge)
            {
                Console.WriteLine("Age={0},count = {1}", item.Key, item.Count());
            }
            stopWatch.Stop();

            Console.WriteLine("Linq group by time is: " + stopWatch.ElapsedMilliseconds);


            stopWatch.Restart();
            var lookupList = customs.ToLookup(i => i.Age);
            foreach (var item in lookupList)
            {
                Console.WriteLine("LookUP:Age={0},count = {1}", item.Key, item.Count());
            }
            stopWatch.Stop();
            Console.WriteLine("LookUp group by time is: " + stopWatch.ElapsedMilliseconds);
        }
        public static void TestPLinq()
        {
            Stopwatch sw = new Stopwatch();
            List<Custom> customs = new List<Custom>();
            for (int i = 0; i < 10000000; i++)
            {
                customs.Add(new Custom() { Name = "Jack", Age = 21, Address = "NewYork" });
                customs.Add(new Custom() { Name = "Jime", Age = 26, Address = "China" });
                customs.Add(new Custom() { Name = "Tina", Age = 29, Address = "ShangHai" });
                customs.Add(new Custom() { Name = "Luo", Age = 30, Address = "Beijing" });
                customs.Add(new Custom() { Name = "Wang", Age = 60, Address = "Guangdong" });
                customs.Add(new Custom() { Name = "Feng", Age = 25, Address = "YunNan" });
            }

            sw.Start();
            var result = customs.Where(c => c.Age > 26).ToList();
            sw.Stop();
            Console.WriteLine("Linq time is {0}.", sw.ElapsedMilliseconds);

            sw.Restart();
            sw.Start();
            var result2 = customs.AsParallel().Where(c => c.Age > 26).ToList();
            sw.Stop();
            Console.WriteLine("Parallel Linq time is {0}.", sw.ElapsedMilliseconds);
        }

        public static void ParallelForMethod1()
        {
            var obj = new Object();
            long num = 0;
            ConcurrentBag<long> bag = new ConcurrentBag<long>();

            stopWatch.Start();
            for (int i = 0; i < 10000; i++)
            {
                for (int j = 0; j < 60000; j++)
                {
                    //int sum = 0;
                    //sum += item;
                    num++;
                }
            }
            stopWatch.Stop();
            Console.WriteLine("NormalFor run " + stopWatch.ElapsedMilliseconds + " ms.");

            stopWatch.Reset();
            stopWatch.Start();
            Parallel.For(0, 10000, item =>
            {
                for (int j = 0; j < 60000; j++)
                {
                    //int sum = 0;
                    //sum += item;
                    lock (obj)
                    {
                        num++;
                    }
                }
            });
            stopWatch.Stop();
            Console.WriteLine("ParallelFor run " + stopWatch.ElapsedMilliseconds + " ms.");
            Console.ReadLine();
        }

        public static void ParallelInvokeException()
        {
            stopWatch.Start();
            try
            {
                Parallel.Invoke(Run1, Run2);
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.InnerExceptions)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            stopWatch.Stop();
            Console.WriteLine("Parallel run " + stopWatch.ElapsedMilliseconds + " ms.");

            stopWatch.Reset();
            stopWatch.Start();
            try
            {
                Run1();
                Run2();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            stopWatch.Stop();
            Console.WriteLine("Normal run " + stopWatch.ElapsedMilliseconds + " ms.");
        }

        public static void ParallelForMethod()
        {
            stopWatch.Start();
            for (int i = 0; i < 10000; i++)
            {
                for (int j = 0; j < 60000; j++)
                {
                    int sum = 0;
                    sum += i;
                }
            }
            stopWatch.Stop();
            Console.WriteLine("NormalFor run " + stopWatch.ElapsedMilliseconds + " ms.");

            stopWatch.Reset();
            stopWatch.Start();
            Parallel.For(0, 10000, item =>
            {
                for (int j = 0; j < 60000; j++)
                {
                    int sum = 0;
                    sum += item;
                }
            });
            stopWatch.Stop();
            Console.WriteLine("ParallelFor run " + stopWatch.ElapsedMilliseconds + " ms.");
            Console.ReadLine();
        }

        private static Stopwatch stopWatch = new Stopwatch();

        public static void Run1()
        {
            Thread.Sleep(2000);
            Console.WriteLine("Task 1 is cost 2 sec");
            throw new Exception("Exception in task 1");
        }
        public static void Run2()
        {
            Thread.Sleep(3000);
            Console.WriteLine("Task 2 is cost 3 sec");
            throw new Exception("Exception in task 2");
        }

        public static void ParallelInvokeMethod()
        {
            stopWatch.Start();
            Parallel.Invoke(Run1, Run2);
            stopWatch.Stop();
            Console.WriteLine("Parallel run " + stopWatch.ElapsedMilliseconds + " ms.");

            stopWatch.Restart();
            Run1();
            Run2();
            stopWatch.Stop();
            Console.WriteLine("Normal run " + stopWatch.ElapsedMilliseconds + " ms.");
            Console.ReadLine();
        }

        private static void ParalleStopBreakTest()
        {
            List<Product> productList = GetProcuctList_500();
            Thread.Sleep(3000);
            Parallel.For(0, productList.Count, (i, loopState) =>
            {
                if (i < 100)
                {
                    Console.WriteLine("采用Stop index:{0}", i);
                }
                else
                {
                    /* 满足条件后 尽快停止执行，无法保证小于100的索引数据全部输出*/
                    loopState.Stop();
                    return;
                }
            });

            Thread.Sleep(3000);
            Parallel.For(0, productList.Count, (i, loopState) =>
            {
                if (i < 100)
                {
                    Console.WriteLine("采用Break index:{0}", i);
                }
                else
                {
                    /* 满足条件后 尽快停止执行，保证小于100的索引数据全部输出*/
                    loopState.Break();
                    return;
                }
            });

            Thread.Sleep(3000);
            Parallel.ForEach(productList, (model, loopState) =>
            {
                if (model.SellPrice < 10)
                {
                    Console.WriteLine("采用Stop index:{0}", model.SellPrice);
                }
                else
                {
                    /* 满足条件后 尽快停止执行，无法保证满足条件的数据全部输出*/
                    loopState.Stop();
                    return;
                }
            });
            Thread.Sleep(3000);
            Parallel.ForEach(productList, (model, loopState) =>
            {
                if (model.SellPrice < 10)
                {
                    Console.WriteLine("采用Break index:{0}", model.SellPrice);
                }
                else
                {
                    /* 满足条件后 尽快停止执行，保证满足条件的数据全部输出*/
                    loopState.Break();
                    return;
                }
            });

            Console.ReadLine();
        }

        private static List<Product> GetProcuctList_500()
        {
            List<Product> ProductList = new List<Product>();
            for (int index = 1; index < 500; index++)
            {
                ProductList.Add(GetProduct(index));
            }
            return ProductList;
        }

        private static void ParalleForeachTest()
        {
            List<Product> ProductList = GetProcuctList();
            Parallel.ForEach(ProductList, (model) =>
            {
                Console.WriteLine(model.Name);
            });
            Console.ReadLine();
        }

        private static List<Product> GetProcuctList()
        {
            List<Product> ProductList = new List<Product>();
            for (int index = 1; index < 100; index++)
            {
                ProductList.Add(GetProduct(index));
            }
            return ProductList;
        }

        private static void ParalleForTest()
        {
            Thread.Sleep(3000);
            ForSetProcuct_100();

            Thread.Sleep(3000);
            ParallelForSetProcuct_100();
            Console.ReadLine();
        }

        private static void ForSetProcuct_100()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<Product> ProductList = new List<Product>();
            for (int index = 1; index < 100; index++)
            {
                ProductList.Add(GetProduct(index));
                Console.WriteLine("for SetProcuct index: {0}", index);
            }
            sw.Stop();
            Console.WriteLine("for SetProcuct 10 执行完成 耗时：{0}", sw.ElapsedMilliseconds);
        }
        private static void ParallelForSetProcuct_100()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<Product> ProductList = new List<Product>();
            Parallel.For(1, 100, index =>
            {
                ProductList.Add(GetProduct(index));
                Console.WriteLine("ForSetProcuct SetProcuct index: {0}", index);
            });
            sw.Stop();
            Console.WriteLine("ForSetProcuct SetProcuct 20000 执行完成 耗时：{0}", sw.ElapsedMilliseconds);
        }

        private static void ParalleTest()
        {
            Thread.Sleep(3000);
            Stopwatch swTask = Stopwatch.StartNew();
            /*执行并行操作*/
            Parallel.Invoke(SetProcuct1_500, SetProcuct2_500, SetProcuct3_500, SetProcuct4_500);
            swTask.Stop();
            Console.WriteLine("500条数据 并行编程所耗时间:" + swTask.ElapsedMilliseconds);

            Thread.Sleep(3000);/*防止并行操作 与 顺序操作冲突*/
            Stopwatch sw = Stopwatch.StartNew();
            SetProcuct1_500();
            SetProcuct2_500();
            SetProcuct3_500();
            SetProcuct4_500();
            sw.Stop();
            Console.WriteLine("500条数据  顺序编程所耗时间:" + sw.ElapsedMilliseconds);

            Thread.Sleep(3000);
            swTask.Restart();
            /*执行并行操作*/
            Parallel.Invoke(() => SetProcuct1_10000(), () => SetProcuct2_10000(), () => SetProcuct3_10000(), () => SetProcuct4_10000());
            swTask.Stop();
            Console.WriteLine("10000条数据 并行编程所耗时间:" + swTask.ElapsedMilliseconds);

            Thread.Sleep(3000);
            sw.Restart();
            SetProcuct1_10000();
            SetProcuct2_10000();
            SetProcuct3_10000();
            SetProcuct4_10000();
            sw.Stop();
            Console.WriteLine("10000条数据 顺序编程所耗时间:" + sw.ElapsedMilliseconds);

            Console.ReadLine();
        }

        private static void SetProcuct1_500()
        {
            List<Product> ProductList = new List<Product>();
            for (int index = 0; index < 500; index++)
            {
                ProductList.Add(GetProduct(index));
            }
            Console.WriteLine("SetProcuct1 执行完成");
        }
        private static void SetProcuct2_500()
        {
            List<Product> ProductList = new List<Product>();
            for (int index = 500; index < 1000; index++)
            {
                ProductList.Add(GetProduct(index));
            }
            Console.WriteLine("SetProcuct2 执行完成");
        }
        private static void SetProcuct3_500()
        {
            List<Product> ProductList = new List<Product>();
            for (int index = 1000; index < 1500; index++)
            {
                ProductList.Add(GetProduct(index));
            }
            Console.WriteLine("SetProcuct3 执行完成");
        }
        private static void SetProcuct4_500()
        {
            List<Product> ProductList = new List<Product>();
            for (int index = 1500; index < 2000; index++)
            {
                ProductList.Add(GetProduct(index));
            }
            Console.WriteLine("SetProcuct4 执行完成");
        }
        private static void SetProcuct1_10000()
        {
            List<Product> ProductList = new List<Product>();
            for (int index = 0; index < 10000; index++)
            {
                ProductList.Add(GetProduct(index));
            }
            Console.WriteLine("SetProcuct1 执行完成");
        }
        private static void SetProcuct2_10000()
        {
            List<Product> ProductList = new List<Product>();
            for (int index = 10000; index < 20000; index++)
            {
                ProductList.Add(GetProduct(index));
            }
            Console.WriteLine("SetProcuct2 执行完成");
        }
        private static void SetProcuct3_10000()
        {
            List<Product> ProductList = new List<Product>();
            for (int index = 20000; index < 30000; index++)
            {
                ProductList.Add(GetProduct(index));
            }
            Console.WriteLine("SetProcuct3 执行完成");
        }
        private static void SetProcuct4_10000()
        {
            List<Product> ProductList = new List<Product>();
            for (int index = 30000; index < 40000; index++)
            {
                ProductList.Add(GetProduct(index));
            }
            Console.WriteLine("SetProcuct4 执行完成");
        }

        private static Product GetProduct(int index)
        {
            return new Product
            {
                Category = "Category" + index,
                Name = "Name" + index,
                SellPrice = index
            };
        }

        private static void UseLockTime()
        {
            int x = 0;
            const int iterationNumber = 5000000;
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < iterationNumber; i++)
            {
                x++;
            }
            Console.WriteLine($"不使用锁情况下花费的时间：{sw.ElapsedMilliseconds}");
            sw.Restart();

            for (int i = 0; i < iterationNumber; i++)
            {
                Interlocked.Increment(ref x);
            }
            Console.WriteLine($"使用的情况下花费的时间：{sw.ElapsedMilliseconds}");
            Console.ReadKey();
        }

        private static void UseTwoThreads()
        {
            Thread thread1 = new Thread(SaleTicketThread1);
            Thread thread2 = new Thread(SaleTicketThread2);
            thread1.Start();
            thread2.Start();
            Console.ReadKey();
        }

        private static void SaleTicketThread1()
        {
            while (tickeds > 0)
            {
                try
                {
                    Monitor.Enter(gloalObj);
                    Thread.Sleep(1);
                    Console.WriteLine($"线程1出票：{tickeds--}");
                }
                finally
                {
                    Monitor.Exit(gloalObj);
                }
            }
        }

        private static void SaleTicketThread2()
        {
            while (tickeds > 0)
            {
                try
                {
                    Monitor.Enter(gloalObj);
                    Thread.Sleep(1);
                    Console.WriteLine($"线程2出票：{tickeds--}");
                }
                finally
                {
                    Monitor.Exit(gloalObj);
                }
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

    class Product
    {
        public string Name { get; set; }

        public string Category { get; set; }

        public int SellPrice { get; set; }
    }
    class Custom
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Collections;

namespace ThreadExample
{
    class Program
    {
        public static Mutex mutex = new Mutex();
        private static int param1;
        private static int Max;
        private static List<int> list1 = new List<int>();
        private static int param3;
        private static int count = 0;

        private void Func(object obj)
        {
            
            while (param1 < Max)
            {
                mutex.WaitOne();
                if (param1 < Max)
                {
                    int val = param1;
                    param1 += 1;
                    if (count <= 0)
                    {
                        list1.Add(0);
                    }
                    list1.Add(Thread.CurrentThread.ManagedThreadId);
                    count += 1;
                    if (list1[count - 1] != list1[count])
                    {
                        Console.WriteLine("线程ID：{0} 原值：{1} 增加后的值：{2}", Thread.CurrentThread.ManagedThreadId.ToString(), val, param1);

                        string log = "线程ID：" + Thread.CurrentThread.ManagedThreadId.ToString() +
                                                "   " + "原值：" + val.ToString() + "   " + "增加后的值：" + param1.ToString();
                        WriteLog(log);
                        //Thread.Sleep(1000);
                    }

                }
                mutex.ReleaseMutex();// 释放互斥锁
            }
            

        }
        static void Main(string[] args)
        {
  //            param1 = int.Parse(args[0]);
   //           Max = int.Parse(args[1]);
            param1 = int.Parse(Console.ReadLine());
            Max = int.Parse(Console.ReadLine());
            param3 = int.Parse(Console.ReadLine());
            Program tm = new Program();
            for (int i = 0; i < param3 /*int.Parse(args[2])*/; i++)
            {
                //在不同的线程中调用受互斥锁保护的方法
                Thread test = new Thread(new ParameterizedThreadStart(delegate(object obj)
                    {
                        tm.Func(i);
                    }));
            test.Start(i);
 //           test.Join();//阻塞Main方法
            }
            Console.Read();
        }

        public void WriteLog(string msg)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string logPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine("消息：" + msg);
                    sw.WriteLine("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    sw.WriteLine("**************************************************");
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch (IOException e)
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine("异常：" + e.Message);
                    sw.WriteLine("时间：" + DateTime.Now.ToString("yyy-MM-dd HH:mm:ss"));
                    sw.WriteLine("**************************************************");
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
        }
    }
}

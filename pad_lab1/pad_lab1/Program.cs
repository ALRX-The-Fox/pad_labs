using System;
using System.Threading;

class Program
{
    //для управления синхронизацией потоков
    static ManualResetEvent compitionCallback1 = new ManualResetEvent(false); 
    static ManualResetEvent compitionCallback2 = new ManualResetEvent(false); 
    static ManualResetEvent compitionCallback3 = new ManualResetEvent(false); 

    static void Main(string[] args)
    {
        Thread thread1 = new Thread(() => DoWorkWithWait(1, new int[] { } , new ManualResetEvent[] { }, compitionCallback1));
        Thread thread2 = new Thread(() => DoWorkWithWait(2, new int[] { 1, 3 }, new[] { compitionCallback1, compitionCallback3 }, compitionCallback2));
        Thread thread3 = new Thread(() => DoWorkWithWait(3, new int[] { 1 }, new[] { compitionCallback1 }, compitionCallback3));
        Thread thread4 = new Thread(() => DoWorkWithWait(4, new int[] { 3 }, new[] { compitionCallback2 }));
        Thread thread5 = new Thread(() => DoWorkWithWait(5, new int[] { 3 }, new[] { compitionCallback2 }));

        thread1.Start();
        thread2.Start();
        thread3.Start();
        thread4.Start();
        thread5.Start();

        thread1.Join();
        thread2.Join();
        thread3.Join();
        thread4.Join();
        thread5.Join(); // джойны нужны чтоб главный поток не завершился раньше времени

        Console.WriteLine("Все потоки завершены.");
    }

    static void DoWorkWithWait(int taskNumber, int[] waitngList, ManualResetEvent[] waitEvents, ManualResetEvent ResetEvent = null)
    {

        if (waitEvents.Count() > 0)
        {
            string waitingForThreads = String.Join(", ", waitngList); // наводим красоту

            Console.WriteLine($"Поток {taskNumber} ждет разрешения начать работу от {waitingForThreads}");
            WaitHandle.WaitAll(waitEvents);
            Console.WriteLine($"Поток {taskNumber} начал работу (после завершения необходимых потоков)");
        }
        else
        {
            Console.WriteLine($"Поток {taskNumber} начал работу");
        }

        
        Thread.Sleep(2000); // чтоб сделать вид что что-то делается
        Console.WriteLine($"Поток {taskNumber} завершил работу.");

        // если есть ResetEvent для завершения, отправляем сигнал
        ResetEvent?.Set();
    }
}

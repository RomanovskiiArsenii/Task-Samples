#region TASK IDISPOSABLE
/*
“Task implements IDisposable and exposes a Dispose method.  
Does that mean I should dispose of all of my tasks?”

“No.  Don’t bother disposing of your tasks, not unless performance 
or scalability testing reveals that you need to dispose of them based 
on your usage patterns in order to meet your performance goals.  
If you do find a need to dispose of them, only do so when it’s easy 
to do so, namely when you already have a point in your code where 
you’re 100% sure that they’re completed and that no one else is using them.”
*/
#endregion

#region TASK START, RUNSYNCHRONOUSLY
/*
Task.Run() запускает новую задачу в пуле потоков. 
Задачу можно запустить синхронно (RunSynchronously) и асинхронно (Start)
 
class Program
{
    static void MyTask()
    {
        Console.WriteLine("\nMyTask запущен в потоке {0}", Thread.CurrentThread.ManagedThreadId);

        for (int i = 0; i < 10; i++)
        {
            Thread.Sleep(200);
            Console.Write("+ ");
        }

        Console.WriteLine("\nMyTask завершен в потоке {0}", Thread.CurrentThread.ManagedThreadId);
    }
    static void Main()
    {
        Console.WriteLine("\nMain запущен в потоке {0}", Thread.CurrentThread.ManagedThreadId);

        Action action = new Action(MyTask);

        Task task = new Task(action);           //создание экземпляра задачи

        task.Start();                           //Запуск задачи на выполнение асинхронно (Main и MyTask будут запущены в разных потоках)

        //task.RunSynchronously();              //Запуск задачи на выполнение синхронно (Main и MyTask будут запущены в одном потоке)

        for (int i = 0; i < 10; i++)
        {
            Thread.Sleep(200);
            Console.Write("- ");
        }

        Thread.Sleep(200);
        Console.WriteLine("\nMain завершен в потоке {0}", Thread.CurrentThread.ManagedThreadId);
    }
}
ВЫВОД:
Main запущен в потоке 1

MyTask запущен в потоке 6
+ - + - - + + - - + + - - + + - - + - +
MyTask завершен в потоке 6

Main завершен в потоке 1
*/
#endregion

#region СВОЙСТВА ID, CURRENTID
/*
//Id - уникальный идентификатор определенного экземпляра Task
//CurrentId - уникальный идентификатор выполняющей задачи 
class Program
{
    static void MyTask()
    {
        Console.WriteLine("MyTask: CurrentId - {0}  MangedThreadId -  {1} запущен",
            Task.CurrentId, Thread.CurrentThread.ManagedThreadId);

        Thread.Sleep(2000);

        Console.WriteLine("MyTask: CurrentId - {0}  MangedThreadId -  {1} завершен",
            Task.CurrentId, Thread.CurrentThread.ManagedThreadId);
    }
    static void Main()
    {
        Console.WriteLine("Main выполняется в контексте задачи? CurrentId = {0}",
        Task.CurrentId == null ? "null" : Task.CurrentId.ToString());

        Task task1 = new Task(MyTask);
        Task task2 = new Task(MyTask);

        task1.Start();
        task2.Start();

        Console.WriteLine("Id задачи Task1: {0}", task1.Id);
        Console.WriteLine("Id задачи Task2: {0}", task2.Id);

        Console.ReadKey();
    }
}
ВЫВОД:
Main выполняется в контексте задачи? CurrentId = null
Id задачи Task1: 1
Id задачи Task2: 2
MyTask: CurrentId - 2  MangedThreadId - 7 запущен
MyTask: CurrentId - 1  MangedThreadId - 6 запущен
MyTask: CurrentId - 2  MangedThreadId - 7 завершен
MyTask: CurrentId - 1  MangedThreadId - 6 завершен
*/
#endregion

#region СВОЙСТВО STATUS
/*
Свойство Status позволяет определить состояние задачи в текущий момент

class Program
{
    static void MyTask()
    {
        Thread.Sleep(3000);
    }
    static void Main()
    {
        Task task = new Task(MyTask);

        Console.WriteLine($"1. {task.Status}");     //задача создана
        
        task.Start();
        Console.WriteLine($"2. {task.Status}");     //задача ожидает выполнения (собирается)

        Thread.Sleep(1000);
        Console.WriteLine($"3. {task.Status}");     //задача выполняется

        Thread.Sleep(3000);
        Console.WriteLine($"4. {task.Status}");     //задача завершилась
    }
}
ВЫВОД:
1. Created
2. WaitingToRun
3. Running
4. RanToCompletion
*/
#endregion

#region СВОЙСТВО ISBACKGROUND
/*
//чтобы задача не завершилась с основным потоком,
//нужно изменить ее свойство IsBackground на false
class Program
{
    static void MyTask()
    {
        Thread.CurrentThread.IsBackground = false;              //По умолчанию IsBackground = true

        for (int i = 0; i < 50; i++) 
        { 
            Thread.Sleep(5);
            Console.Write("+");
        }
        Console.WriteLine("MyTask завершен");
    }
    static void Main()
    {
        Task task = new Task(MyTask);
        task.Start();

        Thread.Sleep(200);

        Console.WriteLine("Main завершен");
    }
}
ВЫВОД:
++++++++++++++Main завершен
++++++++++++++++++++++++++++++++++++MyTask завершен
*/
#endregion

#region ПАРАМЕТР ASYNCSTATE - ПЕРЕДАЧА ПАРАМЕТРОВ В ЗАДАЧИ
/*
В задачу могут быть переданы дополнительные параметры, упакованные в объект
 * 
class Program
{
    static void MyTask(object arg)
    {
        for (int i = 0; i < 120; i++) 
        {
            Thread.Sleep(5);
            Console.Write(arg as string);
        }
    }
    static void Main()
    {
        //Action<object> action = MyTask;
        //Task task = new Task(action, "+");

        // первый аргумент - Action<object> action, второй - AsyncState
        Task task = new Task(MyTask, "+");        //аналогично
        
        task.Start();

        Thread.Sleep(1000);
        Console.WriteLine($"[task.AsyncState = {task.AsyncState as string}]");
    }
}
*/
#endregion

#region WAIT ОЖИДАНИЕ ЗАДАЧИ
/*
Аналогично методу Join у потоков, в задачах есть метод Wait, позволяющий 
дождаться окончания выполнения экземпляра задачи
 * 
class Program
{
    static void MyTask()
    {
        for (int i = 0; i < 120; i++)
        {
            Thread.Sleep(25);
            Console.Write("+");
        }
    }
    static void Main()
    {
        Task task = new Task(MyTask);
        task.Start();

        Thread.Sleep(200);          //даем задаче немного выполниться

        //задача не выполнена на этом моменте
        Console.WriteLine($"\nЗадача завершилась: {task.IsCompleted}");     //false

        //ожидаем ее завершения (работает как Join() у Threads)
        task.Wait();

        //задача выполнена
        Console.WriteLine($"\nЗадача завершилась: {task.IsCompleted}");     //true
        
        //*******************************************************************************
        //вариант 2:
        //while (!task.IsCompleted) Thread.Sleep(100);
        //Console.WriteLine($"\nЗадача завершилась: {task.IsCompleted}");   //true

        //вариант 3:
        //IAsyncResult result = task as IAsyncResult;
        //ManualResetEvent waithandle = result.AsyncWaitHandle as ManualResetEvent;
        //waithandle.WaitOne();
    }
}
*/
#endregion

#region WAITALL, WAITANY ОЖИДАНИЕ ЗАДАЧ (ВСЕХ ИЛИ ЛЮБЫХ)
/*
WaitAll - ожидание завершения всех перечисленных задач
WaitAny - ожидание завершения любой из перечисленных задач

class Program
{
    static void MyTask1()
    {
        Console.WriteLine($"MyTask: CurrentId: {Task.CurrentId} запущен");
        Thread.Sleep(2000);
        Console.WriteLine($"MyTask: CurrentId: {Task.CurrentId} завершен");
    }
    static void MyTask2()
    {
        Console.WriteLine($"MyTask: CurrentId: {Task.CurrentId} запущен");
        Thread.Sleep(3000);
        Console.WriteLine($"MyTask: CurrentId: {Task.CurrentId} завершен");
    }


    static void Main()
    {
        Console.WriteLine("Основной поток запущен");

        Task task1 = new Task(MyTask1);
        Task task2 = new Task(MyTask2);

        task1.Start();
        task2.Start();

        //основной поток ждет завершения всех перечисленных задач
        //Task.WaitAll(task1 , task2);                    

        //основной поток ждет завершения любой из перечисленных задач
        Task.WaitAny(task1 , task2);

        Console.WriteLine("Основной поток завершен");
    }
ВЫВОД:
Основной поток запущен
MyTask: CurrentId: 8 запущен
MyTask: CurrentId: 9 запущен
MyTask: CurrentId: 8 завершен
Основной поток завершен
*/
#endregion

#region TASKFACTORY ФАБРИКА ЗАДАЧ
/*
class Program
{
    static void MyTask()
    {
        for (int i = 0; i < 120; i++)
        {
            Thread.Sleep(25);
            Console.Write("+");
        }
    }
    static void Main()
    {
        //Task task = Task.Factory.StartNew(MyTask);
        //start не требуется

        //ИЛИ
        TaskFactory factory = new TaskFactory();
        Task task = factory.StartNew(MyTask);

        task.Wait();
    }
}
*/
#endregion

#region CONTINUEWITH ПРОДОЛЖЕНИЕ ЗАДАЧИ
/*
При помощи метода ContinueWith можно продолжить выполнение любого потока 
любым методом с входным параметром типа Task

class Program
{
    static void MyTask()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        for (int i = 0; i < 10; i++)
        {
            Thread.Sleep(200);
            Console.Write("1");
        }
    }
    //продолжение задачи обязательно должно иметь аргумент типа Task
    static void MyTaskContinuation(Task task)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        for (int i = 0; i < 10; i++)
        {
            Thread.Sleep(200);
            Console.Write("0");
        }
    }
    static void Main()
    {
        //Action action = new Action(MyTask);
        //Task task = new Task(action);
        //сокращенно:
        
        Task task = new Task(MyTask);

        //Action<Task> continuation = new Action<Task>(MyTaskContinuation); 
        //Task taskContinuation = task.ContinueWith(continuation);
        //сокращенно:

        Task taskContinuation = task.ContinueWith(MyTaskContinuation);

        task.Start();

        Task.WaitAll(task, taskContinuation);
    }
}
ВЫВОД:
11111111110000000000
*/
#endregion

#region RESULT ВОЗВРАЩАЕМОЕ ЗНАЧЕНИЕ ЗАДАЧИ
/*
Task может работать не только с методами, принимающими аргумент типа object,
но и с методами, принимающими аргумент типа object и возвращающими значение

Аналогично тому, как мы сипользовали передавали в задачу входной параметр AsyncState 
Task task = new Task(MyMethod, "Hello, World!");
нужно всего лишь указать возвращаемый тип в угловых скобках:
Task<string> task = new Task<int>(MyMethod, "Hello, World!");
возвращаемое значение метода можно будет получить в свойстве result

class Program
{
    struct Context
    {
        public int a, b;
    }

    static int Sum(object arg)
    {
        int a = ((Context)arg).a;
        int b = ((Context)arg).b;

        return a + b;
    }
    static void Main()
    {
        Console.WriteLine("Основной поток запущен.");
        
        Context context = new Context() { a = 2, b = 3 };

        //1 вариант
        Task<int> task = new Task<int>(Sum, context);
        task.Start();
        task.Wait();
        
        //2 вариант
        //TaskFactory<int> factory = new TaskFactory<int>();
        //Task<int> task = factory.StartNew(Sum, context); 
        //сокращенно:
        //Task <int> task = Task<int>.Factory.StartNew(Sum, context);
        
        Console.WriteLine($"Результат сложения = {task.Result}");

        Console.WriteLine("Основной поток завершен.");

    }
}
ВЫВОД:
Основной поток запущен.
Результат сложения = 5
Основной поток завершен.
*/
#endregion

#region СВЯЗЬ EXCEPTION И TASK.WAIT
/*
Если в задаче возникает исключение, в блоке try обязательно требуется вызвать Wait

class Program
{
    static void MyTask()
    {
        Console.WriteLine("Задача запущена");

        throw new Exception();

        Console.WriteLine("Задача завершена");      //не выпполнится
    }

    static void Main()
    {
        Console.WriteLine("Основной поток запущен.");

        Task task = new Task(MyTask);

        try
        {
            task.Start();
            //Thread.Sleep(1000);   //даже если сделать задержку, исключение не будет перехвачено

            task.Wait();            //для обработки исключения обязательно вызвать wait
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception:\t\t {ex.GetType()}");
            Console.WriteLine($"Message:\t\t {ex.Message}");

            if (ex.InnerException != null)
                Console.WriteLine($"Inner Exception:\t {ex.InnerException.Message}");
        }
        finally
        {
            Console.WriteLine($"Task status:\t\t {task.Status}");
        }
        Console.WriteLine("Основной поток завершен.");
    }
}
/*
Основной поток запущен.
Задача запущена
Exception:               System.AggregateException
Message:                 One or more errors occurred. (Exception of type 'System.Exception' was thrown.)
Inner Exception:         Exception of type 'System.Exception' was thrown.
Task status:             Faulted
Основной поток завершен.
*/
#endregion

#region CANCELLATIONTOKEN ОТМЕНА ЗАДАЧИ
/*
В AsyncState также можно передавать токен отмены
 * 
class Program
{
    static void MyTask(object arg)
    {
        Thread.CurrentThread.IsBackground = false;
        Console.WriteLine("MyTask запущен");
        CancellationToken token = (CancellationToken)arg;

        //Если задача отменена сразу после старта
        //вызвать исключение OperationCanceledException
        token.ThrowIfCancellationRequested();

        for (int i = 0; i < 120; i++)
        {
            //каждую итерацию проверяем, не отменена ли задача?
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("\nПолучен запрос на отмену задачи");

                //выбросить исключение
                token.ThrowIfCancellationRequested();
            }
            
            Thread.Sleep(25);
            Console.Write("+");
        }
        Console.WriteLine("MyTask завершен");
    }
    static void Main()
    {
        Console.WriteLine("Основной поток запущен.");

        //фабрика токенов
        CancellationTokenSource cancellation = new CancellationTokenSource();
        //получить у фабрики один токен
        CancellationToken token = cancellation.Token;

        //задаче сообщаем метод и токен (сейчас токен не просит отмены задачи)
        Task task = new Task(MyTask, token);
        task.Start();

        //время на выполнение задачи 
        Thread.Sleep(500);

        try
        {
            cancellation.Cancel();      //отмена выполняемой задачи
            task.Wait();                //обязательно вызывается для перехвата исключения
        }
        catch (AggregateException ex)
        {
            Console.WriteLine("Статус задачи: {0}", task.Status);
            Console.WriteLine("{0}\n{1}", ex.GetType, ex.Message);
        }

        Console.WriteLine("Основной поток завершен.");
    }
}
ВЫВОД:
Основной поток запущен.
MyTask запущен
+++++++++++++++
Получен запрос на отмену задачи
Статус задачи: Faulted
System.Func`1[System.Type]
One or more errors occurred. (The operation was canceled.)
Основной поток завершен.
*/
#endregion

#region TASKCONTINUATIONOPTIONS ПУТИ ПРОДОЛЖЕНИЯ ЗАДАЧИ
/*
//Возможно три пути продолжения задачи, в зависисмости от того
//выполнена она, отменена или провалена
//при помощи параметра TaskContinuationOptions

class Program
{
    static byte MyTask()
    {
        byte b = 255;

        checked       //если снять комментарий, будет вызвано исключение при переполнении
        {
            b++;        //без checked, byte будет переполнен и станет равным нулю
        }
        return b;
    }
    static void BadContinuation(Task<byte> task)
    {
        Console.WriteLine("Task is not completed! Status: {0}", task.Status);   
    }
    static void GoodContinuation(Task<byte> task)
    {
        Console.WriteLine("Task succesfully completed! Status: {0}", task.Status);
        Console.WriteLine("Result: {0}", task.Result);
    }
    static void Main()
    {
        Task<byte> task = new Task<byte>(MyTask);       //задача с возвращаемым значением
        Task continuation;                              //задача продолжения
        
        //два пути продолжения задачи:
        //продолжение при успешном выполнении задачи
        continuation = task.ContinueWith(GoodContinuation, TaskContinuationOptions.OnlyOnRanToCompletion);
        //продолжение при провале задачи
        continuation = task.ContinueWith(BadContinuation, TaskContinuationOptions.OnlyOnFaulted);

        //старт потоков
        task.Start();
        
        //ожидание
        Console.ReadKey();
    }
}
ВЫВОД:
если задача выполнена:
Task succesfully completed! Status: RanToCompletion
Result: 0

если задача провалена:
Task is not completed! Status: Faulted
*/
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TaskList
{
    [Serializable()]
    class Date
    {
        public Date(int m, int d, int y, int t)
        {
            Month = m; Day = d; Year = y; Time = t;
        }
        public int Month { get; }
        public int Day { get; }
        public int Year { get; }
        public int Time { get; }
    }

    [Serializable()]
    class Task
    {
        private Date date;
        private string task;

        public Task(Date d, string t)
        {
            date = d; task = t;
        }
        public Date GetDate() { return date; }
        public string GetTask() { return task; }
        public void Print()
        {
            Console.Out.WriteLine(date.Month.ToString() + "/" + date.Day.ToString() + "/" + date.Year.ToString() + " at " + date.Time.ToString() +": " + task);
        }
    }

    class Program
    {
        static List<Task> taskList;
        static Stream stream;
        static BinaryFormatter formatter;

        static void Main(string[] args)
        {
            // Load existing data if present
            if (File.Exists("data.dat"))
            {
                stream = File.Open("data.dat", FileMode.Open);
                formatter = new BinaryFormatter();
                taskList = (List<Task>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                taskList = new List<Task>();
            }

            // Begin mainloop
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            while(true)
            {
                MainMenu();
            }
        }

        static void MainMenu()
        {
            Console.Clear();
            short option = 0;
            Console.Out.WriteLine("-------- Task Scheduler --------");
            Console.Out.WriteLine("\tEnter an option:\n\n\t(1) Show Tasks\n\t(2) Add a Task\n\t(3) Remove a Task\n\t(4) Exit");
            Console.Out.WriteLine("--------------------------------");

            
            if (!Int16.TryParse(Console.In.ReadLine(), out option))
            {
                Console.Clear();
                return;
            }
            
            switch (option)
            {
                case 1:
                    ShowTasks();
                    break;
                case 2:
                    AddTask();
                    break;
                case 3:
                    RemoveTask();
                    break;
                case 4:
                    Exit();
                    break;
            }
        }

        static void ShowTasks()
        {
            Console.Clear();
            if (taskList.Count == 0) { return; }

            foreach (Task t in taskList)
            {
                t.Print();
            }
            Console.Out.WriteLine("\nPress any key to continue...");
            Console.In.Read();
        }

        static void AddTask()
        {
            Console.Clear();

            int month = 0, day = 0, year = 0, time = 0;
            string task;

            Console.Out.Write("Month: ");
            while (!Int32.TryParse(Console.In.ReadLine(), out month) || (month < 1 || month > 12))
            {
                Console.Out.WriteLine("Enter a valid month.");
            }
            Console.Out.Write("Day: ");
            while (!Int32.TryParse(Console.In.ReadLine(), out day) || (day < 1 || day > 31))
            {
                Console.Out.WriteLine("Enter a valid day.");
            }
            Console.Out.Write("Year: ");
            while (!Int32.TryParse(Console.In.ReadLine(), out year)) ;
            Console.Out.Write("Time (Format 0000 - 2400): ");
            while (!Int32.TryParse(Console.In.ReadLine(), out time) || (time < 0 || time > 2400))
            {
                Console.Out.WriteLine("Enter a valid time.");
            }
            Console.Out.Write("Task: ");
            task = Console.In.ReadLine();

            Date newDate = new Date(month, day, year, time);
            Task newTask = new Task(newDate, task);
            taskList.Add(newTask);
            SortTasks();
        }

        static void RemoveTask()
        {
            int removeLocation;

            Console.Clear();
            if (taskList.Count == 0) { return; }
            for (int i = 0; i < taskList.Count; ++i)
            {
                Console.Out.Write("(" + i + "): ");
                taskList[i].Print();
            }
            Console.Out.Write("Select an index to remove: ");
            Int32.TryParse(Console.In.ReadLine(), out removeLocation);
            if (removeLocation >= 0 && removeLocation < taskList.Count)
            {
                taskList.RemoveAt(removeLocation);
            }
            else
            {
                Console.Out.WriteLine("Not a valid index.");
            }
        }

        // Returns true if t1 is less than t2
        static bool DateComp(Task t1, Task t2)
        {
            if (t1.GetDate().Year < t2.GetDate().Year)
            {
                return true;
            }
            if (t1.GetDate().Year == t2.GetDate().Year && t1.GetDate().Month < t2.GetDate().Month)
            {
                return true;
            }
            if (t1.GetDate().Year == t2.GetDate().Year && t1.GetDate().Month == t2.GetDate().Month && t1.GetDate().Day < t2.GetDate().Day)
            {
                return true;
            }
            if (t1.GetDate().Year == t2.GetDate().Year && t1.GetDate().Month == t2.GetDate().Month && t1.GetDate().Day == t2.GetDate().Day && t1.GetDate().Time < t2.GetDate().Time)
            {
                return true;
            }
            return false;
        }

        static void SortTasks()
        {
            if (taskList.Count < 2) { return; }

            bool sorted = false;
            while (!sorted)
            {
                sorted = true;
                for(int i = 0; i < taskList.Count - 1; ++i)
                {
                    if (!DateComp(taskList[i], taskList[i+1]))
                    {
                        Task temp = taskList[i];
                        taskList[i] = taskList[i + 1];
                        taskList[i + 1] = temp;
                        sorted = false;
                    }
                }
            }
        }

        static void Exit()
        {
            // Save task list
            stream = File.Open("data.dat", FileMode.Create);
            formatter = new BinaryFormatter();
            formatter.Serialize(stream, taskList);
            stream.Close();

            Environment.Exit(0);
        }
    }
}

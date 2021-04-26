using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ezThread
{
    public class JobManager
    {
        private List<EZTHREAD> ezthreads = new List<EZTHREAD>();
        Queue<Job> qjob = new Queue<Job>();
        private bool started = false;


        public JobManager(List<Job> Jobs, int threadstouse)
        {
            qjob = new Queue<Job>(Jobs);
            for (int i = 0; i < threadstouse; i++)
            {
                addThread();
            }

        }
        public JobManager(int threadstouse)
        {
            for (int i = 0; i < threadstouse; i++)
            {
                addThread();
            }
        }
        //Requires the cancellation token source of the token you gave to your threads
        public JobManager(List<EZTHREAD> threads)
        {
            ezthreads = threads;
        }
        public JobManager()
        {
        }

        //Starts the threads
        public void startThreads()
        {
            if (ezthreads.Count <= 0)
            {
                throw new Exception("Thread list empty");
            }
            else if (qjob.Count <= 0)
            {
                throw new Exception("Job list empty");
            }
            else
            {
                foreach (var thread in ezthreads)
                {
                    thread.start();
                }

                started = true;
            }
        }
        //Pauses all threads
        public void pauseThreads()
        {
            foreach (var ezthread in ezthreads)
            {
                ezthread.pause();
            }
            started = false;
        }
        //Resumes all threads
        public void resumeThreads()
        {
            foreach (var ezthread in ezthreads)
            {
                ezthread.resume();
            }
            started = true;
        }
        //Returns true if all threads are paused
        public bool isPaused()
        {
            foreach (var ezthread in ezthreads)
            {
                if (!ezthread.isPaused())
                {
                    return false;
                }
            }

            return true;
        }
        //Kills all the threads
        public void killThreads()
        {
            foreach (var ezthread in ezthreads)
            {
               ezthread.killThread();
            }
            started = false;
        }

        //Removes job to list
        public void removeJob(Job j)
        {
            qjob = new Queue<Job>(qjob.Where(s => s != j));
        }
        //Adds job to list
        public void addJob(Job j)
        {
            qjob.Enqueue(j);
        }
        //Kills specific thread (Thread IDs start at 0 and ends at threadAmount-1)
        public void killThread(int id)
        {
            ezthreads[id].killThread();
        }
        //Returns true if all threads are done/off
        public bool isDone()
        {
            foreach (var ezthread in ezthreads)
            {
                if (ezthread.t.IsAlive)
                {
                    return false;
                }
            }

            started = false;
            return true;
        }
        //Adds thread to thread list and starts them if the manager is running
        public void addThread()
        {
            EZTHREAD ezt = new EZTHREAD(qjob, ezthreads.Count);
            ezthreads.Add(ezt);
            if (started)
            {
                ezt.start();
            }
        }
        //Deletes specified amount of threads (Use it when the manager isn't running)
        public void decreaseThreads(int amount)
        {
            if (amount <= ezthreads.Count)
            {
                for (int i = 0; i <= amount; i++)
                {
                    ezthreads.Last().killThread();
                    ezthreads.Remove(ezthreads.Last());
                }
            }
            else
            {
                foreach (EZTHREAD ezt in ezthreads)
                {
                    ezthreads.Last().killThread();
                }
                ezthreads.Clear();
            }
          
        }
        public void decreaseThreads(double percentage)
        {
            int amount = Convert.ToInt32(ezthreads.Count * (percentage / 100));
            if (amount <= ezthreads.Count)
            {
                for (int i = 0; i <= amount; i++)
                {
                    ezthreads.Last().killThread();
                    ezthreads.Remove(ezthreads.Last());
                }
            }
            else
            {
                foreach (EZTHREAD ezt in ezthreads)
                {
                    ezthreads.Last().killThread();
                }
                ezthreads.Clear();
            }
        }

        public int threadAmount()
        {
            return ezthreads.Count;
        }
    }

    public class EZTHREAD
    {
        public Thread t;
        public readonly int ID;
        private Queue<Job> jobs;
        private bool paused = false;
        private bool killthread = false;
        public EZTHREAD(Queue<Job> lj, int id)
        {
            jobs = lj;
            ID = id;
            ThreadStart ts = new ThreadStart(() =>
            {
                while (true)
                {
                    if (paused)
                    {
                        try
                        {
                            Thread.Sleep(Timeout.Infinite);
                        }
                        catch
                        {
                        }
                    }
                    else if (killthread)
                    {
                        break;
                    }
                    else if (jobs.Count != 0)
                    {
                        var j = jobs.Dequeue();
                        if (j != null)
                        {
                            j.execute();
                        }
                    }
                    else
                    {
                        break;
                    }

                }

            });
            t = new Thread(ts);
        }

        public void start()
        {
            killthread = false;
            ThreadStart ts = new ThreadStart(() =>
            {
                Job j = new Job(() => { });
                while (true)
                {
                    if (paused)
                    {
                        try
                        {
                            Thread.Sleep(Timeout.Infinite);
                        }
                        catch
                        {
                        }
                    }
                    else if (killthread)
                    {
                        break;
                    }
                    else if (jobs.Count != 0)
                    {
                        lock (jobs)
                        { 
                            j = jobs.Dequeue();
                        }
                        if (j != null)
                        {
                            j.execute();
                        }
                    }
                    else
                    {
                        break;
                    }

                }

            });
            t = new Thread(ts);
            t.Start();
        
        }
        public void pause()
        {
            paused = true;
        }
        public void resume()
        {
            paused = false;
            t.Interrupt();
        }
        public bool isPaused()
        {
            return paused;
        }
        public void killThread()
        {
            killthread = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;

namespace ezThread
{
    public class JobManager
    {
        private List<EZTHREAD> ezthreads = new List<EZTHREAD>();
        private List<Job> jobs = new List<Job>();
        public int threads;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public JobManager(List<Job> Jobs, int threadstouse)
        {
            jobs = Jobs;
            threads = threadstouse;
        }
        //Requires the cancellation token source of the token you gave to your threads
        public JobManager(List<EZTHREAD> threads, CancellationTokenSource threadancellationTokenSource)
        {
            ezthreads = threads;
            cts = threadancellationTokenSource;
        }
        //Creates the threads and allocates jobs to them. Use this if you add or remove a job
        public void build()
        {
            ezthreads.Clear();
            int jobsperthread = jobs.Count/threads;
            int jindex = 0;
            for (int i = 0; i < threads; i++)
            {
                List<Job> jobsT = new List<Job>();
                if (i >= threads-1)
                {
                    for (int j = jindex; j < jobs.Count; j++)
                    {
                        jobsT.Add(jobs[j]);
                    }
                }
                else
                {
                    for (int j = 0; j < jobsperthread; j++)
                    {
                        jobsT.Add(jobs[jindex]);
                        ++jindex;
                    }
                }
                addThread(jobsT);
            }
        }
        //Starts the threads, Make sure to build() before
        public void startThreads()
        {
            foreach (var thread in ezthreads)
            {
                thread.executeJobs();
            }
        }
        //Pauses all threads
        public void pause()
        {
            foreach (var ezthread in ezthreads)
            {
                ezthread.pause();
            }
        }
        //Resumes all threads
        public void resume()
        {
            foreach (var ezthread in ezthreads)
            {
                ezthread.resume();
            }
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
            cts.Cancel();
        }
        
        //Removes job to list
        public void removeJob(Job j)
        {
            jobs.Remove(j);
        }
        //Adds job to list
        public void addJob(Job j)
        {
            jobs.Add(j);
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

            return true;
        }
        //Adds thread to thread list
        private void addThread(List<Job> lj)
        {
            EZTHREAD ezt = new EZTHREAD(lj, cts.Token, ezthreads.Count);
            ezthreads.Add(ezt);
        } 
    }

    public class EZTHREAD
    {
        public Thread t;
        public readonly int ID;
        private CancellationToken ctx;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private CancellationToken indvctx;
        private List<Job> jobs;
        public int killthreadafter = 0;
        private bool paused = false;
        public EZTHREAD(List<Job> lj, CancellationToken ct, int id)
        {
            jobs = lj;
            ctx = ct;
            ID = id;
            ThreadStart ts = new ThreadStart(() =>
            {
                foreach (Job j in jobs)
                {
                    for (int i = 0; i < j.executions; i++)
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
                        else if (ctx.IsCancellationRequested || indvctx.IsCancellationRequested)
                        {
                            break;
                        }

                        j.execute();
                    }
                }
            });
            
            t = new Thread(ts);
            indvctx = cts.Token;
        }

        public void executeJobs()
        {
            t.Start();
            if (killthreadafter != 0)
            {
                cts.CancelAfter(killthreadafter);
            }
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
            cts.Cancel();
        }
    }
}

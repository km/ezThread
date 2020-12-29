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
        /// <summary>Creates the threads and allocates jobs to them. Use this if you add or remove a job</summary>
        public void setThreads()
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
        /// <summary><say>Starts the threads, Make sure to set threads before</say></summary>
        public void startThreads()
        {
            foreach (var thread in ezthreads)
            {
                thread.executeJobs();
            }
        }
        /// <summary>Kills all the threads</summary>
        public void killThreads()
        {
            cts.Cancel();
        }

        public void removeJob(Job j)
        {
            jobs.Remove(j);
        }
        public void addJob(Job j)
        {
            jobs.Add(j);
        }

        public void killThread(int id)
        {
            ezthreads[id].killThread();
        }
        public bool isDone()
        {
            int doneindex = 0;
            foreach (var ezthread in ezthreads)
            {
                if (ezthread.t.IsAlive)
                {
                    
                }
                else
                {
                    ++doneindex;
                }
            }

            if (doneindex >= threads)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void addThread(List<Job> lj)
        {
            EZTHREAD ezt = new EZTHREAD(lj, cts.Token, ezthreads.Count);
            ezthreads.Add(ezt);
        } 
    }

    class EZTHREAD
    {
        public Thread t;
        public readonly int ID;
        public CancellationToken ctx;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private CancellationToken indvctx;
        private List<Job> jobs;
        public int killthreadafter = 0;
        
        public EZTHREAD(List<Job> lj, CancellationToken ct, int id)
        {
            jobs = lj;
            ctx = ct;
            ID = id;
            ThreadStart ts = new ThreadStart(() =>
            {
                foreach (Job j in jobs)
                {
                    if (ctx.IsCancellationRequested || indvctx.IsCancellationRequested)
                    {
                        break;
                    }
                    j.execute();
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

        public void killThread()
        {
            cts.Cancel();
        }
    }
}

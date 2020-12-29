﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace ezThread
{
    public class JobManager
    {
        List<EZTHREAD> ezthreads = new List<EZTHREAD>();
        List<Job> jobs = new List<Job>();
        private int threads = 0;
        public JobManager(int threadstouse)
        {
            threads = threadstouse;
            for (int i = 0; i < threads; i++)
            {
                
            }
        }

        public void start()
        {

        }

        public void stop()
        {

        }

        private void addThread(List<Job> lj)
        {
            EZTHREAD ezt = new EZTHREAD(lj);
            ezthreads.Add(ezt);
            ezt.id = ezthreads.IndexOf(ezt);
        } 
    }

    class EZTHREAD
    {
        private Thread t;
        public int id;
        public CancellationToken ctx;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private CancellationToken indvctx;
        private List<Job> jobs = new List<Job>();
        public int killthreadafter = 0;
        
        public EZTHREAD(List<Job> lj)
        {
            jobs = lj;
            ThreadStart ts = new ThreadStart(() =>
            {
                foreach (Job j in jobs)
                {
                    if (ctx.IsCancellationRequested || indvctx.IsCancellationRequested)
                    {
                        return;
                    }
                    j.execute();
                }
            });
            t = new Thread(ts);
            t.IsBackground = true;
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

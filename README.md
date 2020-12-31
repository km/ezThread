# ezThread
**ezThread** is a .NET standard library made to make threading easy. Features currently include an easy way to execute functions and cancel them async and a class that'll run your functions with the amount of threads requested.

## Jobs

Job is a class that contains your functions and can execute and cancel async.
Jobs are required for the JobManager class.
### Example

   
```csharp
   Job j = new Job(() =>
        {
            Console.WriteLine("Job1");
        });

//runs the job optional parameter of how many times to execute
j.execute();

//runs the job async optional parameter of how many times to execute
j.executeAsync();

//cancels the execution of the job if its running async
j.cancelExecution();

//Run this to be able to rerun a job after cancelling it
j.resetCancellation();
```
## JobManager

JobManager is a class you can provide a list of jobs too and the amount of threads you want to execute the jobs with.
### Example

 
```csharp
 List<Job> joblist = new List<Job>();
        //Creating a 100 jobs to execute
        for (int i = 0; i < 100; i++)
        {
            int e = i;
            Job j = new Job(() =>
            {
                Console.WriteLine("Job"+e);
            });
            joblist.Add(j);
        }

//Creates a jobmanager class with our list and 10 threads to execute them with
JobManager jobManager = new JobManager(joblist, 10);

//Call this function everytime you create a jobmanager class and everytime you remove or add a job using addJob() or removeJob()
jobManager.setThreads();

//Starts the execution of the jobs
jobManager.startThreads();

//Returns true if all threads are done
bool done = jobManager.isDone();

//Stops the threads from proceeding to the next queued job and terminates them
jobManager.killThreads();
```
## Notes:

 - Do not use more threads than the amount of jobs.
 - Too many threads might slow down the device your running on and bottleneck the program.
 - In networking cases sometimes the more threads you use the slowler it'll execute if they're bottlenecked by the speed of your internet.
 - The killthread() function isn't meant to be used right now, but if you're gonna use it the first thread has an ID of 0 and the last one has an ID of `Threads-1` since threads are stored in a list.
 -  Easier ways to control the threads coming in the future.

using System.Collections.Generic;
using System.Linq;


namespace Assets
{
    public class RunnersQueue
    {
        readonly List<IRunner> queue;
        bool disposed;

        public RunnersQueue()
        {
            queue = new List<IRunner>();
        }

        public bool HasReadyRunner()
        {
            if (disposed) return false;
            lock (queue)
            {
                var disposedRunners = queue.Where(r => r.Disposed).ToArray();
                foreach (var disposedRunner in disposedRunners)
                    queue.Remove(disposedRunner); // hotfix
                return queue.Any(runner => runner.CanStart);
            }
        }

        public IRunner DequeueReadyRunner() // null if noone ready
        {
            if (disposed) return null;
            lock (queue) // в шарпе лок на один и тот же объект в одном треде можно брать сколько угодно раз. дедлока не будет.
                if (HasReadyRunner())
                {
                    var readyRunner = queue.First(runner => runner.CanStart);
                    queue.Remove(readyRunner);
                    return readyRunner;
                }
            return null;
        }

        public void EnqueueRunner(IRunner runner)
        {
            if (disposed) return;
            lock (queue)
                queue.Add(runner);
        }

        public void DisposeRunners()
        {
            lock (queue)
                foreach (var runner in queue)
                    runner.Dispose();
            disposed = true;
        }
    }
}

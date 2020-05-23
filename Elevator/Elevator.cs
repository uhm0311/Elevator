using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace org.uhm.toy.elevator
{
    public class Elevator
    {
        public delegate void ElevatorArrivedListener(int floor);
        public delegate void ElevatorPausedListener(int floor);

        private Thread elevator;
        private ElevatorState state = ElevatorState.Pause;

        private int floor = 1;
        private List<int> requests = new List<int>();

        private bool isRunning = true;
        private int sleep;

        private ElevatorArrivedListener arrivedListener;
        private ElevatorPausedListener pausedListener;

        public Elevator(int sleep = 1000, ElevatorArrivedListener arrivedListener = null, ElevatorPausedListener pausedListener = null)
        {
            this.sleep = sleep;
            this.pausedListener = pausedListener;
            this.arrivedListener = arrivedListener;

            Start();
        }

        public void Start()
        {
            isRunning = true;

            elevator = new Thread(new ThreadStart(Run)) { IsBackground = true };
            elevator.Start();
        }

        public void Stop()
        {
            isRunning = false;
            elevator.Join();
        }

        public void Request(int request)
        {
            if (request != 0 && !requests.Contains(request))
            {
                requests.Add(request);
            }
        }

        public void Abort(int request)
        {
            requests.Remove(request);
        }

        private void Run()
        {
            while (isRunning)
            {
                if (requests.Count > 0)
                {
                    int request = GetRequest();

                    if (request != 0 && request != floor)
                    {
                        ChangeState(request);

                        while (requests.Contains(request) && request != floor)
                        {
                            Thread.Sleep(sleep);

                            ChangeFloor();
                            arrivedListener?.Invoke(floor);

                            if (requests.Contains(floor))
                            {
                                requests.Remove(floor);
                                pausedListener?.Invoke(floor);

                                Thread.Sleep(sleep);
                            }
                        }

                        if (requests.Count == 0)
                        {
                            state = ElevatorState.Pause;
                        }
                    }
                }
            }
        }

        private int GetRequest()
        {
            switch (state)
            {
                case ElevatorState.Up:
                    return requests.Max();

                case ElevatorState.Down:
                    return requests.Min();

                case ElevatorState.Pause:
                    return requests[0];

                default:
                    return 0;
            }
        }

        private void ChangeState(int request)
        {
            if (request > floor)
            {
                state = ElevatorState.Up;
            }
            else if (request < floor)
            {
                state = ElevatorState.Down;
            }
        }

        private void ChangeFloor()
        {
            switch (state)
            {
                case ElevatorState.Up:
                    floor++;
                    break;

                case ElevatorState.Down:
                    floor--;
                    break;
            }
        }
    }
}

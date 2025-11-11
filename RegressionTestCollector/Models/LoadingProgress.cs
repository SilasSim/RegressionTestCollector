using MVVMArchitecture;

namespace RegressionTestCollector.Models
{
    public class LoadingProgress : ObservableObject
    {
        public LoadingProgress(int current, int max)
        {
            Max = max;
            Current = current;
        }

        private int mMax;

        public int Max
        {
            get => mMax;
            set => SetField(ref mMax, value);
        }

        private int mCurrent;

        public int Current
        {
            get => mCurrent;
            set => SetField(ref mCurrent, value);
        }
    }

    public class LoadingProgressEventArgs : EventArgs
    {
        public LoadingProgress Progress { get; }
        public LoadingProgressEventArgs(LoadingProgress progress)
        {
            Progress = progress;
        }
    }
}

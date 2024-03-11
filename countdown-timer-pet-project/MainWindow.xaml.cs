using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace countdown_timer_pet_project;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow() 
    {
        InitializeComponent();
        DataContext = new ViewModel();
    }
    
    public class ViewModel : INotifyPropertyChanged
    {
        private Timer _timer;
        private int _timerValue;
        private int _timerInput;

        public ViewModel()
        {
            StartCommand = new CommandController(StartTimer, CanStartTimer);
            StopCommand = new CommandController(StopTimer);
            ResetCommand = new CommandController(ResetTimer);
        }

        public int TimerValue
        {
            get => _timerValue;
            set
            {
                if (_timerValue != value)
                {
                    _timerValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TimerInput
        {
            get => _timerInput;
            set
            {
                if (_timerInput != value)
                {
                    _timerInput = value;
                    TimerValue = _timerInput;
                    OnPropertyChanged();
                }
            }
        }
        
        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand ResetCommand { get; set; }

        private void StartTimer(object parameter)
        {
            _timer = new Timer(TimerCallback, null, 0, 1000);
        }

        private void StopTimer(object parameter) => _timer?.Dispose();

        private void ResetTimer(object parameter)
        {
            _timer?.Dispose();
            TimerValue = 0;
        }

        private bool CanStartTimer(object parameter) => TimerInput > 0;

        private void TimerCallback(object state)
        {
            if (TimerValue <= 0)
            {
                StopTimer(null);
                MessageBox.Show("Сountdown completed");
            }
            else
            {
                TimerValue--;
            }
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CommandController : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public CommandController(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
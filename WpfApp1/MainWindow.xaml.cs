using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace TicTacToe
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly Random _random = new Random();
        private readonly string[] _players = { "X", "O" };
        private string _currentPlayer;
        private bool _isGameOver;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> Board { get; set; }
        public ICommand CellClickCommand { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitializeGame();
        }

        private void InitializeGame()
        {
            Board = new ObservableCollection<string>(new string[9]);
            CellClickCommand = new RelayCommand(ExecuteCellClick, CanExecuteCellClick);
            _currentPlayer = _players[0];
            _isGameOver = false;
        }

        private bool CanExecuteCellClick(object parameter)
        {
            if (_isGameOver)
                return false;

            int index = (int)parameter;
            return string.IsNullOrEmpty(Board[index]);
        }

        private void ExecuteCellClick(object parameter)
        {
            int index = (int)parameter;
            Board[index] = _currentPlayer;
            CheckForWinner();
            if (!_isGameOver)
            {
                _currentPlayer = (_currentPlayer == _players[0]) ? _players[1] : _players[0];
                if (_currentPlayer == _players[1])
                    RobotMove();
            }
        }

        private void RobotMove()
        {
            int index = _random.Next(0, 9);
            while (!string.IsNullOrEmpty(Board[index]))
                index = _random.Next(0, 9);
            Board[index] = _players[1];
            CheckForWinner();
            _currentPlayer = _players[0];
        }

        private void CheckForWinner()
        {
            
            for (int i = 0; i < 3; i++)
            {
                if (!string.IsNullOrEmpty(Board[i * 3]) &&
                    Board[i * 3] == Board[i * 3 + 1] &&
                    Board[i * 3] == Board[i * 3 + 2])
                {
                    _isGameOver = true;
                    MessageBox.Show($"{Board[i * 3]} wins!");
                    return;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (!string.IsNullOrEmpty(Board[i]) &&
                    Board[i] == Board[i + 3] &&
                    Board[i] == Board[i + 6])
                {
                    _isGameOver = true;
                    MessageBox.Show($"{Board[i]} wins!");
                    return;
                }
            }

          
            if (!string.IsNullOrEmpty(Board[0]) &&
                Board[0] == Board[4] &&
                Board[0] == Board[8])
            {
                _isGameOver = true;
                MessageBox.Show($"{Board[0]} wins!");
                return;
            }

            if (!string.IsNullOrEmpty(Board[2]) &&
                Board[2] == Board[4] &&
                Board[2] == Board[6])
            {
                _isGameOver = true;
                MessageBox.Show($"{Board[2]} wins!");
                return;
            }

          
            if (!Board.Contains(""))
            {
                _isGameOver = true;
                MessageBox.Show("It's a draw!");
                return;
            }
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame();
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}

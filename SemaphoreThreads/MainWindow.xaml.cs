using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SemaphoreThreads;

public partial class MainWindow : Window, INotifyPropertyChanged
{
	public MainWindow()
	{
		InitializeComponent();

		Max = 100;
		SPCount = 1;
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	private int _max;
	private int _spCount;
	public int SPCount { get => _spCount; set { _spCount = value; OnPropertyChanged(); } }
	public int Max { get => _max; set { _max = value; OnPropertyChanged(); } }

	private void CreateNewThreadBtn_Click(object sender, RoutedEventArgs e)
	{
		Semaphore s = new Semaphore(SPCount, Max);
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		if (SPCount > 1)
			SPCount--;
	}

	private void Button_Click_1(object sender, RoutedEventArgs e)
	{
		if (SPCount < Max)
			SPCount++;
	}
}
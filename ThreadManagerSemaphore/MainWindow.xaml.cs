using System.Windows;

namespace ThreadManagerSemaphore;

public partial class MainWindow : Window
{
	public Semaphore Semaphore { get; set; }
	public int MaxCount { get; set; }
	public int ThreadCounter { get; set; }

	public List<string> CreatedThreads { get; set; }
	public List<string> WaitingThreads { get; set; }
	public List<string> WorkingThreads { get; set; }

	public MainWindow()
	{
		InitializeComponent();

		MaxCount = 5;
		ThreadCounter = 1;

		CreatedThreads= new List<string>();
		WaitingThreads= new List<string>();
		WorkingThreads= new List<string>();
		
		InitializeSemaphore();
	}

	private void InitializeSemaphore()
	{
		Semaphore = new Semaphore(MaxCount, MaxCount);
		UpdateCounter();
	}

	private void UpdateCounter()
	{
		txtCounter.Text = MaxCount.ToString();
	}

	private void btnCreateThread_Click(object sender, RoutedEventArgs e)
	{
		string threadName = $"Thread - {ThreadCounter++}";
		CreatedThreads.Add(threadName);
		CreatedThreadsLB.Items.Add(threadName);
	}

	private void CreatedThreadsLB_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		if (CreatedThreadsLB.SelectedItem != null)
		{
			string selectedThread = CreatedThreadsLB.SelectedItem.ToString();
			CreatedThreads.Remove(selectedThread);
			WaitingThreads.Add(selectedThread);
			CreatedThreadsLB.Items.Remove(selectedThread);
			WaitingThreadsLB.Items.Add(selectedThread);

			StartWaitingThreadsLB();
		}
	}

	private void WaitingThreadsLB_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		if (WaitingThreadsLB.SelectedItem != null)
		{
			string selectedThread = WaitingThreadsLB.SelectedItem.ToString();
			WaitingThreads.Remove(selectedThread);
			WaitingThreadsLB.Items.Remove(selectedThread);
			Semaphore.Release();
			UpdateCounter();

			StartWaitingThreadsLB();
		}
	}

	private async void StartWaitingThreadsLB()
	{
		while (WaitingThreads.Count > 0 && Semaphore.WaitOne(0))
		{
			string threadToMove = WaitingThreads[0];
			WaitingThreads.RemoveAt(0);
			WaitingThreadsLB.Items.Remove(threadToMove);

			WorkingThreads.Add(threadToMove);
			WorkingThreadsLB.Items.Add(threadToMove);

			await Task.Run(async () =>
			{
				await Task.Delay(8000);

				Dispatcher.Invoke(() =>
				{
					WorkingThreads.Remove(threadToMove);
					WorkingThreadsLB.Items.Remove(threadToMove);
					Semaphore.Release();
					UpdateCounter();
					StartWaitingThreadsLB();
				});
			});
		}
	}

	private void btnIncrease_Click(object sender, RoutedEventArgs e)
	{
		MaxCount++;
		ResetSemaphore();
	}

	private void btnDecrease_Click(object sender, RoutedEventArgs e)
	{
		if (MaxCount > 1)
		{
			MaxCount--;
			ResetSemaphore();
		}
	}

	private void ResetSemaphore()
	{
		if (WorkingThreads.Count > 0)
		{
			while (Semaphore.WaitOne(0))
				Semaphore.Release();

			Semaphore.Dispose();
			Semaphore = new Semaphore(MaxCount, MaxCount);
		}

		UpdateCounter();
	}
}
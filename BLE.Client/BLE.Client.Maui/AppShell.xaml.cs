namespace BLE.Client.Maui;

public partial class AppShell : Shell
{
    static public TabBar MainNavBar { get; private set; }

	public AppShell()
	{
		InitializeComponent();
        MainNavBar = NavBar;
    }
}


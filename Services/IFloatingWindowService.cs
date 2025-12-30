namespace PomodoroFocus.Services
{
    public interface IFloatingWindowService
    {
        void Show();
        void Hide();
        event Action OnWindowClosed;
    }
}

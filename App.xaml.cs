namespace PomodoroFocus
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // 定义我们期望的窗口尺寸
            const int DesiredWidth = 1100;
            const int DesiredHeight = 650; // 给一个足够的高度以容纳所有内容

            // 创建主窗口
            var window = new Window(new MainPage())
            {
                Title = "PomodoroFocus"
            };

            return window;
        }
    }
}

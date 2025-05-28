namespace Frame
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 注册未处理的 UI 线程异常事件
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainFrame());
        }

        // 处理 UI 线程中的未处理异常
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show($@"捕获到未处理异常: {e.Exception.Message}", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // 这里可以做更多的错误处理，如记录日志等
        }

        // 处理所有线程中的未处理异常
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            MessageBox.Show($@"捕获到未处理异常: {ex.Message}", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // 这里可以做更多的错误处理，如记录日志等
        }
    }
}